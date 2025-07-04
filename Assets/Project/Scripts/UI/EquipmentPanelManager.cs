using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace InventoryNamespace
{
    public class EquipmentPanelManager : MonoBehaviour
    {
        public static EquipmentPanelManager Instance { get; private set; }

        [Header("Systemn References")]
        [SerializeField] private PlayerEquipment playerEquipment;

        [Header("UI Slot References")]
        [SerializeField] private EquipmentSlotUI headSlot;
        [SerializeField] private EquipmentSlotUI chestSlot;
        [SerializeField] private EquipmentSlotUI legsSlot;
        [SerializeField] private EquipmentSlotUI feetSlot;
        [SerializeField] private EquipmentSlotUI mainHandSlot;
        [SerializeField] private EquipmentSlotUI offHandSlot;
        [SerializeField] private EquipmentSlotUI necklaceSlot;
        [SerializeField] private EquipmentSlotUI ringSlot;
        [SerializeField] private EquipmentSlotUI trinketSlot;
        [SerializeField] private EquipmentSlotUI bagSlot;

        private Dictionary<PlayerEquipment.EquipmentSlot, EquipmentSlotUI> equipmentSlotUIs;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            InitializeSlotDictionary();
        }

        private void Start()
        {
            if (playerEquipment == null) playerEquipment = FindFirstObjectByType<PlayerEquipment>();
            InitializeSlotDictionary();
            RefreshAllSlots();
        }

        private void InitializeSlotDictionary()
        {
            equipmentSlotUIs = new Dictionary<PlayerEquipment.EquipmentSlot, EquipmentSlotUI>
            {
                { PlayerEquipment.EquipmentSlot.Head, headSlot },
                { PlayerEquipment.EquipmentSlot.Chest, chestSlot },
                { PlayerEquipment.EquipmentSlot.Legs, legsSlot },
                { PlayerEquipment.EquipmentSlot.Feet, feetSlot },
                { PlayerEquipment.EquipmentSlot.MainHand, mainHandSlot },
                { PlayerEquipment.EquipmentSlot.OffHand, offHandSlot },
                { PlayerEquipment.EquipmentSlot.Necklace, necklaceSlot },
                { PlayerEquipment.EquipmentSlot.Ring, ringSlot },
                { PlayerEquipment.EquipmentSlot.Trinket, trinketSlot },
                { PlayerEquipment.EquipmentSlot.Bag, bagSlot }
            };
        }

        public void RefreshAllSlots()
        {
            if (playerEquipment == null) return;
            foreach (var kvp in equipmentSlotUIs)
            {
                if (kvp.Value != null && playerEquipment.equippedItems.ContainsKey(kvp.Key))
                {
                    kvp.Value.UpdateSlot(playerEquipment.equippedItems[kvp.Key]);
                }
            }
        }
    }
}