using UnityEngine;

namespace CharacterNamespace
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]

    public class ProjectileController : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 20f;
        //[SerializeField] private float lifetime = 5f; not currently required

        [Header("Collision Settings")]
        [SerializeField] private string environmentTag = "Environment";
        [SerializeField] private string enemyTag = "Enemy";

        private Transform owner;
        private Rigidbody2D rb;
        private Vector3 spawnPosition;
        private float currentDamage;
        private float maxTravelDistance;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            spawnPosition = transform.position;
            if (rb != null)
            {
                rb.linearVelocity = transform.right * speed;
            }
        }

        private void Update()
        {
            if (Vector3.Distance(spawnPosition, transform.position) >= maxTravelDistance)
            {
                Destroy(gameObject);
            }
        }

        public void Initialize(float damageAmount, float range, Transform projectileOwner)
        {
            currentDamage = damageAmount;
            Debug.Log($"Damage = {damageAmount}");
            maxTravelDistance = range;
            owner = projectileOwner;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform == owner) return;

            if (other.TryGetComponent<EnemyNamespace.EnemyHealth>(out var enemyHealth))
            {
                enemyHealth.TakeDamage(currentDamage, owner);
                Debug.Log($"<color=red>Projectile hit an enemy: {other.name}!</color>");
                Destroy(gameObject);
                return;
            }

            if (other.CompareTag(environmentTag))
            {
                Debug.Log("Collided with environment");
                Destroy(gameObject);
            }
        }
    }
}
