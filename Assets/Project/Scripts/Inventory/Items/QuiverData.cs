using UnityEngine;

namespace InventoryNamespace
{
    [CreateAssetMenu(fileName = "NewQuiver", menuName = "Items/Equipment/Quiver")]
    public class QuiverData : EquipmentData
    {
        [Header("Primary Stats")]
        public int minPhysicalDamage;
        public int maxPhysicalDamage;
        public GameObject projectilePrefab;

        [Header("Secondary Stats")]
        public float critHitChance;
        public int minMagicDamage;
        public int maxMagicDamage;
        public int armor;
        public int magicResistance;
        public int armorPenetration;
        public int magicPenetration;
    }
}