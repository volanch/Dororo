using UnityEngine;
using System.Collections;
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHp = 3;
    protected int currentHp;
    public int damage = 1;
    public int scoreValue = 10;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float detectionRange = 5f;
    [SerializeField] protected float attackRange = 0.8f;
    [SerializeField] protected float attackCooldown = 1.5f;

    [Header("VFX")]
    [SerializeField] private GameObject deathEffect;

    protected Transform player;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected float attackTimer = 0f;
    protected bool isDead = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHp = maxHp;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    protected virtual void Update()
    {
        if (isDead || player == null) return;
        attackTimer -= Time.deltaTime;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < attackRange && attackTimer <= 0f)
        {
            AttackPlayer();
            attackTimer = attackCooldown;
        }
        else if (dist < detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }
    }

   protected virtual void ChasePlayer()
{
    float dist = Vector2.Distance(transform.position, player.position);
    
    // Останавливается чуть раньше чем войти в коллайдер игрока
    if (dist <= attackRange + 0.3f)
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator?.SetFloat("Speed", 0f);
        return;
    }

    float dir = player.position.x > transform.position.x ? 1f : -1f;
    rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    
    transform.localScale = new Vector3(
        dir * Mathf.Abs(transform.localScale.x),
        transform.localScale.y,
        transform.localScale.z);
        
    animator?.SetFloat("Speed", 1f);
}

    protected virtual void Idle()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator?.SetFloat("Speed", 0f);
    }

   protected virtual void AttackPlayer()
{
    rb.linearVelocity = Vector2.zero;
    StartCoroutine(AttackCoroutine());
}

    private IEnumerator AttackCoroutine()
    {
        animator?.SetTrigger("Attack");
        
        yield return new WaitForSeconds(1.4f);
        if(Vector2.Distance(transform.position, player.position)<2)
        
        player.GetComponent<Player>()?.TakeDamage(damage);
    }

    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHp -= damage;
        Debug.Log("Получил урон");
        if (currentHp <= 0) Die();
    }

    protected virtual void Die()
{
    isDead = true;
    animator?.SetTrigger("Die");
    rb.linearVelocity = Vector2.zero;
    rb.bodyType = RigidbodyType2D.Kinematic; // ← добавь эту строку
    GetComponent<Collider2D>().enabled = false;
    GameManager.Instance.AddScore(scoreValue);
    GameManager.Instance.EnemyKilled();

    if (deathEffect != null)
        Instantiate(deathEffect, transform.position, Quaternion.identity);

    Destroy(gameObject, 1.5f);
}
}