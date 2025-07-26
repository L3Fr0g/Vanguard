using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

namespace CharacterNamespace
{
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 1.5f;
        [SerializeField] private LayerMask interactableLayer;

        [Header("Component References")]
        [SerializeField] private PlayerController playerController;

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
            IInteractable newInteractable = FindClosestInteractable(colliders);

            if (newInteractable != currentInteractable)
            {
                currentInteractable = newInteractable;

                if (currentInteractable != null)
                {
                    InteractionUI.Instance.Show(currentInteractable);
                }
                else
                {
                    InteractionUI.Instance.Hide();
                }
            }
        }

        private void StartInteraction()
        {
            if (currentInteractable != null && !isInteracting)
            {
                interactionCoroutine = StartCoroutine(InteractionProcess(currentInteractable));
            }
        }

        private void CancelInteraction()
        {
            if (isInteracting && interactionCoroutine != null)
            {
                StopCoroutine(interactionCoroutine);
                interactionCoroutine = null;
                isInteracting = false;
                playerController.EndInteraction();
                InteractionUI.Instance.UpdateProgress(0);

                Debug.Log("Interaction Canceled (button released).");
            }
        }

        private IEnumerator InteractionProcess(IInteractable interactable)
        {
            isInteracting = true;
            playerController.StartInteractionAnimation(interactable.Type);
            float timer = 0f;
            float duration = interactable.InteractionDuration;

            if (duration <= 0f)
            {
                interactable.Interact(this);
                isInteracting = false;
                playerController.EndInteraction();
                yield break;
            }

            while (timer < duration)
            {
                InteractionUI.Instance.UpdateProgress(timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            interactable.Interact(this);

            InteractionUI.Instance.Hide();
            isInteracting = false;
            playerController.EndInteraction();
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
