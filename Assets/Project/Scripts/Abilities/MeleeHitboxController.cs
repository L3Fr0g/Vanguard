using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterNamespace
{

    [RequireComponent(typeof(BoxCollider2D))]
    public class MeleeHitboxController : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float activeDuration = 0.15f;
        [SerializeField] private float hitboxWidth = 1f;

        private Transform owner;
        private BoxCollider2D hitboxCollider;
        private List<Collider2D> alreadyHitTargets = new List<Collider2D>();
        private float currentDamage;

        private void Awake()
        {
            hitboxCollider = GetComponent<BoxCollider2D>();
            hitboxCollider.enabled = false;
        }

        public void PerformAttack(int damage, Vector2 direction, float range, Transform attackOwner)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            hitboxCollider.size = new Vector2(range, hitboxWidth);
            hitboxCollider.offset = new Vector2(range / 2f, 0);

            currentDamage = damage;
            owner = attackOwner;
            StartCoroutine(AttackSequence());
        }

        private IEnumerator AttackSequence()
        {
            alreadyHitTargets.Clear();
            hitboxCollider.enabled = true;
            yield return new WaitForSeconds(activeDuration);
            hitboxCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.transform == owner || alreadyHitTargets.Contains(other)) return;

            if (other.TryGetComponent<EnemyNamespace.EnemyHealth>(out var enemyHealth))
            {
                enemyHealth.TakeDamage(currentDamage, owner);
                alreadyHitTargets.Add(other);
            }

            if (other.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(currentDamage);
                Debug.Log($"<color=orange>{this.name} hit {other.name} for {currentDamage} damage!</color>");
                alreadyHitTargets.Add(other);
            }
        }
    }
}
