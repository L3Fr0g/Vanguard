using UnityEngine;
using System.Collections.Generic;
using InventoryNamespace;
using CharacterNamespace;

public class EnemyLoot : MonoBehaviour
{
    [Header("Loot Settings")]
    [SerializeField] private GameObject genericPickupPrefab;
    [SerializeField] private List<ItemData> lootTable;
    [Range(0, 1)]
    [SerializeField] private float dropChance = 0.5f;

    public void DropLoot()
    {
        if (Random.value > dropChance)
        {
            Debug.Log($"{gameObject.name} dropped no loot.");
            return;
        }

        if (lootTable.Count > 0 && genericPickupPrefab != null)
        {
            ItemData itemToDrop = lootTable[Random.Range(0, lootTable.Count)];
            GameObject pickupInstance = Instantiate(genericPickupPrefab, transform.position, Quaternion.identity);

            if (pickupInstance.TryGetComponent<ItemPickup>(out var itemPickup))
            {
                itemPickup.Initialize(itemToDrop, 1);
                Debug.Log($"<color=green>{gameObject.name} dropped {itemToDrop.itemName}.</color>");
            }
        }
    }
}
