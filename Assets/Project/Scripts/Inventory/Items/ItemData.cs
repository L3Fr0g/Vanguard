using UnityEngine;
using System.Collections.Generic;
using CharacterNamespace;

namespace InventoryNamespace
{
    public enum ItemRarity { Junk, Common, Uncommon, Rare, Unique, Legendary }

    public abstract class ItemData : ScriptableObject
    {
        [Header("Prefabs")]
        public GameObject pickupPrefab;
        public GameObject equippedPrefab;

        [Header("Base Item Information")]
        public string itemName;
        [TextArea(3, 5)] public string itemDescription;
        public Sprite icon;
        public ItemRarity rarity;
        public int itemLevel = 1;

        [Header("Stacking")]
        public bool isStackable = false;
        public int maxStackSize = 1;
    }
}
