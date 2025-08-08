using System.Collections.Generic;
using UnityEngine;
using InventoryNamespace;
using Unity.Collections;

namespace CharacterNamespace
{
    public class PlayerStats : CharacterStats
    {
        [Header("Player Class")]
        public PlayerClass playerClass;

        [Header("Component References")]
        [SerializeField] private PlayerEquipment playerEquipment;

        [Header("Base Stats")]
        public int baseHealth = 100;
        public int baseMana = 50;
        public int baseStamina = 50;
        public int baseStrength = 5;
        public int baseIntellect = 5;
        public int baseAgility = 5;
        public int baseArmor = 0;
        public int baseMagicResistance = 0;
        public int baseMinPhysicalDamage = 1;
        public int baseMaxPhysicalDamage = 2;
        public int baseMinMagicDamage = 0;
        public int baseMaxMagicDamage = 0;
        public float baseCritChance = 5.0f;
        public int baseArmorPenetration = 0;
        public int baseMagicPenetration = 0;
        public float baseAttackSpeed = 1f;
        public float baseMovementSpeed = 4.0f;

        [Header("Final Calculated Stats")]
        [ReadOnly] public int MaxHealth;
        [ReadOnly] public int MaxMana;
        [ReadOnly] public int MaxStamina;
        [ReadOnly] public int Strength;
        [ReadOnly] public int Intellect;
        [ReadOnly] public int Agility;
        [ReadOnly] public int Armor;
        [ReadOnly] public int MagicResistance;
        [ReadOnly] public int MinPhysicalDamage;
        [ReadOnly] public int MaxPhysicalDamage;
        [ReadOnly] public int MinMagicDamage;
        [ReadOnly] public int MaxMagicDamage;
        [ReadOnly] public float CritChance;
        [ReadOnly] public int ArmorPenetration;
        [ReadOnly] public int MagicPenetration;
        [ReadOnly] public float AttackSpeed;

        private void Awake()
        {
            if (playerEquipment == null) playerEquipment = GetComponent<PlayerEquipment>();
            RecalculateStats();
        }

        public void RecalculateStats()
        {
            MaxHealth = baseHealth;
            MaxMana = baseMana;
            MaxStamina = baseStamina;
            Strength = baseStrength;
            Intellect = baseIntellect;
            Agility = baseAgility;
            Armor = baseArmor;
            MagicResistance = baseMagicResistance;
            MinPhysicalDamage = baseMinPhysicalDamage;
            MaxPhysicalDamage = baseMaxPhysicalDamage;
            MinMagicDamage = baseMinMagicDamage;
            MaxMagicDamage = baseMaxMagicDamage;
            CritChance = baseCritChance;
            ArmorPenetration = baseArmorPenetration;
            MagicPenetration = baseMagicPenetration;
            AttackSpeed = baseAttackSpeed;
            MovementSpeed = baseMovementSpeed;

            if (playerEquipment == null || playerEquipment.equippedItems == null) return;

            foreach (InventorySlot slot in playerEquipment.equippedItems.Values)
            {
                if (slot == null || slot.IsEmpty()) continue;

                if (slot.itemData is EquipmentData equipment)
                {
                    Strength += equipment.strength;
                    Intellect += equipment.intellect;
                    Agility += equipment.agility;

                    if (equipment is WeaponData weapon)
                    {
                        AttackSpeed = weapon.attackSpeed; 
                        MinPhysicalDamage += weapon.minPhysicalDamage;
                        MaxPhysicalDamage += weapon.maxPhysicalDamage;
                        MinMagicDamage += weapon.minMagicDamage;
                        MaxMagicDamage += weapon.maxMagicDamage;
                        CritChance += weapon.critHitChance;
                        ArmorPenetration += weapon.armorPenetration;
                        MagicPenetration += weapon.magicPenetration;
                    }
                    else if (equipment is ArmorData armorPiece)
                    {
                        Armor += armorPiece.armor;
                        MaxHealth += armorPiece.health;
                        MaxMana += armorPiece.mana;
                        MagicResistance += armorPiece.magicResistance;
                    }
                    else if (equipment is QuiverData quiver)
                    {
                        MinPhysicalDamage += quiver.minPhysicalDamage;
                        MaxPhysicalDamage += quiver.maxPhysicalDamage;
                        CritChance += quiver.critHitChance;
                        Armor += quiver.armor;
                        MagicResistance += quiver.magicResistance;
                        ArmorPenetration += quiver.armorPenetration;
                        MagicPenetration += quiver.magicPenetration;
                    }
                    else if (equipment is ShieldData shield)
                    {
                        Armor += shield.armor;
                        MaxHealth += shield.health;
                        MaxMana += shield.mana;
                        MagicResistance += shield.magicResistance;
                    }
                    else if (equipment is JewelryData jewelry)
                    {
                        MaxHealth += jewelry.health;
                        MaxMana += jewelry.mana;
                        MovementSpeed += jewelry.movementSpeed;
                        Armor += jewelry.armor;
                        MagicResistance += jewelry.magicResistance;
                        MinPhysicalDamage += jewelry.minPhysicalDamage;
                        MaxPhysicalDamage += jewelry.maxPhysicalDamage;
                        MinMagicDamage += jewelry.minMagicDamage;
                        MaxMagicDamage += jewelry.maxMagicDamage;
                        CritChance += jewelry.critChance;
                        ArmorPenetration += jewelry.armorPenetration;
                        MagicPenetration += jewelry.magicPenetration;
                        AttackSpeed -= (AttackSpeed * (jewelry.increasedAttackSpeed / 100f)); // Correctly apply percentage
                    }
                }
            }
            Debug.Log($"Stats recalculated for {playerClass}.");
        }
    }
}