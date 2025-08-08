using UnityEngine;
using UnityEngine.Events;
//using AnimationNamespace;
using UINamespace;

namespace CharacterNamespace
{
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Component References")]
        //[SerializeField] private AnimationController animationController;
        [SerializeField] private GameObject nameplatePrefab;

        private NameplateController nameplateController;
        private PlayerStats playerStats;
        private float currentHealth;

        public UnityEvent<float, float> OnHealthChanged;

        private void Awake()
        {
            //animationController = GetComponent<AnimationController>();
            playerStats = GetComponent<PlayerStats>();
        }

        private void Start()
        {
            currentHealth = playerStats.MaxHealth;

            if (nameplatePrefab != null)
            {
                GameObject nameplateInstance = Instantiate(nameplatePrefab, transform);
                if(nameplateInstance.TryGetComponent<NameplateController>(out nameplateController))
                {
                    OnHealthChanged.AddListener(nameplateController.UpdateHealth);
                    nameplateController.SetName(gameObject.name);
                }
            }

            OnHealthChanged.Invoke(currentHealth, playerStats.MaxHealth);
        }

        public void TakeDamage(float damageAmount)
        {
            if (currentHealth <= 0) return;

            float damageTaken = damageAmount;

            currentHealth -= damageTaken;
            if (currentHealth < 0) currentHealth = 0;

            OnHealthChanged.Invoke(currentHealth, playerStats.MaxHealth);

            Debug.Log($"<color=orange>Player took {damageTaken} damage, has {currentHealth} health remaining. </color>");

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                //animationController.SetTrigger("Hit");
            }
        }

        private void Die()
        {
            Debug.Log("<color=red>Player has died.");

            nameplateController.Hide();

            if (TryGetComponent<PlayerController>(out var controller))
            {
                controller.enabled = false;
            }
        }
    }
}
