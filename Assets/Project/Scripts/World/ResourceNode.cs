using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using InventoryNamespace;

namespace CharacterNamespace
{
    public class ResourceNode : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] private string prompt = "Gather";
        [SerializeField] private float interactionDuration = 2.0f;
        [Tooltip("The type of animation the player should use for this node.")]
        [SerializeField] private InteractionType interactionType = InteractionType.Mining;

        [Header("Respawn Settings")]
        [SerializeField] private float fadeDuration = 1.5f;
        [SerializeField] private float respawnTime = 900f;

        [Header("Visuals")]
        [SerializeField] GameObject visualsParent;
        [SerializeField] private Sprite depletedSprite;

        [Header("Loot")]
        [Tooltip("The prefab for the item pickup that will be spawned.")]
        [SerializeField] private GameObject genericPickupPrefab;
        [SerializeField] private List<ItemData> possibleLoot;

        private bool isDepleted = false;
        private SpriteRenderer spriteRenderer;
        private Collider2D nodeCollider;
        private Sprite originalSprite;

        private void Awake()
        {
            if (visualsParent != null)
            {
                spriteRenderer = visualsParent.GetComponentInChildren<SpriteRenderer>();
            }

            nodeCollider = GetComponent<Collider2D>();

            if (spriteRenderer != null)
            {
                originalSprite = spriteRenderer.sprite;
            }
        }

        private void Start()
        {
            if (genericPickupPrefab == null)
            {
                Debug.LogError($"ResourceNode'{gameObject.name}' is missing a reference to the Generic Prefab!", this);
            }
        }

        public string InteractionPrompt => prompt;
        public float InteractionDuration => interactionDuration;
        public InteractionType Type => interactionType;
        public bool Interact(PlayerInteractor interactor)
        {
            if (isDepleted) return false;
            isDepleted = true;
            nodeCollider.enabled = false;

            if (possibleLoot.Count > 0 && genericPickupPrefab != null )
            {
                ItemData droppedItem = possibleLoot[Random.Range(0, possibleLoot.Count)];
                GameObject pickupInstance = Instantiate(genericPickupPrefab, transform.position, Quaternion.identity);
                if (pickupInstance.TryGetComponent<ItemPickup>(out var itemPickup))
                {
                    itemPickup.Initialize(droppedItem, 1);
                }
            }

            StartCoroutine(HandleRespawnCycle());
            return true;
        }

        private IEnumerator HandleRespawnCycle()
        {
            if (spriteRenderer !=  null && depletedSprite !=null)
            {
                spriteRenderer.sprite = depletedSprite;
            }

            yield return new WaitForSeconds(fadeDuration);

            float timer = 0f;
            Color startColor = spriteRenderer.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
            
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                spriteRenderer.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
                yield return null;
            }

            if (visualsParent != null)
            {
                visualsParent.SetActive(false);
            }

            yield return new WaitForSeconds(respawnTime);

            if (visualsParent != null)
            {
                visualsParent.SetActive(true);
            }
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = originalSprite;
                spriteRenderer.color = startColor;
            }

            isDepleted = false;
            nodeCollider.enabled = true;

            Debug.Log($"Node {gameObject.name} has respawned.");
        }
    }
}
