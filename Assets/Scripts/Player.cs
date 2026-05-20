using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    public int maxHp = 5;
    public int currentHp;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 15f;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.7f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int attackDamage = 1;
    
    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.333f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private AudioClip dashSound;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hurtSound;

    // Components
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    // State
    private float horizontalInput;
    private bool isGrounded;
    private bool isDead = false;

    // Combo attack
    private bool isAttacking = false;
    private bool comboQueued = false;
    private int attackStep = 0;
    
    //dashing state
    private bool isDashing = false;
    private bool canDash = true;

    // Invincibility frames after getting hit
    private bool isInvincible = false;
    [SerializeField] private float invincibilityDuration = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentHp = maxHp;
        GameManager.Instance?.UpdateHpUI();
    }

    void Update()
    {
        if (isDead) return;
        if (isDashing) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            audioSource.PlayOneShot(jumpSound);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
            StartCoroutine(Dash());
        
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
                StartAttack1();
            else if (attackStep == 1)
                comboQueued = true; // queue Attack2 during Attack1
        }

        // Animator parameters
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    void FixedUpdate()
    {
        if (isDead) return;
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }
    
    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        isInvincible = true;
 
        float dashDirection = transform.localScale.x;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
 
        animator.SetTrigger("Dash");
        audioSource.PlayOneShot(dashSound);
 
        yield return new WaitForSeconds(dashDuration);
 
        rb.gravityScale = 4f;
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        isInvincible = false;
 
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void StartAttack1()
    {
        isAttacking = true;
        attackStep = 1;
        comboQueued = false;
        animator.SetTrigger("Attack1");
        audioSource.PlayOneShot(attackSound);
        DoAttackHit();
        Invoke(nameof(CheckCombo), 0.35f); // combo window
    }

    void CheckCombo()
    {
        if (comboQueued)
        {
            // Chain into Attack2
            attackStep = 2;
            comboQueued = false;
            animator.SetTrigger("Attack2");
            audioSource.PlayOneShot(attackSound);
            DoAttackHit();
            Invoke(nameof(EndAttack), 0.4f);
        }
        else
        {
            EndAttack();
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        attackStep = 0;
        comboQueued = false;
    }

    void DoAttackHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position, attackRange, enemyLayer);

        foreach (var hit in hits)
            hit.GetComponent<Enemy>()?.TakeDamage(attackDamage);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return;

        currentHp -= damage;
        audioSource.PlayOneShot(hurtSound);
        animator.SetTrigger("Hit");
        GameManager.Instance?.UpdateHpUI();

        // Brief invincibility so enemy doesn't hit every frame
        isInvincible = true;
        Invoke(nameof(EndInvincibility), invincibilityDuration);

        if (currentHp <= 0)
            Die();
    }

    void EndInvincibility()
    {
        isInvincible = false;
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; // stop all physics
        GetComponent<Collider2D>().enabled = false;
        Invoke(nameof(ReloadScene), 2f);
    }

    void ReloadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public bool IsGrounded() => isGrounded;

    public void PlayCoinSound(AudioClip coinSound)
    {
        audioSource.PlayOneShot(coinSound);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}