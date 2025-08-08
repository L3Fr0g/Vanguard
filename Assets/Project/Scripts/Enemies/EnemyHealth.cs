using UnityEngine;
using UnityEngine.Events;
using UINamespace;

namespace EnemyNamespace
{
    public interface IAIController
    {   
        void OnAttacked(Transform attacker);
        void StartRespawnCycle();
        void OnDeath();
    }

    public class EnemyHealth : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] private GameObject nameplatePrefab;

        [Header("Health")]
        [SerializeField] private float maxHealth = 50f;
        private float currentHealth;

        [Header("Events")]
        public UnityEvent<float, float> OnHealthChanged;
        public UnityEvent OnDie;

        private IAIController AIController;
        private Animator animator;
        private EnemyBuffManager buffManager;
        private EnemyStats enemyStats;
        private NameplateController nameplateController;
        private ThreatManager threatManager;

        private void Awake()
        {
            enemyStats = GetComponent<EnemyStats>();
            buffManager = GetComponent<EnemyBuffManager>();
            animator = GetComponent<Animator>();
            threatManager = GetComponent<ThreatManager>();
            AIController = GetComponent<IAIController>();
        }

        private void Start()
        {
            if (nameplatePrefab != null)
            {
                GameObject nameplateInstance = Instantiate(nameplatePrefab, transform);
                if (nameplateInstance.TryGetComponent<NameplateController>(out nameplateController))
                {
                    OnHealthChanged.AddListener(nameplateController.UpdateHealth);
                    nameplateController.SetName(gameObject.name);
                }
            }
            currentHealth = enemyStats.MaxHealth;
            OnHealthChanged.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(float damageAmount, Transform damageSource)
        {
            if (currentHealth <= 0) return;

            currentHealth -= damageAmount;
            if (currentHealth < 0) currentHealth = 0;

            OnHealthChanged.Invoke(currentHealth, maxHealth);

            if (threatManager != null)
            {
                threatManager.AddThreat(damageSource, damageAmount);
            }

            if (buffManager != null)
            {
                buffManager.UpdateBuffs(currentHealth, enemyStats.MaxHealth);
            }

            if (AIController != null)
            {
                AIController.OnAttacked(damageSource);
            }

            Debug.Log($"<color=yellow>{gameObject.name} took {damageAmount} damage, has {currentHealth} health remaining.</color>");

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                animator.SetTrigger("Hit");
            }
        }

        private void Die()
        {
            Debug.Log($"<color=red>{gameObject.name} has died.</color>");

            nameplateController.Hide();
            OnDie.Invoke();
            animator.SetTrigger("Death");
            animator.SetBool("isDead", true);
            GetComponent<Collider2D>().enabled = false;

            if (AIController != null)
            {
                (AIController as MonoBehaviour).enabled = false;
                AIController.StartRespawnCycle();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ResetHealth()
        {
            currentHealth = enemyStats.MaxHealth;
            nameplateController.Show();
            if (buffManager != null)
            {
                buffManager.UpdateBuffs(currentHealth, enemyStats.MaxHealth);
            }
            OnHealthChanged.Invoke(currentHealth, enemyStats.MaxHealth);
            GetComponent<Collider2D>().enabled = true;
            if (AIController != null) (AIController as MonoBehaviour).enabled = true;
            animator.SetBool("isDead", false);
        }
    }
}
