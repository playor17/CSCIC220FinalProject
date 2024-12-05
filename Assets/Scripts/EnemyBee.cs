using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBee : MonoBehaviour
{
    public float speed = 3f; // Movement speed of the bee
    public int damage = 1;   // Damage dealt to the player
    private Transform player; // Current player's Transform
    private Rigidbody2D rb;   // Rigidbody2D of the bee

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdatePlayerReference(); // Initialize player reference
        rb.freezeRotation = true; // Lock Z-axis rotation
    }

    void FixedUpdate()
    {
        // Chase the player
        if (player == null)
        {
            UpdatePlayerReference(); // Update reference if player is null
        }

        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * speed;
        }
        else
        {
            Debug.LogWarning("Player reference is null. EnemyBee cannot move.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Bullet hit the Enemy!");
            Destroy(gameObject); // Destroy the bee
            Destroy(collision.gameObject); // Destroy the bullet
        }
    }

    public void UpdatePlayerReference()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length > 0)
        {
            player = players[0].transform; // Reference the first "Player" object found
            Debug.Log($"EnemyBee now tracking new Player: {player.name}");
        }
        else
        {
            Debug.LogWarning("No Player found with the 'Player' tag.");
        }

        Debug.Log($"Number of Player objects found: {players.Length}");
    }
}
