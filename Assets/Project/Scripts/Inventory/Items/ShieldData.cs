using UnityEngine;

namespace InventoryNamespace
{
    [CreateAssetMenu(fileName = "NewShield", menuName = "Items/Equipment/Shield")]
    public class ShieldData : EquipmentData
    {
        [Header("Primary Stat")]
        public int armor;

        [Header("Secondary Stats")]
        public int health;
        public int mana;
        public int magicResistance;
    }
}