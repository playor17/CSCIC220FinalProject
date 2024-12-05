using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyHp = 3; // Enemy's HP (default: 3)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if collided with a bullet
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Bullet hit the Enemy!");

            // Decrease HP
            enemyHp--;

            // If HP is 0 or less, destroy the enemy
            if (enemyHp <= 0)
            {
                Destroy(gameObject); // Destroy the enemy object
            }

            Destroy(collision.gameObject); // Destroy the bullet
        }
    }
}
