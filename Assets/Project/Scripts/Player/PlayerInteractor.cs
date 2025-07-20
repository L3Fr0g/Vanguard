using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using System.Collections;
using Unity.VisualScripting;

namespace CharacterNamespace
{
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 1.5f;
        [SerializeField] private LayerMask interactableLayer;

        private PlayerControls playerControls;
        private IInteractable currentInteractable;
        private Coroutine interactionCoroutine;
        private bool isInteracting;

        private void Awake()
        {
            playerControls = new PlayerControls();
            playerControls.Actions.Interact.started += _ => StartInteraction();
            playerControls.Actions.Interact.canceled += _ => CancelInteraction();
        }

        private void OnEnable() => playerControls.Enable();
        private void OnDisable() => playerControls.Disable();

        private void Update()
        {
            if (isInteracting) return;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactableLayer);
            currentInteractable = FindClosestInteractable(colliders);
        }

        private void StartInteraction()
        {
            if (currentInteractable != null)
            {
                interactionCoroutine = StartCoroutine(InteractionProcess(currentInteractable));
            }
        }

        private void CancelInteraction()
        {
            if (currentInteractable != null)
            {
                StopCoroutine(interactionCoroutine);
                interactionCoroutine = null;
                Debug.Log("Interaction Cancelled");
            }
        }

        private IEnumerator InteractionProcess(IInteractable interactable)
        {
            float timer = 0f;
            float duration = interactable.InteractionDuration;

            Debug.Log($"Starting interaction with '{interactable.InteractionPrompt}' which has a duration of {duration} seconds.");

            if (duration <= 0f)
            {
                interactable.Interact(this);
                yield break;
            }

            while (timer < duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            interactable.Interact(this);
            interactionCoroutine = null;
        }

        private IInteractable FindClosestInteractable(Collider2D[] colliders)
        {
            IInteractable closest = null;
            float minDistance = float.MaxValue;
            foreach (var col in colliders)
            {
                if (col.TryGetComponent<IInteractable>(out var interactable))
                {
                    float dist = Vector2.Distance(transform.position, col.transform.position);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closest = interactable;
                    }
                }
            }
            return closest;
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
