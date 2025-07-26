using UnityEngine;
using UnityEngine.InputSystem;
using AnimationNamespace;
using CharacterNamespace;
using MovementNamespace;
using UINamespace;
using InventoryNamespace;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private AnimationController animationController;
    private Movement movement;
    private PlayerAbilityManager playerAbilityManager;
    private PlayerControls playerControls;
    private PlayerInteractor playerInteractor;
    private PlayerEquipment playerEquipment;
    private PlayerStats playerStats;

    private Vector2 moveDirection;
    private Vector2 lookDirection;
    private bool isUnsheathed = false;
    private bool isInteracting = false;

    private void Awake()
    {
        animationController = GetComponent<AnimationController>();
        movement = GetComponent<Movement>();
        playerAbilityManager = GetComponent<PlayerAbilityManager>();
        playerControls = new PlayerControls();
        playerInteractor = GetComponent<PlayerInteractor>();
        playerEquipment = GetComponent<PlayerEquipment>();
        playerStats = GetComponent<PlayerStats>();

        playerControls.UIActions.Inventory.performed += _ => ToggleInventory();
        playerControls.Actions.Unsheath.performed += _ => ToggleSheathe();
        playerControls.Actions.Action1.performed += _ => PerformPrimaryAction();
    }

    private void OnEnable() => playerControls.Enable();
    private void OnDisable() => playerControls.Disable();

    private void Update()
    {
        if(isInteracting)
        {
            movement.SetTargetVelocity(Vector2.zero);
            return;
        }

        moveDirection = playerControls.Movement.Move.ReadValue<Vector2>();
        float currentSpeed = playerStats.MovementSpeed;
        movement.SetTargetVelocity(moveDirection * currentSpeed);

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

    public IEnumerator StartInteraction(Transform targetPoint)
    {
        isInteracting = true;

        if (targetPoint != null)
        {
            Debug.Log("Moving to interaction point...");
            while (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
            {
                Vector2 direction = (targetPoint.position - transform.position).normalized;
                movement.SetTargetVelocity(direction * playerStats.MovementSpeed);
                yield return null;
            }
        }

        movement.SetTargetVelocity(Vector2.zero);
        Debug.Log("Starting gathering animation.");
        animationController.SetActionBool("isGatering", true);
    }

    public void StartInteractionAnimation(InteractionType type)
    {
        isInteracting = true;
        ForceSheathedState();
        animationController.UpdateAnimations(Vector2.zero, lookDirection, isUnsheathed);

        switch (type)
        {
            case InteractionType.Mining:
                animationController.SetActionBool("isMining", true);
                break;
            case InteractionType.Fishing:
                animationController.SetActionBool("isFishing", true);
                break;
            case InteractionType.Pickup:
                animationController.SetActionBool("isPickup", true);
                break;
        }
    }

    public void EndInteraction()
    {
        isInteracting = false;
        animationController.SetActionBool("isMining", false);
        animationController.SetActionBool("isFishing", false);
        animationController.SetActionBool("isPickup", false);
    }

    private void ToggleSheathe()
    {
        if (isInteracting || !playerEquipment.IsWeaponEquipped()) return;
        isUnsheathed = !isUnsheathed;
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
