using UnityEngine;
using System.Collections.Generic;
using InventoryNamespace;
using CharacterNamespace;
using UnityEngine.SocialPlatforms;

namespace EnemyNamespace
{
    [System.Serializable]
    public class LootDrop
    {
        public ItemData item;
        [Range(0f, 1f)]
        public float dropChance;
    }

    public class EnemyLoot : MonoBehaviour
    {
        [Header("Loot Settings")]
        [SerializeField] private GameObject genericPickupPrefab;
        [SerializeField] private List<LootDrop> lootTable;

        public void DropLoot()
        {
            foreach (var drop in lootTable)
            {
                if (Random.value < drop.dropChance)
                {
                    if (drop.item != null && genericPickupPrefab != null)
                    {
                        GameObject pickupInstance = Instantiate(genericPickupPrefab, transform.position, Quaternion.identity);
                        if (pickupInstance.TryGetComponent<ItemPickup>(out var itemPickup))
                        {
                            itemPickup.Initialize(drop.item, 1);
                            Debug.Log($"<color=green>{gameObject.name} dropped {drop.item.itemName}.</color");
                        }
                    }
                }
            }
        }
    }
}
