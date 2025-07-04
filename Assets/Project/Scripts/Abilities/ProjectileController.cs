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

        private Rigidbody2D rb;
        private float currentDamage;
        private Vector3 spawnPosition;
        private float maxTravelDistance;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
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

        public void Initialize(float damageAmount, float range)
        {
            currentDamage = damageAmount;
            maxTravelDistance = range;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) return;

            if (other.CompareTag(enemyTag))
            {
                Debug.Log($"<color=redProjectile hit an enemy: {other.name}!</color>");

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
