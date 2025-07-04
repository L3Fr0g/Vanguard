using UnityEngine;

namespace InventoryNamespace
{
    public enum JewelrySlot { Necklace, Ring, Trinket }

    [CreateAssetMenu(fileName = "NewJewelry", menuName = "Items/Equipment/Jewelry")]
    public class JewelryData : EquipmentData
    {
        [Header("Jewelry Slot")]
        public JewelrySlot slot;

        // NOTE: As discussed, primary stats for jewelry can be added later with crafting.
        // For now, all stats are treated as secondary.
        [Header("Possible Stats")]
        public int health;
        public int mana;
        public float movementSpeed;
        public int armor;
        public int magicResistance;
        public int minPhysicalDamage;
        public int maxPhysicalDamage;
        public int minMagicDamage;
        public int maxMagicDamage;
        public float increasedAttackSpeed; // Percentage
        public float critChance;
        public int armorPenetration;
        public int magicPenetration;
    }
}