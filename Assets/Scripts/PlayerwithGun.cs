using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene transitions

public class PlayerWithGun : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public float jumpForce = 10f; // Jump strength
    public float fallThreshold = -10f; // Height threshold to detect falling off the map

    private Rigidbody2D rb; // Reference to Rigidbody2D
    private bool isGrounded = false; // Whether the player is on the ground
    private bool facingRight = true; // Whether the player is facing right

    public GameObject bulletPrefab; // Bullet prefab
    public Transform firePoint; // Bullet firing position

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // Lock Z-axis rotation
    }

    void Update()
    {
        // Horizontal movement
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Flip the sprite based on movement direction
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Shoot bullet
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot();
        }

        // Check if the player has fallen off the map
        if (transform.position.y < fallThreshold)
        {
            SceneManager.LoadScene("Failed"); // Transition to the "Failed" scene
        }
    }

    private void Flip()
    {
        facingRight = !facingRight; // Reverse direction
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Flip the X-axis
        transform.localScale = scale;
    }

    private void Shoot()
    {
        // Create a bullet instance
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Get the Rigidbody2D of the bullet
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        // Set bullet speed based on player direction
        float bulletSpeed = 10f;
        Vector2 bulletDirection = facingRight ? Vector2.right : Vector2.left;
        bulletRb.velocity = bulletDirection * bulletSpeed;

        // Flip the bullet's sprite if necessary
        if (!facingRight)
        {
            Vector3 bulletScale = bullet.transform.localScale;
            bulletScale.x = Mathf.Abs(bulletScale.x) * -1; // Flip the X-axis
            bullet.transform.localScale = bulletScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Set isGrounded to true when the player touches the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // Reduce HP when colliding with an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameManager.Instance.ChangeHp(-1); // Decrease HP
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Set isGrounded to false when the player leaves the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Reduce HP when hit by a Fireball (EnemyBullet)
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeHp(-1); // Decrease HP
            }
            else
            {
                Debug.LogError("GameManager instance is not set!");
            }

            // Destroy the fireball
            Destroy(collision.gameObject);
        }
    }
}
