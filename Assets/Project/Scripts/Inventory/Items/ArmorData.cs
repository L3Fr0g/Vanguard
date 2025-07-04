using UnityEngine;

namespace InventoryNamespace
{
    public enum ArmorSlot { Head, Chest, Legs, Feet }
    public enum ArmorType { Light, Medium, Heavy }

    [CreateAssetMenu(fileName = "NewArmor", menuName = "Items/Armor/Generic Armor")]
    public class ArmorData : EquipmentData
    {
        [Header("Armor Properties")]
        public ArmorSlot slot;
        public ArmorType armorType;

        [Header("Primary Stat")]
        public int armor;

        [Header("Secondary Stats")]
        public int health;
        public int mana;
        public int magicResistance;
    }
}