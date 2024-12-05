using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // Bullet speed
    public float lifetime = 2f; // Bullet lifetime
    private Vector2 direction; // Bullet direction

    public void SetDirection(Vector2 bulletDirection)
    {
        direction = bulletDirection.normalized; // Normalize the direction
    }

    void Start()
    {
        // Destroy the bullet after the specified lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the bullet
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
