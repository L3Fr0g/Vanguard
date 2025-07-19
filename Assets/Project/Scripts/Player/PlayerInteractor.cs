using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace CharacterNamespace
{
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 1.5f;
        [SerializeField] private LayerMask interactableLayer;

        private IInteractable currentInteractable;
        private Collider2D[] colliders = new Collider2D[10];

        private ContactFilter2D contactFilter;

        private void Start()
        {
            contactFilter.SetLayerMask(interactableLayer);
            contactFilter.useTriggers = true;
        }

        private void Update()
        {
            int count = Physics2D.OverlapCircle(transform.position, interactionRadius, contactFilter, colliders);

            IInteractable closestInteractable = null;
            float  closestDistance = float.MaxValue;

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    if (colliders[i].TryGetComponent<IInteractable>(out var interactable))
                    {
                        float distance = Vector2.Distance(transform.position, colliders[i].transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestInteractable = interactable;
                        }
                    }
                }
            }

            currentInteractable = closestInteractable;
        }

        public void TryInteract()
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact(this);
            }
        }

        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
