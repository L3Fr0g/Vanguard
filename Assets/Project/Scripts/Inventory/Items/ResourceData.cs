using UnityEngine;

namespace InventoryNamespace
{
    [CreateAssetMenu(fileName = "NewResource", menuName = "Items/Resource")]
    public class ResourceData : ItemData
    {
        // For now, this class can be empty. Its main purpose is to provide a distinct "Resource"
        // type that we can check for in our code (e.g., "if (item is ResourceData)").

        // In the future, you could add resource-specific fields here, for example:
        // public GatheringToolType requiredTool;
        // public int requiredGatheringLevel;
        private void OnValidate()
        {
            isStackable = true;
            if (maxStackSize <= 1)
            {
                maxStackSize = 99;
            }
        }
    }
}
