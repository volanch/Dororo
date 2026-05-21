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

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    // Invincibility frames after getting hit
    private bool isInvincible = false;
    [SerializeField] private float invincibilityDuration = 0.5f;

    // Footstep timer
    private float footstepTimer = 0f;
    [SerializeField] private float footstepInterval = 0.35f;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private float fallDeathY = -10f; // порог падения

    // Components
    private Rigidbody2D rb;
    private Animator animator;

    // State
    private float horizontalInput;
    private bool isGrounded;
    private bool isDead = false;
    public bool dialoguePlaying = false;

    // Combo attack
    private bool isAttacking = false;
    private bool comboQueued = false;
    private int attackStep = 0;

    // Dash state
    private bool isDashing = false;
    private bool canDash = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHp = maxHp;
        GameManager.Instance?.UpdateHpUI();
    }

    void Update()
    {
        if (isDead) return;
        if (isDashing) return;
        if (dialoguePlaying) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            SoundManager.Instance?.PlayJump();
        }

        // Dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
            StartCoroutine(Dash());

        // Attack combo
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
                StartAttack1();
            else if (attackStep == 1)
                comboQueued = true;
        }

        // Footstep sounds while walking on ground
        if (isGrounded && Mathf.Abs(horizontalInput) > 0)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                footstepTimer = footstepInterval;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        // Animator parameters
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);

        // Падение в яму
        if (transform.position.y < fallDeathY && !isDead)
            Die();
    }

    void FixedUpdate()
    {
        if (isDead || isDashing) return;
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
        SoundManager.Instance?.PlayDash();

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
        SoundManager.Instance?.PlaySwordSlice();
        DoAttackHit();
        Invoke(nameof(CheckCombo), 0.35f);
    }

    void CheckCombo()
    {
        if (comboQueued)
        {
            attackStep = 2;
            comboQueued = false;
            animator.SetTrigger("Attack2");
            SoundManager.Instance?.PlaySwordSlice();
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
        SoundManager.Instance?.PlayHit();
        animator.SetTrigger("Hit");
        GameManager.Instance?.UpdateHpUI();

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
        DeathCounter.Instance?.AddDeath();
        animator.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        // Показываем экран поражения только на LevelGolem и Level3
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if ((scene == "LevelGolem" || scene == "Level3") && gameOverCanvas != null)
        {
            Invoke(nameof(ShowGameOver), 1f); // небольшая задержка после анимации смерти
        }
        else
        {
            Invoke(nameof(ReloadScene), 2f);
        }
    }

    void ShowGameOver()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f; // пауза игры
    }

    void ReloadScene()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public bool IsGrounded() => isGrounded;

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