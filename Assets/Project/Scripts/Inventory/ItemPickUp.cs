using UnityEngine;

namespace InventoryNamespace 
{
    public class ItemPickUp : MonoBehaviour
    {
        public ItemData itemData;
        public int quantity = 1;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerInventorySystem playerInventory = collision.GetComponent<PlayerInventorySystem>();
                if (playerInventory != null)
                {
                    quantity = playerInventory.PickUpItem(itemData, quantity);
                    
                    InventoryPanelManager.Instance.RefreshInventoryUI();

                    if (quantity <= 0)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}