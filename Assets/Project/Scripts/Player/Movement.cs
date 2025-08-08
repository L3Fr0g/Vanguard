using UnityEngine;
using System.Collections;

namespace CharacterNamespace
{
    public class Movement : MonoBehaviour
    {
        public enum MovementState {  Sheathed, Unsheathed, Attacking, Casting }

        [Header("Movement Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float unsheathedSpeedMultiplier = 0.75f;
        [Range(0f, 1f)]
        [SerializeField] private float attackingSpeedMultiplier = 0.5f;
        [Range(0f, 1f)]
        [SerializeField] private float castingSpeedMultiplier = 0.1f;

        private CharacterStats stats;
        private Rigidbody2D rb;

        private Vector2 moveDirection;
        private MovementState currentState = MovementState.Sheathed;
        private bool isKnockbackActive = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            stats = GetComponent<CharacterStats>();
        }

        private void FixedUpdate()
        {
            if (isKnockbackActive) return;

            float speedMultiplier = 1f;

            switch (currentState)
            {
                case MovementState.Sheathed: speedMultiplier = 1f; Debug.Log($"{this.name}: Sheathed State"); break;
                case MovementState.Unsheathed: speedMultiplier = unsheathedSpeedMultiplier; Debug.Log($"{this.name}: Unsheathed State"); break;
                case MovementState.Attacking: speedMultiplier = attackingSpeedMultiplier; Debug.Log($"{this.name}: Attacking State"); break;
                case MovementState.Casting: speedMultiplier = castingSpeedMultiplier; Debug.Log($"{this.name}: Casting State"); break;
            }
            float finalSpeed = stats.MovementSpeed * speedMultiplier;
            rb.linearVelocity = moveDirection * finalSpeed;
        }

        public void SetMoveDirection(Vector2 direction)
        {
            moveDirection = direction.normalized;
        }

        public void SetMovementState(MovementState newState)
        {
            currentState = newState;
        }

        public void ApplyKnockback(Vector2 force, float duration)
        {
            StartCoroutine(KnockbackCoroutine(force, duration));
        }

        private IEnumerator KnockbackCoroutine(Vector2 force, float duration)
        {
            isKnockbackActive = true;
            rb.linearVelocity = force;
            yield return new WaitForSeconds(duration);
            isKnockbackActive = false;
        }
    }
}
