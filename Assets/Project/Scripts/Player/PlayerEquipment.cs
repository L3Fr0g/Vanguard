using System.Collections.Generic;
using UnityEngine;
using CharacterNamespace;
using AnimationNamespace;
using System.Linq;

namespace InventoryNamespace
{
    public class PlayerEquipment : MonoBehaviour
    {
        public enum EquipmentSlot { Head, Chest, Legs, Feet, MainHand, OffHand, Necklace, Ring, Trinket, Bag }

        [Header("Equipment Slots")]
        [SerializeField] private Transform mainHandSlot;
        [SerializeField] private Transform offHandSlot;

        [Header("Component References")]
        [SerializeField] public PlayerInventorySystem playerInventorySystem;
        private PlayerStats playerStats;
        private AnimationController animationController;
        private PlayerAbilityManager playerAbilityManager;

        public Dictionary<EquipmentSlot, InventorySlot> equippedItems = new Dictionary<EquipmentSlot, InventorySlot>();

        private readonly Dictionary<PlayerClass, ArmorType[]> classArmorRestrictions = new Dictionary<PlayerClass, ArmorType[]>()
        {
            { PlayerClass.Knight, new[] { ArmorType.Light, ArmorType.Medium, ArmorType.Heavy } },
            { PlayerClass.Berserker, new[] {ArmorType.Light, ArmorType.Medium, ArmorType.Heavy } },
            { PlayerClass.Archer, new[] { ArmorType.Light, ArmorType.Medium } },
            { PlayerClass.Lancer, new[] { ArmorType.Light, ArmorType.Medium } },
            { PlayerClass.Rogue, new[] { ArmorType.Light, ArmorType.Medium } },
            { PlayerClass.Monk, new[] { ArmorType.Light, ArmorType.Medium } },
            { PlayerClass.Cleric, new[] { ArmorType.Light, ArmorType.Medium } },
            { PlayerClass.Mage, new[] { ArmorType.Light } },
            { PlayerClass.Summoner, new[] {ArmorType.Light} }
        };

        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
            animationController = GetComponent<AnimationController>();
            playerAbilityManager = GetComponent<PlayerAbilityManager>();

            foreach (EquipmentSlot slot in System.Enum.GetValues(typeof(EquipmentSlot)))
            {
                equippedItems.Add(slot, null);
            }
        }

        public void TryEquipItem(int sourceInventoryIndex)
        {
            if (sourceInventoryIndex < 0 || sourceInventoryIndex >= playerInventorySystem.inventorySystem.slots.Length) return;
            ItemData itemToEquip = playerInventorySystem.inventorySystem.slots[sourceInventoryIndex]?.itemData;
            if (itemToEquip == null) return;

            EquipmentSlot? targetSlot = GetSlotForItem(itemToEquip);
            if (!targetSlot.HasValue)
            {
                Debug.LogWarning($"No compatible equipment slot found for {itemToEquip.name}.");
                return;
            }

            EquipItem(itemToEquip, targetSlot.Value, sourceInventoryIndex);
        }

        private void EquipItem(ItemData itemToEquip, EquipmentSlot targetSlot, int sourceInventoryIndex)
        {
            if (itemToEquip is EquipmentData equipmentToEquip)
            {
                if (equipmentToEquip.equippableBy.Length > 0 && !equipmentToEquip.equippableBy.Contains(playerStats.playerClass))
                {
                    Debug.LogWarning($"Cannot equip {itemToEquip.name}. Class '{playerStats.playerClass}' is not allowed.");
                    return;
                }


                if (equipmentToEquip is ArmorData armor && classArmorRestrictions.ContainsKey(playerStats.playerClass))
                {
                    if (!classArmorRestrictions[playerStats.playerClass].Contains(armor.armorType))
                    {
                        Debug.LogWarning($"Cannot equip {armor.name}. Class '{playerStats.playerClass}' cannot wear {armor.armorType} armor.");
                        return;
                    }
                }
            }

            InventorySlot currentlyEquippedItem = equippedItems[targetSlot];

            HandleUnequipEffects(currentlyEquippedItem?.itemData);

            equippedItems[targetSlot] = playerInventorySystem.inventorySystem.slots[sourceInventoryIndex];
            playerInventorySystem.inventorySystem.slots[sourceInventoryIndex] = currentlyEquippedItem;

            HandleEquipEffects(itemToEquip);

            playerStats.RecalculateStats();

            InventoryPanelManager.Instance?.RefreshInventoryUI();
            EquipmentPanelManager.Instance?.RefreshAllSlots();

            Debug.Log($"Equipped {itemToEquip.name}");
        }

