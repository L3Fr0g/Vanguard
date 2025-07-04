using UnityEngine;
using System.Collections.Generic;
using CharacterNamespace;

namespace InventoryNamespace
{
    public class InventoryPanelManager : MonoBehaviour
    {
        public static InventoryPanelManager Instance { get; private set; }

        [Header("UI Setup")]
        [SerializeField] private GameObject itemSlotUIPrefab;
        [SerializeField] private Transform itemContainer;

        [Header("System References")]
        [SerializeField] private PlayerEquipment playerEquipment;

        private List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        private void Start()
        {
            if (playerEquipment == null) playerEquipment = FindFirstObjectByType<PlayerEquipment>();
            InitializeInventoryUI();
        }

        void InitializeInventoryUI()
        {
            foreach (Transform child in itemContainer) Destroy(child.gameObject);
            uiSlots.Clear();

            int capacity = playerEquipment.playerInventorySystem.inventorySystem.inventorySlots;
            for (int i = 0; i < capacity; i++)
            {
                GameObject slotGO = Instantiate(itemSlotUIPrefab, itemContainer);
                InventorySlotUI slotUI = slotGO.GetComponent<InventorySlotUI>();
                if (slotUI != null) uiSlots.Add(slotUI);
            }
            RefreshInventoryUI();
        }

        public void RefreshInventoryUI()
        {
            if (playerEquipment == null || playerEquipment.playerInventorySystem == null || uiSlots.Count == 0) return;
            if (uiSlots.Count != playerEquipment.playerInventorySystem.inventorySystem.inventorySlots)
            {
                InitializeInventoryUI();
                return;
            }

            InventorySystem currentInventory = playerEquipment.playerInventorySystem.inventorySystem;
            for (int i = 0; i < uiSlots.Count; i++)
            {
                uiSlots[i].Initialize(currentInventory.slots[i], i, playerEquipment);
            }
        }
    }
}