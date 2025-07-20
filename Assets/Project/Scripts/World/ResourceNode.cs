using UnityEngine;
using System.Collections.Generic;
using InventoryNamespace;

namespace CharacterNamespace
{
    public class ResourceNode : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] private string prompt = "Gather";
        [SerializeField] private float interactionDuration = 2.0f;
        [Tooltip("The specific spot the player should move to when interacting.")]
        [SerializeField] private Transform interactionPoint;

        [Header("Loot")]
        [Tooltip("The prefab for the item pickup that will be spawned.")]
        [SerializeField] private GameObject genericPickupPrefab;
        [SerializeField] private List<ItemData> possibleLoot;

        private bool isDepleted = false;

        private void Start()
        {
            if (genericPickupPrefab == null)
            {
                Debug.LogError($"ResourceNode'{gameObject.name}' is missing a reference to the Generic Prefab!", this);
            }
        }

        public string InteractionPrompt => prompt;
        public float InteractionDuration => interactionDuration;

        public bool Interact(PlayerInteractor interactor)
        {
            if (isDepleted) return false;

            Debug.Log($"Successfully gathered from {gameObject.name}!");

            if (possibleLoot.Count > 0 && genericPickupPrefab != null )
            {
                int randomIndex = Random.Range( 0, possibleLoot.Count );
                ItemData droppedItem = possibleLoot[randomIndex];

                GameObject pickupInstance = Instantiate(genericPickupPrefab, transform.position, Quaternion.identity);
                if (pickupInstance.TryGetComponent<ItemPickup>(out var itemPickup))
                {
                    itemPickup.Initialize(droppedItem, 1);
                }
            }

            isDepleted = true;

            GetComponent<Collider2D>().enabled = false;

            return true;
        }

        public Transform GetInteractionPoint()
        {
            return interactionPoint;
        }
    }
}
