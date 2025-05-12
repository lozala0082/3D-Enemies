using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{   
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask Ground, whatIsPlayer;
    public float health = 100f;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 10f;

    //Attacking
    public float timeBetweenAttacks = 2f;
    bool alreadyAttacked;
    public GameObject projectile;
    
    //States
    public float sightRange = 15f, attackRange = 10f;
    public bool playerInSightRange, playerInAttackRange;

    // Debug variables
    private float lastAttackTime = 0f;
    private int attackAttempts = 0;

    private void Awake()
    {
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("No player found with tag 'Player'");
            }
        }
        
        // Get NavMeshAgent if not assigned
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError("No NavMeshAgent component found on " + gameObject.name);
            }
        }
        
        // Make sure the enemy has the Enemy tag
        if (gameObject.tag != "Enemy")
        {
            gameObject.tag = "Enemy";
            Debug.Log("Set tag to Enemy on " + gameObject.name);
        }
    }
    
    private void Start()
    {
        // If projectile is not assigned, show warning
        if (projectile == null)
        {
            Debug.LogError("Enemy projectile is not assigned to " + gameObject.name);
        }
        
        // Set up starting health
        health = 100f;
        
        Debug.Log("Enemy initialized: " + gameObject.name + " with " + health + " health");
    }
    
    private void Update()
    {
        // Skip if no player found
        if (player == null) return;
        
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet && agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, Ground))
            walkPointSet = true;
    }
    
    private void ChasePlayer()
    {
        if (agent != null && agent.isActiveAndEnabled && player != null)
        {
            agent.SetDestination(player.position);
        }
    }
    
    private void AttackPlayer()
    {
        // Safety check to prevent NullReferenceException
        if (agent == null || player == null) return;
        
        // Make sure enemy doesn't move while attacking
        if (agent.isActiveAndEnabled)
        {
            agent.SetDestination(transform.position);
        }
        
        // Look at player
        Vector3 lookPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPosition);
        
        // Track attack attempts
        attackAttempts++;
        
        // Log debug info
        Debug.Log("Attack attempt #" + attackAttempts + ". alreadyAttacked=" + alreadyAttacked + ", projectile=" + (projectile != null));
        
        if (!alreadyAttacked && projectile != null)
        {
            lastAttackTime = Time.time;
            
            // Calculate the direction to shoot
            Vector3 shootDirection = (player.position - transform.position).normalized;
            
            // Create projectile slightly in front of enemy to avoid self-collision
            Vector3 projectilePos = transform.position + transform.forward * 1.5f + Vector3.up * 1.5f;
            
            // Instantiate the projectile
            GameObject bulletInstance = Instantiate(projectile, projectilePos, Quaternion.identity);
            
            // Make sure the bullet has a rigidbody
            Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = bulletInstance.AddComponent<Rigidbody>();
            }
            
            // Configure the bullet as enemy bullet
            Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
            if (bulletScript == null)
            {
                bulletScript = bulletInstance.AddComponent<Bullet>();
            }
            
            bulletScript.isPlayerBullet = false;
            bulletScript.damage = 10f;
            
            // Add force to the projectile
            rb.AddForce(shootDirection * 20f, ForceMode.Impulse);
            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse); // Add slight upward force
            
            Debug.Log("Enemy " + gameObject.name + " fired projectile at player!");
            
            // Destroy the projectile after 5 seconds if it doesn't hit anything
            Destroy(bulletInstance, 5f);
            
            alreadyAttacked = true;

            // Reset attack after cooldown
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        Debug.Log("Enemy " + gameObject.name + " reset attack state, ready to fire again");
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Health remaining: " + health);
        
        if (health <= 0) 
        {
            Debug.Log(gameObject.name + " died!");
            Die();
        }
    }
    
    void Die()
    {
        Debug.Log(gameObject.name + " started death sequence");
        // Disable components instead of destroying immediately for debugging
        if (agent != null)
        {
            agent.enabled = false;
        }
        
        // Disable all colliders
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }
        
        // Change color to red to indicate death
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            if (r.material != null)
            {
                r.material.color = Color.red;
            }
        }
        
        // Destroy after delay
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}