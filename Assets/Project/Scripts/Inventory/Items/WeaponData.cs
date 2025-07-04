using InventoryNamespace;
using UnityEngine;

namespace InventoryNamespace
{
    public abstract class WeaponData : EquipmentData
    {
        [Header("Primary Weapon Stats")]
        public float range;
        public float attackSpeed = 1.0f;
        public int minPhysicalDamage;
        public int maxPhysicalDamage;

        [Header("Secondary Weapon Stats")]
        public int minMagicDamage;
        public int maxMagicDamage;
        public float critHitChance;
        public int armorPenetration;
        public int magicPenetration;
    }
}