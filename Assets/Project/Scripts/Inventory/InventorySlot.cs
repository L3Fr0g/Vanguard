using UnityEngine;

namespace InventoryNamespace
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemData itemData;
        public int quantity;

        public InventorySlot(ItemData newItemData, int newQuantity)
        {
            itemData = newItemData;
            quantity = newQuantity;
        }

        public bool IsEmpty()
        {
            return itemData == null || quantity <= 0;
        }

        public void ClearSlot()
        {
            itemData = null;
            quantity = 0;
        }

        public void SetItem(ItemData newItemData, int newQuantity)
        {
            itemData = newItemData;
            quantity = newQuantity;
        }
    }

}