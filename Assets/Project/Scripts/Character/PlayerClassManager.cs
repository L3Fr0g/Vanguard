using UnityEngine;
using CharacterNamespace;
using Unity.VisualScripting;

namespace InventoryNamespace
{
    public class PlayerClassManager : MonoBehaviour
    {
        [Header("Class")]
        [SerializeField] private ClassData currentClassData;

        [Header("Component References")]
        [SerializeField] private PlayerAbilityManager playerAbilityManager;

        private void Awake()
        {
            if (playerAbilityManager == null)
            {
                playerAbilityManager = GetComponent<PlayerAbilityManager>();
            }
        }

        private void Start()
        {
            GrantStartingAbilities();
        }

        private void GrantStartingAbilities()
        {
            if (currentClassData == null)
            {
                Debug.LogError("No ClassData assigned to the PlayerClassManager!");
                return;
            }

            if (playerAbilityManager != null)
            {
                playerAbilityManager.ClearAllAbilities();
                playerAbilityManager.AddAbilities(currentClassData.startingAbilities);
            }
        }
    }
}