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

        private BoxCollider2D hitboxCollider;
        private float currentDamage;

        private List<Collider2D> alreadyHitTargets = new List<Collider2D>();

        private void Awake()
        {
            hitboxCollider = GetComponent<BoxCollider2D>();
            hitboxCollider.enabled = false;
        }

        public void PerformAttack(float damage, Vector2 direction, float range)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            if (hitboxCollider != null )
            {
                hitboxCollider.size = new Vector2(range, hitboxWidth);
                hitboxCollider.offset = new Vector2(range / 2f, 0);
            }

            currentDamage = damage;
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
            if (other.CompareTag("Player") || alreadyHitTargets.Contains(other))
            {
                return;
            }

            if (other.CompareTag("Enemy"))
            {
                Debug.Log($"Polearm hit {other.name} for {currentDamage}");
                alreadyHitTargets.Add(other);
            }
        }
    }
}
