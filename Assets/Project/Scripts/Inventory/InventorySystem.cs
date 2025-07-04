using System.Collections.Generic;
using UnityEngine;

namespace InventoryNamespace
{
    public class InventorySystem : MonoBehaviour
    {
        public InventorySlot[] slots;
        public int inventorySlots;

        private void Awake()
        {
            slots = new InventorySlot[inventorySlots];
        }

        public int? FindFirstEmptySlotIndex()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null || slots[i].IsEmpty())
                {
                    return i;
                }
            }
            return null;
        }

        public int AddItem(ItemData itemData, int quantity)
        {
            if (quantity <= 0 || itemData == null) return quantity;

            int remainingQuantity = quantity;

            if (itemData.isStackable)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i] != null && !slots[i].IsEmpty() && slots[i].itemData == itemData)
                    {
                        int spaceInStack = itemData.maxStackSize - slots[i].quantity;
                        if (spaceInStack > 0)
                        {
                            int quantityToAdd = Mathf.Min(remainingQuantity, spaceInStack);
                            slots[i].quantity += quantityToAdd;
                            remainingQuantity -= quantityToAdd;

                            if (remainingQuantity <= 0) return 0;
                        }
                    }
                }
            }

            if (remainingQuantity > 0)
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i] == null || slots[i].IsEmpty())
                    {
                        int quantityToAdd = Mathf.Min(remainingQuantity, itemData.isStackable ? itemData.maxStackSize : 1);
                        slots[i] = new InventorySlot(itemData, quantityToAdd);
                        remainingQuantity -= quantityToAdd;

                        if (remainingQuantity <= 0) return 0;
                        if (!itemData.isStackable) continue;
                    }
                }
            }
            return remainingQuantity;
        }

        public void RemoveItemFromSlot(int slotIndex, int quantity)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length || slots[slotIndex] == null || slots[slotIndex].IsEmpty())
                return;

            slots[slotIndex].quantity -= quantity;

            if (slots[slotIndex].quantity <= 0)
            {
                slots[slotIndex] = null;
            }
        }

        public void SwapSlots(int indexA, int indexB)
        {
            if (indexA < 0 || indexA >= slots.Length || indexB < 0 || indexB >= slots.Length || indexA == indexB)
                return;

            InventorySlot tempslot = slots[indexA];
            slots[indexA] = slots[indexB];
            slots[indexB] = tempslot;
        }

        public bool IsFull()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null || slots[i].IsEmpty())
                {
                    return false;
                }
            }
            return true;
        }

        public void ClearInventory()
        {
            slots = new InventorySlot[inventorySlots];
        }
    }
}