using UnityEngine;
using UnityEngine.UI; // For UI health display if needed

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    
    // Optional UI elements
    public Slider healthBar;
    
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        // Ensure player has the Player tag
        if (gameObject.tag != "Player")
        {
            gameObject.tag = "Player";
            Debug.Log("Set Player tag on player object");
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took " + damage + " damage. Health: " + currentHealth);
        
        UpdateHealthUI();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void UpdateHealthUI()
    {
        // Update UI if you have a health bar
        if (healthBar != null)
        {
            healthBar.value = currentHealth / maxHealth;
        }
    }
    
    void Die()
    {
        Debug.Log("Player died!");
        // Handle player death
        // For testing, let's respawn
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        // Optional: Teleport player to spawn point
        // transform.position = spawnPoint.position;
    }
}