using CharacterNamespace;
using System.Collections.Generic;
using UnityEngine;

namespace InventoryNamespace
{
    public class ItemPickup : MonoBehaviour, IInteractable
    {
        [Header("Item Data")]
        public ItemData itemData;
        public int quantity = 1;
        public string InteractionPrompt => $"Pick up {itemData.itemName}";
        public float InteractionDuration => 0f;
        public InteractionType Type => InteractionType.Instant;

        [Header("Visuals")]
        [SerializeField] private GameObject visualsParent;

        [Header("Rarity Colors")]
        [SerializeField] private Color junkColor = Color.gray;
        [SerializeField] private Color commonColor = Color.white;
        [SerializeField] private Color uncommonColor = new Color(0.1f, 1f, 0.1f);
        [SerializeField] private Color rareColor = new Color(0.2f, 0.5f, 1f);
        [SerializeField] private Color uniqueColor = new Color(0.6f, 0.2f, 1f);
        [SerializeField] private Color legendaryColor = new Color(1f, 0.8f, 0.2f);
        
        private Dictionary<ItemRarity, Color> rarityColors;
        private SpriteRenderer spriteRenderer;
        private ParticleSystem particleSystem;

        private void Awake()
        {
            rarityColors = new Dictionary<ItemRarity, Color>
            {
                { ItemRarity.Junk, junkColor },
                { ItemRarity.Common, commonColor },
                { ItemRarity.Uncommon, uncommonColor },
                { ItemRarity.Rare, rareColor },
                { ItemRarity.Unique, uniqueColor },
                { ItemRarity.Legendary, legendaryColor }
            };

            if (visualsParent != null)
            {
                spriteRenderer = visualsParent.GetComponentInChildren<SpriteRenderer>();
                particleSystem = visualsParent.GetComponentInChildren<ParticleSystem>();
            }
        }

        public void Initialize(ItemData data, int qty)
        {
            itemData = data;
            quantity = qty;

            if (itemData == null)
            {
                Debug.LogError("ItemPickup was initialized with null ItemData!", this);
                Destroy(gameObject);
                return;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = itemData.icon;
            }

            if (particleSystem != null)
            {
                var main = particleSystem.main;
                Color currentColor = rarityColors.ContainsKey(itemData.rarity) ? rarityColors[itemData.rarity] : commonColor;
                main.startColor = new ParticleSystem.MinMaxGradient(currentColor);
                particleSystem.Play();
            }
        }

        public bool Interact(PlayerInteractor interactor)
        {
            PlayerInventorySystem playerInventory = interactor.GetComponent<PlayerInventorySystem>();
            if (playerInventory != null)
            {
                int remainingQuantity = playerInventory.PickUpItem(itemData, quantity);
                if (remainingQuantity < quantity)
                {
                    quantity = remainingQuantity;
                    InventoryPanelManager.Instance.RefreshInventoryUI();
                    if (quantity <= 0) Destroy(gameObject); 
                    return true;
                }
            }
            return false;
        }
    }
}