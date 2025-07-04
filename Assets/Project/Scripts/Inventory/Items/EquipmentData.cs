using UnityEngine;
using CharacterNamespace;

namespace InventoryNamespace
{
    public abstract class EquipmentData : ItemData
    {
        [Header("Class & Stat Requirements")]
        [Tooltip("Which classes are allowed to equip this item.")]
        public PlayerClass[] equippableBy;

        [Header("Secondary Core Stats")]
        public int strength;
        public int intellect;
        public int agility;
    }
}