        public void TryUnequipItem(EquipmentSlot slotToUnequip)
        {
            int? emptySlot = playerInventorySystem.inventorySystem.FindFirstEmptySlotIndex();
            if (emptySlot.HasValue)
            {
                UnequipItem(slotToUnequip, emptySlot.Value);
            }
            else
            {
                Debug.LogWarning("Cannot unequip, inventory is full");
            }
        }

        private void UnequipItem(EquipmentSlot slotToUnequip, int targetInventoryIndex)
        {
            if (!equippedItems.ContainsKey(slotToUnequip) || equippedItems[slotToUnequip] == null || equippedItems[slotToUnequip].IsEmpty()) return;

            InventorySlot itemToUnequip = equippedItems[slotToUnequip];

            playerInventorySystem.inventorySystem.slots[targetInventoryIndex] = itemToUnequip;
            equippedItems[slotToUnequip] = null;

            HandleUnequipEffects(itemToUnequip.itemData);
            playerStats.RecalculateStats();

            InventoryPanelManager.Instance?.RefreshInventoryUI();
            EquipmentPanelManager.Instance?.RefreshAllSlots();

            Debug.Log($"Unequipped {itemToUnequip.itemData.name}.");
        }

        public bool IsWeaponEquipped()
        {
            return equippedItems.TryGetValue(EquipmentSlot.MainHand, out var slot) && slot != null && !slot.IsEmpty();
        }

        public GameObject GetEquippedProjectilePrefab()
        {
            if (equippedItems.TryGetValue(EquipmentSlot.OffHand, out InventorySlot offHandSlot) &&
                offHandSlot != null && !offHandSlot.IsEmpty() &&
                offHandSlot.itemData is QuiverData quiver)
            {
                return quiver.projectilePrefab;
            }
            return null;
        }

        public MeleeHitboxController GetEquippedMeleeHitbox()
        {
            if (mainHandSlot != null && mainHandSlot.childCount > 0)
            {
                Transform weapoVisual = mainHandSlot.GetChild(0);
                return weapoVisual.GetComponentInChildren<MeleeHitboxController>();
            }
            return null;
        }

        private void HandleEquipEffects(ItemData item) { if (item is WeaponData weapon) { animationController.SetWeaponType(weapon); SpawnWeaponVisual(weapon); } }
        private void HandleUnequipEffects(ItemData item) { if (item is WeaponData) { animationController.SetWeaponType(null); ClearWeaponVisual(); } }
        private void SpawnWeaponVisual(WeaponData weapon) { ClearWeaponVisual(); if (weapon.equippedPrefab != null && mainHandSlot != null) { GameObject weaponGO = Instantiate(weapon.equippedPrefab, mainHandSlot); animationController.SetWeaponAnimator(weaponGO.GetComponent<Animator>()); } }
        private void ClearWeaponVisual() { if (mainHandSlot != null) { foreach (Transform child in mainHandSlot) Destroy(child.gameObject); } animationController.ClearWeaponAnimator(); }

        private EquipmentSlot? GetSlotForItem(ItemData item)
        {
            if (item is BowData || item is SwordData || item is AxeData || item is PolearmData || item is DaggerData || item is MaceData || item is FistWeaponData) return EquipmentSlot.MainHand;
            if (item is ShieldData || item is QuiverData) return EquipmentSlot.OffHand;
            if (item is ArmorData armor)
            {
                switch (armor.slot)
                {
                    case ArmorSlot.Head: return EquipmentSlot.Head;
                    case ArmorSlot.Chest: return EquipmentSlot.Chest;
                    case ArmorSlot.Legs: return EquipmentSlot.Legs;
                    case ArmorSlot.Feet: return EquipmentSlot.Feet;
                }
            }
            if (item is JewelryData jewelry)
            {
                switch (jewelry.slot)
                {
                    case JewelrySlot.Necklace: return EquipmentSlot.Necklace;
                    case JewelrySlot.Ring: return EquipmentSlot.Ring;
                    case JewelrySlot.Trinket: return EquipmentSlot.Trinket;
                }
            }
            if (item is BagData) return EquipmentSlot.Bag;

            return null;
        }
    }
}