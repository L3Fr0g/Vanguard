using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CharacterNamespace;

namespace EnemyNamespace
{
    public class BaseMeleeAI : MonoBehaviour, IAIController
    {
        private enum State { Idle, Moving, Attacking }
        [SerializeField] private State currentState;

        [Header("AI Parameters")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float patrolSpeed = 1.5f;
        [SerializeField] private float aggroRadius = 5f;
        [SerializeField] private float leashRadius = 40f;

        [Header("Patrol Behaviour")]
        [SerializeField] private Transform patrolRoute;

        [Header("Respawn")]
        [SerializeField] private float respawnTime = 60f;

        [Header("References")]
        [SerializeField] private GameObject visualsParent;

        private List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
        private int currentPatrolIndex = 0;
        private float idleTimer = 0f;

        private Animator animator;
        private EnemyAbilityManager abilityManager;
        private EnemyHealth enemyHealth;
        private Movement movement;
        private ThreatManager threatManager;
        private Transform currentTarget;
        private Vector3 leashAnchorPoint;
        private Vector3 moveDestination;
        private bool isAttacking = false;
        private bool isDead = false;

        private void Awake()
        {
            abilityManager = GetComponent<EnemyAbilityManager>();
            animator = GetComponent<Animator>();
            enemyHealth = GetComponent<EnemyHealth>();
            threatManager = GetComponent<ThreatManager>();
            movement = GetComponent<Movement>();

            if (patrolRoute != null)
            {
                foreach (Transform child in patrolRoute)
                {
                    if (child.TryGetComponent<PatrolPoint>(out var point))
                    {
                        patrolPoints.Add(point);
                    }
                }
            }
        }

        private void Start()
        {
            leashAnchorPoint = transform.position;
            ChangeState(State.Idle);
        }

        void Update()
        {
            if (isDead) return;
            currentTarget = threatManager.CurrentHighestThreatTarget;
            switch (currentState)
            {
                case State.Idle:      UpdateIdleState();      break;
                case State.Moving:    UpdateMovingState();    break;
                case State.Attacking: UpdateAttackingState(); break;
            }
        }

        public void OnAttacked(Transform attacker)
        {
            threatManager.AddThreat(attacker, 1);
            if (currentState == State.Idle)
            {
                leashAnchorPoint = transform.position;
                ChangeState(State.Moving);
            }
        }

        public void StartRespawnCycle()
        {
            if (respawnTime > 0) StartCoroutine(HandleRespawn());
            else Destroy(gameObject, 5f);
        }

        private IEnumerator HandleRespawn()
        {
            yield return new WaitForSeconds(2f);
            if (visualsParent != null) visualsParent.SetActive(false);
            yield return new WaitForSeconds(respawnTime);
            transform.position = leashAnchorPoint;
            if (visualsParent != null) visualsParent.SetActive(true);
            enemyHealth.ResetHealth();
            threatManager.ClearThreat();
            ChangeState(State.Idle);
        }

        private void UpdateIdleState()
        {
            if (currentTarget != null) { ChangeState(State.Moving); return; }
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, aggroRadius, LayerMask.GetMask("Player"));
            if (playerCollider != null)
            {
                threatManager.AddThreat(playerCollider.transform, 1);
                leashAnchorPoint = transform.position;
                ChangeState(State.Moving);
                return;
            }

            if (patrolPoints.Count > 0)
            {
                idleTimer += Time.deltaTime;
                float currentWaitTime = patrolPoints[currentPatrolIndex].waitTime;
                if(idleTimer >= currentWaitTime)
                {
                    currentPatrolIndex = (currentPatrolIndex +1) % patrolPoints.Count;
                    moveDestination = patrolPoints[currentPatrolIndex].transform.position;
                    ChangeState(State.Moving);
                }
            }
        }

        private void UpdateMovingState()
        {
            if (currentTarget != null)
            {
                moveDestination = currentTarget.position;
                EnemyAbility primaryAttack = abilityManager.GetPrimaryAbility();
                if (Vector2.Distance(transform.position, leashAnchorPoint) > leashRadius)
                {
                    threatManager.ClearThreat();
                    currentTarget = null;
                    moveDestination = leashAnchorPoint;
                    return;
                }
                else if (Vector2.Distance(transform.position, currentTarget.position) < (primaryAttack.range - 0.2f))
                {
                    ChangeState(State.Attacking);
                    return;
                }
            }

            if (Vector2.Distance(transform.position, moveDestination) > 0.1f)
            {
                Vector2 direction = (moveDestination - transform.position).normalized;
                animator.SetBool("isMoving", true);
                animator.SetFloat("moveX", direction.x);
                animator.SetFloat("moveY", direction.y);
                animator.SetFloat("lastMoveX", direction.x);
                animator.SetFloat("lastMoveY", direction.y);
                movement.SetMoveDirection(direction);
            }
            else
            {
                animator.SetBool("isMoving", false);
                ChangeState(State.Idle);
            }
        }

        private void UpdateAttackingState()
        {
            if (currentTarget == null)
            {
                ChangeState(State.Idle);
                return;
            }

            EnemyAbility primaryAttack = abilityManager.GetPrimaryAbility();
            if (primaryAttack == null) return;

            Vector2 direction = (currentTarget.position - transform.position).normalized;
            animator.SetFloat("lastMoveX", direction.x);
            animator.SetFloat("lastMoveY", direction.y);

            if (Vector2.Distance(transform.position, currentTarget.position) > primaryAttack.range && !isAttacking)
            {
                ChangeState(State.Moving);
                return;
            }

            if (abilityManager.CanUseAbility(primaryAttack) && !isAttacking)
            {
                StartCoroutine(AttackSequence(primaryAttack));
            }
        }

        private IEnumerator AttackSequence(EnemyAbility ability)
        {
            isAttacking = true;
            yield return StartCoroutine(abilityManager.UseAbility(ability, currentTarget));
            isAttacking = false;
        }

        private void ChangeState(State newState)
        {
            currentState = newState;
            idleTimer = 0f;
            animator.SetBool("isMoving", newState == State.Moving);
            animator.SetBool("isIdle", newState == State.Idle);

            if (newState != State.Moving)
            {
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", 0);
            }
        }

        public void OnDeath()
        {
            isDead = true;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aggroRadius);
        }
    }
}