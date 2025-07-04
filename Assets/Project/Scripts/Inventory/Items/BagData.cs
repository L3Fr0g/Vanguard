using UnityEngine;

namespace InventoryNamespace
{
    [CreateAssetMenu(fileName = "NewBag", menuName = "Items/Equipment/Bag")]
    public class BagData : ItemData
    {
        [Header("Primary Stat")]
        public int inventoryCapacityBonus;
    }
}