using UnityEngine;
using UnityEngine.EventSystems;

namespace InventoryNamespace
{
    public class UIDropZone : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            InventorySlotUI sourceSlot= eventData.pointerDrag.GetComponent<InventorySlotUI>();

            if (sourceSlot != null )
            {
                Debug.Log($"Item from slot {sourceSlot.SlotIndex} was dropped into the world.");

                PlayerInventorySystem playerInventory = FindFirstObjectByType<PlayerInventorySystem>();
                if (playerInventory != null )
                {
                    playerInventory.DropItemFromSlot(sourceSlot.SlotIndex);
                }

                DragDropManager.Instance.StopDrag();
            }
        }
    }
}
