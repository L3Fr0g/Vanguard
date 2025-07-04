using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace InventoryNamespace
{
    public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI References")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemQtyText;
        [SerializeField] private GameObject filledSlotVisuals;

        public int SlotIndex { get; private set; }
        private PlayerEquipment playerEquipment;
        private ItemData currentItemData;

        private bool HasItem => currentItemData != null;

        public void Initialize(InventorySlot slotData, int slotIndex, PlayerEquipment playerEquip)
        {
            SlotIndex = slotIndex;
            playerEquipment = playerEquip;
            currentItemData = slotData?.itemData;

            bool hasItem = (slotData != null && !slotData.IsEmpty());

            filledSlotVisuals.SetActive(hasItem);
            itemIcon.gameObject.SetActive(hasItem);

            if (hasItem)
            {
                itemIcon.sprite = slotData.itemData.icon;
                itemQtyText.text = slotData.quantity > 1 ? slotData.quantity.ToString() : "";
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (HasItem) TooltipManager.Instance.ShowTooltip(currentItemData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipManager.Instance.HideTooltip();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (HasItem && playerEquipment != null)
                {
                    playerEquipment.TryEquipItem(SlotIndex);
                }
            }
        }
    }
}