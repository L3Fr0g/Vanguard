using UnityEngine;
using CharacterNamespace;

namespace InventoryNamespace
{
    public class PlayerInventorySystem : MonoBehaviour
    {
        public InventorySystem inventorySystem;

        [Header("Item Dropping")]
        [SerializeField] private GameObject genericPickupPrefab;

        public int PickUpItem(ItemData itemData, int quantity)
        {
            if (!inventorySystem.IsFull() || itemData.isStackable)
            {
                return inventorySystem.AddItem(itemData, quantity);
            }
            return quantity;
        }

        public void DropItemFromSlot(int slotNumber)
        {
            if (inventorySystem == null || slotNumber < 0 || slotNumber >= inventorySystem.slots.Length) return;

            InventorySlot slotToDrop = inventorySystem.slots[slotNumber];
            if (slotToDrop == null || slotToDrop.IsEmpty()) return;

            if (genericPickupPrefab != null)
            {
                Vector3 dropPosition = transform.position + (transform.up * 1.5f);
                GameObject droppedItemGO = Instantiate(genericPickupPrefab, dropPosition, Quaternion.identity);

                if (droppedItemGO.TryGetComponent<ItemPickup>(out var pickupScript))
                {
                    pickupScript.Initialize(slotToDrop.itemData, slotToDrop.quantity);
                }

                inventorySystem.RemoveItemFromSlot(slotNumber, slotToDrop.quantity);
                InventoryPanelManager.Instance.RefreshInventoryUI();
            }
        }
    }
}