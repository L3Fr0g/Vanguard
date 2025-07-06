using UnityEngine;
using UnityEngine.InputSystem;
using AnimationNamespace;
using CharacterNamespace;
using MovementNamespace;
using UINamespace;
using InventoryNamespace;

public class PlayerController : MonoBehaviour
{
    private AnimationController animationController;
    private Movement movement;
    private PlayerAbilityManager playerAbilityManager;
    private PlayerControls playerControls;
    private PlayerEquipment playerEquipment;
    private PlayerStats playerStats;

    private Vector2 moveDirection;
    private bool isUnsheathed = false;

    private void Awake()
    {
        animationController = GetComponent<AnimationController>();
        movement = GetComponent<Movement>();
        playerAbilityManager = GetComponent<PlayerAbilityManager>();
        playerControls = new PlayerControls();
        playerEquipment = GetComponent<PlayerEquipment>();
        playerStats = GetComponent<PlayerStats>();

        playerControls.Actions.Unsheath.performed += _ => ToggleSheathe();
        playerControls.UIActions.Inventory.performed += _ => ToggleInventory();

        playerControls.Actions.Action1.performed += _ => PerformPrimaryAction();
    }

    private void OnEnable() => playerControls.Enable();
    private void OnDisable() => playerControls.Disable();

    private void Update()
    {
        moveDirection = playerControls.Movement.Move.ReadValue<Vector2>();
        float currentSpeed = playerStats.MovementSpeed;
        movement.SetTargetVelocity(moveDirection * currentSpeed);

        Vector2 lookDirection;
        if (isUnsheathed)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 playerPos = transform.position;
            lookDirection = (mouseWorldPos - playerPos).normalized;
        }
        else
        {
            lookDirection = moveDirection;
        }
        playerAbilityManager.UpdateAimDirection(lookDirection);
        animationController.UpdateAnimations(moveDirection, lookDirection, isUnsheathed);
    }

    private void ToggleSheathe()
    {
        if (!playerEquipment.IsWeaponEquipped()) return;
        if (!isUnsheathed)
        {
            if (playerEquipment.IsWeaponEquipped())
            {
                isUnsheathed = true;
            }
        }
        else
        {
            //playerAbilityManager.CancelAction(); currently set to private, will need to change this potentially
            isUnsheathed = false;
        }
    }

    public void ForceSheathedState()
    {
        isUnsheathed = false;
    }

    private void ToggleInventory()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleInventoryUI();
        }
    }

    private void PerformPrimaryAction()
    {
        if (!isUnsheathed)
        {
            if (playerEquipment.IsWeaponEquipped())
            {
                isUnsheathed = true;
            }
            else
            {
                return;
            }
        }

        if (playerAbilityManager != null)
        {
            playerAbilityManager.UsePrimaryAbility();
        }
    }
}
