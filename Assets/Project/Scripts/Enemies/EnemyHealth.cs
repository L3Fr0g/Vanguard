using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;

    [Header("Events")]
    public UnityEvent OnTakeDamage;
    public UnityEvent OnDie;

    private Animator animator;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damageAmount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damageAmount;

        OnTakeDamage.Invoke();

        Debug.Log($"<color=yellow>{gameObject.name} took {damageAmount} damage, has {currentHealth} health remaining.</color>");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (animator != null) animator.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        Debug.Log($"<color=red>{gameObject.name} has died.</color>");
        OnDie.Invoke();
        if (animator != null) animator.SetTrigger("Death");

        GetComponent<Collider2D>().enabled = false;
        if (TryGetComponent<EnragedShroomianAI>(out var ai)) ai.enabled = false;
        this.enabled = false;
    }
}
