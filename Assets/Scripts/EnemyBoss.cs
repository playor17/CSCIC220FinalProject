using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public int maxHp = 50; // Boss's maximum HP
    private int currentHp; // Current HP

    public float moveSpeed = 3f; // Horizontal movement speed
    public Transform leftPoint; // Left movement limit
    public Transform rightPoint; // Right movement limit
    private bool movingRight = true; // Current movement direction

    public GameObject fireballPrefab; // Fireball Prefab
    public Transform fireballSpawnPoint; // Fireball spawn position
    public float fireballSpeed = 5f; // Fireball speed

    public GameObject gemPrefab; // Gem Prefab
    public Transform gemSpawnPoint; // Gem spawn position

    private bool isFiring = false; // Whether the boss has started firing fireballs

    void Start()
    {
        currentHp = maxHp;
    }

    void Update()
    {
        MoveHorizontally();

        // Remove the boss if its HP is 0 or below
        if (currentHp <= 0)
        {
            DropGem(); // Spawn the gem
            Destroy(gameObject); // Remove the boss
            Debug.Log("Boss defeated!");
        }
    }

    private void MoveHorizontally()
    {
        if (movingRight)
        {
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            if (transform.position.x >= rightPoint.position.x)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            if (transform.position.x <= leftPoint.position.x)
            {
                movingRight = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            currentHp--;
            Debug.Log($"Boss HP: {currentHp}");

            Destroy(collision.gameObject); // Remove the bullet

            // Condition to start firing fireballs
            if (!isFiring && currentHp <= maxHp - 10)
            {
                isFiring = true;
                StartCoroutine(FireContinuously());
            }
        }
    }

    private IEnumerator FireContinuously()
    {
        while (isFiring)
        {
            // Fire fireballs in both directions
            FireFireball(Vector2.left);  // Left
            FireFireball(Vector2.right); // Right
            yield return new WaitForSeconds(3f); // Wait for 3 seconds
        }
    }

    private void FireFireball(Vector2 direction)
    {
        // Spawn a fireball
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);

        // Set fireball speed
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        rb.velocity = direction.normalized * fireballSpeed;

        // Destroy the fireball after 2 seconds
        Destroy(fireball, 2f);
    }

    private void DropGem()
    {
        // Spawn the gem
        Instantiate(gemPrefab, gemSpawnPoint.position, Quaternion.identity);
        Debug.Log("Gem dropped!");
    }
}
