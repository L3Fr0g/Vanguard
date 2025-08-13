using AnimationNamespace;
using CharacterNamespace;
using InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UINamespace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private AnimationController animationController;
    private CharacterStats stats;
    private Dictionary<AbilitySlot, Ability> slottedAbilities = new Dictionary<AbilitySlot, Ability>();
    private Movement movement;
    private PlayerAbilityManager playerAbilityManager;
    private PlayerControls playerControls;
    private PlayerEquipment playerEquipment;

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
        playerEquipment = GetComponent<PlayerEquipment>();
        stats = GetComponent<CharacterStats>();

        playerControls.UIActions.Inventory.performed += _ => ToggleInventory();
        playerControls.Actions.Unsheath.performed += _ => ToggleSheathe();
        playerControls.Actions.CancelAction.performed += _ => CancelAction();
        playerControls.Actions.Action1.started += _ => OnActionPressed(AbilitySlot.Primary);
        playerControls.Actions.Action1.canceled += _ => OnActionReleased(AbilitySlot.Primary);
        playerControls.Actions.Action2.started += _ => OnActionPressed(AbilitySlot.Secondary);
        playerControls.Actions.Action2.canceled += _ => OnActionReleased(AbilitySlot.Secondary);
    }

    private void OnEnable() => playerControls.Enable();
    private void OnDisable() => playerControls.Disable();

    private void Update()
    {
        if(isInteracting)
        {
            movement.SetMoveDirection(Vector2.zero);
            return;
        }

        moveDirection = playerControls.Movement.Move.ReadValue<Vector2>();

        float currentSpeed = stats.MovementSpeed;

        if (isUnsheathed)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 playerPos = transform.position;
            lookDirection = (mouseWorldPos - playerPos).normalized;
        }
        else
        {
            lookDirection = moveDirection;
            currentSpeed = stats.MovementSpeed;
        }
        movement.SetMoveDirection(moveDirection);
        playerAbilityManager.UpdateAimDirection(lookDirection);
        animationController.UpdateAnimations(moveDirection, lookDirection, isUnsheathed);
    }

    public void AssignAbilityToSlot(Ability ability, AbilitySlot slot)
    {
        slottedAbilities[slot] = ability;
        Debug.Log($"Assigned '{ability.abilityName}' to slot {slot}");
    }

    public IEnumerator StartInteraction(Transform targetPoint)
    {
        isInteracting = true;

        if (targetPoint != null)
        {
            while (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
            {
                Vector2 direction = (targetPoint.position - transform.position).normalized;
                movement.SetMoveDirection(direction * stats.MovementSpeed);
                yield return null;
            }
        }

        movement.SetMoveDirection(Vector2.zero);
        animationController.SetActionBool("isGatering", true);
    }

    public void StartInteractionAnimation(InteractionType type)
    {
        isInteracting = true;
        ForceSheathedState();
        animationController.UpdateAnimations(Vector2.zero, lookDirection, isUnsheathed);

        switch (type)
        {
            case InteractionType.Mining: animationController.SetActionBool("isMining", true); break;
            case InteractionType.Fishing: animationController.SetActionBool("isFishing", true); break;
            case InteractionType.Pickup: animationController.SetActionBool("isPickup", true); break;
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
        CancelAction();
        movement.SetMovementState(isUnsheathed ? Movement.MovementState.Unsheathed : Movement.MovementState.Sheathed);
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

    private void CancelAction()
    {
        playerAbilityManager.CancelAbility();
        movement.SetMovementState(isUnsheathed ? Movement.MovementState.Unsheathed : Movement.MovementState.Sheathed);
    }

    private void OnActionPressed(AbilitySlot slot)
    {
        if ((slot == AbilitySlot.Primary || slot == AbilitySlot.Secondary) && !isUnsheathed)
        {
            if (playerEquipment.IsWeaponEquipped())
            {
                ToggleSheathe();
            }
            else
            {
                return;
            }
        }

        if (slottedAbilities.TryGetValue(slot, out Ability ability))
        {
            playerAbilityManager.UseAbility(ability);
            ability.isPressed = true;
        }
    }

    private void OnActionReleased(AbilitySlot slot)
    {
        slottedAbilities.TryGetValue(slot, out Ability ability);
        ability.isPressed = false;
    }
}
