using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float lifetime = 4f;

    private Transform player;
    private Vector2 direction;

    void Start()
    {
        // Find player and move toward them
        player = FindFirstObjectByType<Player>()?.transform;
        if (player != null)
            direction = (player.position - transform.position).normalized;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}