using UnityEngine;

public class GolemController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Передаём скорость движения
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    public void Attack()
    {
        animator.SetBool("IsAttacking", true);
    }

    public void StopAttack()
    {
        animator.SetBool("IsAttacking", false);
    }

    public void Die()
    {
        animator.SetBool("IsDead", true);
        // Отключи коллайдер и логику
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}