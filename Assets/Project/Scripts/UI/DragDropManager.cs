using UnityEngine;
using UnityEngine.UI;
using InventoryNamespace;

namespace InventoryNamespace
{
    public class DragDropManager : MonoBehaviour
    {
        public static DragDropManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] public Image dragIcon;

        public bool IsDragging { get; private set; }
        public InventorySlotUI SourceInventorySlot { get; set; }
        public EquipmentSlotUI SourceEquipmentSlot { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        private void Start()
        {
            HideDragIcon();
        }

        public void StartDragFromInventory(InventorySlotUI sourceSlot, Sprite iconSprite)
        {
            if (sourceSlot == null || iconSprite == null) return;
            
            IsDragging = true;
            SourceInventorySlot = sourceSlot;
            SourceEquipmentSlot = null;

            if (dragIcon != null)
            {
                dragIcon.sprite = iconSprite;
                ShowDragIcon();
            }
        }

        public void StartDragFromEquipment(EquipmentSlotUI sourceSlot, Sprite iconSprite)
        {
            if (sourceSlot == null || iconSprite == null) return;

            IsDragging = true;
            SourceEquipmentSlot = sourceSlot;
            SourceInventorySlot = null;

            if (dragIcon != null)
            {
                dragIcon.sprite = iconSprite;
                ShowDragIcon();
            }
        }

        public void StopDrag()
        {
            IsDragging = false;
            SourceInventorySlot = null;
            SourceEquipmentSlot = null;
            HideDragIcon();
        }

        private void ShowDragIcon()
        {
            if (dragIcon != null) dragIcon.enabled = true;
        }

        private void HideDragIcon()
        {
            if (dragIcon != null)
            {
                dragIcon.enabled = false;
                dragIcon.sprite = null;
            }
        }
    }
}