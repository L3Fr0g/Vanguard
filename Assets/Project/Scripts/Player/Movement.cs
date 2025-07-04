using UnityEngine;

namespace MovementNamespace
{
    public class Movement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float deceleration = 100f;

        private Rigidbody2D rb;
        private Vector2 targetVelocity;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            float currentAccel = targetVelocity.magnitude > 0.1f ? acceleration : deceleration;

            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, currentAccel * Time.fixedDeltaTime);
        }

        public void SetTargetVelocity(Vector2 velocity)
        {
            targetVelocity = velocity;
        }

        public void ApplyKnockback(Vector2 force)
        {
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
