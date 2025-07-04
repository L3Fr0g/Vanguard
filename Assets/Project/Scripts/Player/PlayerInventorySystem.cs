using UnityEngine;

namespace InventoryNamespace
{
    public class PlayerInventorySystem : MonoBehaviour
    {
        public InventorySystem inventorySystem;

        public int PickUpItem(ItemData itemData, int quantity)
        {
            if (!inventorySystem.IsFull() || itemData.isStackable)
            {
                return inventorySystem.AddItem(itemData, quantity);
            }
            return quantity;
        }
    }
}