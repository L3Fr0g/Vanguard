using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace InventoryNamespace
{
    public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Slot Configuration")]
        [SerializeField] private PlayerEquipment.EquipmentSlot equipmentSlotType;

        [Header("UI References")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private GameObject filledSlotVisuals;

        private PlayerEquipment playerEquipment;
        private ItemData currentItemData;

        private void Awake()
        {
            playerEquipment = FindFirstObjectByType<PlayerEquipment>();
        }

        public void UpdateSlot(InventorySlot slotData)
        {
            currentItemData = slotData?.itemData;
            bool hasItem = currentItemData != null;

            filledSlotVisuals.SetActive(hasItem);
            itemIcon.gameObject.SetActive(hasItem);

            if (hasItem)
            {
                itemIcon.sprite = currentItemData.icon;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (currentItemData != null) TooltipManager.Instance.ShowTooltip(currentItemData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipManager.Instance.HideTooltip();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (currentItemData != null && playerEquipment != null)
                {
                    playerEquipment.TryUnequipItem(equipmentSlotType);
                }
            }
        }
    }
}