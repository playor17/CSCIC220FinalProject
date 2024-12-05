using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene transitions

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Movement speed
    public float jumpForce = 10f; // Jump force
    public float fallThreshold = -10f; // Height threshold to detect falling off the map

    private Rigidbody2D rb; // Reference to Rigidbody2D
    private bool isGrounded = false; // Whether the player is on the ground
    private bool facingRight = true; // Whether the player is facing right

    public GameObject playerWithGunPrefab; // Prefab of Player with Gun
    private bool hasGun = false; // Whether the player has picked up the gun

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // Lock Z-axis rotation

        // Register the current player in GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetCurrentPlayer(gameObject);
        }
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        // Check if the player has fallen off the map
        if (transform.position.y < fallThreshold)
        {
            Debug.Log("Player has fallen out of bounds!");
            SceneManager.LoadScene("Failed"); // Transition to the "Failed" scene
        }
    }

    private void Flip()
    {
        // Flip the player's direction
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Reverse X-axis scale
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Set isGrounded to true when the player touches the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // Reduce HP when colliding with an Enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeHp(-1); // Decrease HP
                Debug.Log($"Player hit by Enemy! Current HP: {GameManager.Instance.playerHp}");
            }
            else
            {
                Debug.LogError("GameManager instance is not set!");
            }
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
        Debug.Log($"Collision detected with: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");

        // Reduce HP when hit by a Fireball (EnemyBullet)
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeHp(-1); // Decrease HP
                Debug.Log($"Player hit by Fireball! Current HP: {GameManager.Instance.playerHp}");
            }
            else
            {
                Debug.LogError("GameManager instance is not set!");
            }

            // Destroy the fireball
            Destroy(collision.gameObject);
        }

        // Check if the triggered object is a Gun
        if (collision.gameObject.CompareTag("Gun") && !hasGun)
        {
            hasGun = true;
            Debug.Log("Player picked up the gun!");

            // Save the current player's position and rotation
            Vector3 currentPosition = transform.position;
            Quaternion currentRotation = transform.rotation;

            // Instantiate a new PlayerWithGun prefab
            GameObject newPlayer = Instantiate(playerWithGunPrefab, currentPosition, currentRotation);
            newPlayer.tag = "Player";

            // Update references for EnemyBee objects
            EnemyBee[] bees = FindObjectsOfType<EnemyBee>();
            StartCoroutine(UpdateBeeReferencesDelayed(bees));

            // Destroy the current player object
            Destroy(gameObject);

            // Destroy the Gun object
            Destroy(collision.gameObject);
        }
    }

    private IEnumerator UpdateBeeReferencesDelayed(EnemyBee[] bees)
    {
        yield return new WaitForEndOfFrame(); // Wait for one frame
        foreach (EnemyBee bee in bees)
        {
            bee.UpdatePlayerReference();
            Debug.Log($"EnemyBee updated to track new player.");
        }
    }
}
