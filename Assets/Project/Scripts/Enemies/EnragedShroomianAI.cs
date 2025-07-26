using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.XR;

public class EnragedShroomianAI : MonoBehaviour
{
    private enum State { Planted, Chase, Attack }
    private State currentState;

    [Header("AI Parameters")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float aggroRadius = 2f;
    [SerializeField] private float attackRange = 1.2f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float leashRadius = 15f;

    [Header("References")]
    [SerializeField] private LayerMask playerLayer;

    private Transform player;
    private Animator animator;
    private Vector3 startingPosition;
    private float lastAttackTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        startingPosition = transform.position;
        currentState = State.Planted;
        if (animator != null) animator.SetBool("isPlanted", true);
    }

    private void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case State.Planted:
                UpdatePlantedState();
                break;
            case State.Chase:
                UpdateChaseState();
                break;
            case State.Attack:
                UpdateAttackState();
                break;
        }
    }

    private void UpdatePlantedState()
    {
        if (Vector2.Distance(transform.position, player.position) < aggroRadius)
        {
            ChangeState(State.Chase);
        }
    }

    private void UpdateChaseState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            ChangeState(State.Attack);
        }
        else if (distanceToPlayer > leashRadius)
        {
            ChangeState(State.Planted);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    private void UpdateAttackState()
    {
        if (Vector2.Distance(transform.position, player.position) > attackRange)
        {
            ChangeState(State.Chase);
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
        }
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
        
        animator.SetBool("isPlanted", newState == State.Planted);
        animator.SetBool("isChasing", newState == State.Chase);

        if (newState == State.Planted)
        {
            StartCoroutine(ReturnToStart());
        }
    }

    private IEnumerator ReturnToStart()
    {
        while (Vector2.Distance(transform.position, startingPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, startingPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = startingPosition;
    }
}
