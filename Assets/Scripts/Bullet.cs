using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 20f;
    public bool isPlayerBullet = true; // To determine if bullet is from player or enemy
    
    // Add a reference to the renderer for visual debugging
    private Renderer bulletRenderer;
    
    private void Start()
    {
        // Get renderer component if it exists
        bulletRenderer = GetComponent<Renderer>();
        
        // Set different colors for player vs enemy bullets
        if (bulletRenderer != null)
        {
            bulletRenderer.material.color = isPlayerBullet ? Color.blue : Color.red;
        }
        
        // Log bullet creation for debugging
        Debug.Log((isPlayerBullet ? "Player" : "Enemy") + " bullet created");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Log collision for debugging
        Debug.Log("Bullet hit: " + collision.gameObject.name + " with tag: " + collision.gameObject.tag);
        
        // Handle collision with enemy
        if (isPlayerBullet && collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Player bullet hit enemy for " + damage + " damage");
            }
            else
            {
                Debug.LogWarning("Hit object tagged as Enemy but no Enemy component found");
            }
        }
        
        // Handle collision with player
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///Perhaps if we can get the position of the player, we can push the player back in line with the force of the bullet.
        else if (!isPlayerBullet && collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Enemy bullet hit player for " + damage + " damage");
            }
            else
            {
                Debug.LogWarning("Hit object tagged as Player but no PlayerHealth component found");
            }
        }
        
        // Destroy the bullet in any case
        Destroy(gameObject);
    }
    
    // Alternative using trigger instead of collision
    private void OnTriggerEnter(Collider other)
    {
        // Log trigger for debugging
        Debug.Log("Bullet trigger with: " + other.gameObject.name + " with tag: " + other.gameObject.tag);
        
        // Handle collision with enemy
        if (isPlayerBullet && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Player bullet triggered enemy for " + damage + " damage");
            }
        }
        
        // Handle collision with player
        else if (!isPlayerBullet && other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Enemy bullet triggered player for " + damage + " damage");
            }
        }
        
        // Destroy the bullet in any case
        Destroy(gameObject);
    }
}