using UnityEngine;
using CharacterNamespace;
using Unity.VisualScripting;

namespace InventoryNamespace
{
    public class PlayerClassManager : MonoBehaviour
    {
        [Header("Class")]
        [SerializeField] private ClassData currentClassData;

        private PlayerController playerController;

        private void Awake()
        {   
            playerController = GetComponent<PlayerController>();
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

            foreach (var ability in currentClassData.startingAbilities)
            {
                playerController.AssignAbilityToSlot(ability, ability.abilitySlot);
            }
        }
    }
}