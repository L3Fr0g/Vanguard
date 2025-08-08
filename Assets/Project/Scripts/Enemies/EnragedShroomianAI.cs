/*using UnityEngine;
using System.Collections;
using CharacterNamespace;

namespace Enemy
{
    public class EnragedShroomianAI : BaseMeleeAI
    {
        // Override the ChangeState method to add Shroomian-specific animation logic
        protected override void ChangeState(State newState)
        {
            base.ChangeState(newState); // Run all the logic from the base class

            // Now, add the extra logic for this specific enemy
            if (animator != null)
            {
                animator.SetBool("isPlanted", newState == State.Idle);
            }
        }

        protected override void UpdateIdleState()
        {
            // First, check for proximity aggro
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, aggroRadius, LayerMask.GetMask("Player"));
            if (playerCollider != null)
            {
                threatManager.AddThreat(playerCollider.transform, 1);
                ChangeState(State.Moving);
                return;
            }

            // If no player is nearby, run the base idle logic (which handles patrolling)
            base.UpdateIdleState();
        }
        
        private enum State { Stationary, Chase, Attack }
        private State currentState;

        [Header("AI Parameters")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float aggroRadius = 2f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private int attackDamage = 2;
        [SerializeField] private float attackWindUp = 0.5f;
        [SerializeField] private float leashRadius = 40f;

        [Header("References")]
        [SerializeField] private ThreatManager threatManager;
        [SerializeField] private MeleeHitboxController meleeHitbox;


        private Transform currentTarget;
        private Animator animator;
        private Vector3 startingPosition;
        private float lastAttackTime;
        private bool isAttacking = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            if (threatManager == null) threatManager = GetComponent<ThreatManager>();
        }

        private void Start()
        {
            startingPosition = transform.position;
            currentState = State.Stationary;
            if (animator != null) animator.SetBool("isStationary", true);
        }

        private void Update()
        {
            currentTarget = threatManager.CurrentHighestThreatTarget;

            if (currentTarget == null && currentState != State.Stationary)
            {
                ChangeState(State.Stationary);
            }

            switch (currentState)
            {
                case State.Stationary: UpdateStationaryState(); break;
                case State.Chase: UpdateChaseState(); break;
                case State.Attack: UpdateAttackState(); break;
            }
        }

        public void OnAttacked(Transform attacker)
        {
            if (currentState == State.Stationary)
            {
                Debug.Log($"{gameObject.name} was attacked from range! Waking up.");
                ChangeState(State.Chase);
            }
        }

        private void UpdateStationaryState()
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, aggroRadius, LayerMask.GetMask("Player"));

            if (playerCollider != null)
            {
                Debug.Log($"Player entered aggro radius!");
                threatManager.AddThreat(playerCollider.transform, 1);
                ChangeState(State.Chase);
            }
        }

        private void UpdateChaseState()
        {
            float distanceToPlayer = Vector2.Distance(transform.position, currentTarget.position);
            float distanceToStartingPosition = Vector2.Distance(transform.position, startingPosition);

            if (distanceToPlayer < (attackRange - 0.2f))
            {
                ChangeState(State.Attack);
            }
            else if (distanceToStartingPosition > leashRadius)
            {
                ChangeState(State.Stationary);
            }
            else
            {
                Vector2 direction = (currentTarget.position - transform.position).normalized;
                animator.SetFloat("moveX", direction.x);
                animator.SetFloat("moveY", direction.y);
                animator.SetFloat("lastMoveX", direction.x);
                animator.SetFloat("lastMoveY", direction.y);

                transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, moveSpeed * Time.deltaTime);
            }
        }

        private void UpdateAttackState()
        {
            if (currentTarget == null) return;

            if (Vector2.Distance(transform.position, currentTarget.position) > attackRange)
            {
                if (!isAttacking)
                {
                    ChangeState(State.Chase);
                }
                return;
            }

            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
            {
                StartCoroutine(AttackSequence());
            }

            Vector2 direction = (currentTarget.position - transform.position).normalized;
        }

        private IEnumerator AttackSequence()
        {
            isAttacking = true;
            lastAttackTime = Time.time;

            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(attackWindUp);

            if (meleeHitbox != null && currentTarget != null)
            {
                Vector2 direction = (currentTarget.position - transform.position).normalized;
                meleeHitbox.PerformAttack(attackDamage, direction, attackRange, transform);
            }
            yield return new WaitForSeconds(0.5f);
            isAttacking = false;
        }

        private void ChangeState(State newState)
        {
            currentState = newState;

            animator.SetBool("isStationary", newState == State.Stationary);
            animator.SetBool("isChasing", newState == State.Chase);

            if (newState != State.Chase)
            {
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", 0);
            }

            if (newState == State.Stationary)
            {
                StartCoroutine(ReturnToStart());
            }
        }

        private IEnumerator ReturnToStart()
        {
            while (Vector2.Distance(transform.position, startingPosition) > 0.1f)
            {
                Vector2 direction = (startingPosition - transform.position).normalized;
                animator.SetFloat("moveX", direction.x);
                animator.SetFloat("moveY", direction.y);

                transform.position = Vector2.MoveTowards(transform.position, startingPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = startingPosition;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aggroRadius);
        }
    }
}*/