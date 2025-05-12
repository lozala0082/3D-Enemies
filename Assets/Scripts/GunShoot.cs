using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [SerializeField] GameObject referenceProjectile;
    [SerializeField] Transform barrel;
    [SerializeField] float projectileSpeed = 50f;
    [SerializeField] float projectileLifetime = 5f;

    Vector3 destination;

    void Start()
    {
        if (referenceProjectile == null)
        {
            Debug.LogError("No projectile assigned to GunShoot script!");
        }
        
        if (barrel == null)
        {
            Debug.LogWarning("No barrel transform assigned, using this transform");
            barrel = transform;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnFire();
        }
    }

    void CreateProjectile()
    {
        if (referenceProjectile == null) return;
        
        // Instantiate at barrel position with forward rotation
        GameObject projectile = Instantiate(referenceProjectile, barrel.position, barrel.rotation);
        
        // Set it as player bullet
        Bullet bulletScript = projectile.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.isPlayerBullet = true;
        }
        else
        {
            // Add bullet script if it doesn't exist
            bulletScript = projectile.AddComponent<Bullet>();
            bulletScript.isPlayerBullet = true;
        }
        
        // Make sure the projectile has a rigidbody
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = projectile.AddComponent<Rigidbody>();
        }
        
        // Add tag if needed
        projectile.tag = "Bullet";
        
        // Calculate direction
        Vector3 direction = (destination - projectile.transform.position).normalized;
        
        // Add force
        rb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
        
        // Destroy after lifetime
        Destroy(projectile, projectileLifetime);
        
        Debug.Log("Player fired projectile!");
    }

    void OnFire()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            destination = hit.point;
            
            // Debug hit info
            Debug.Log("Hit: " + hit.collider.gameObject.name);
            
            // If we hit an enemy directly with the ray, damage them
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(20f); // Direct hit bonus damage
                    Debug.Log("Direct hit on enemy!");
                }
            }
        }
        else
        {
            destination = ray.GetPoint(1000);
        }

        CreateProjectile();
    }
}