using UnityEngine;
using InventoryNamespace;
using System.Collections.Generic;

namespace AnimationNamespace
{
    public class AnimationController : MonoBehaviour
    {
        private Animator playerAnimator;
        private Dictionary<PlayerEquipment.EquipmentSlot, Animator> equipmentAnimators = new Dictionary<PlayerEquipment.EquipmentSlot, Animator>();
        private WeaponData currentWeaponData;

        private Vector2 lastMoveDirection;
        private Vector2 lastLookDirection;
        private bool lastUnsheathedState;
        private bool isMoving;

        void Awake()
        {
            playerAnimator = GetComponent<Animator>();
        }

        private void SyncAnimatorState(Animator targetAnimator)
        {
            if (targetAnimator == null) return;

            targetAnimator.SetFloat("moveX", lastMoveDirection.x);
            targetAnimator.SetFloat("moveY", lastMoveDirection.y);
            targetAnimator.SetBool("isUnsheathed", lastUnsheathedState);

            if (isMoving)
            {
                targetAnimator.SetFloat("lastMoveX", lastMoveDirection.x);
                targetAnimator.SetFloat("lastMoveY", lastMoveDirection.y);
            }

            if (lastUnsheathedState)
            {
                targetAnimator.SetFloat("mousePosX", lastLookDirection.x);
                targetAnimator.SetFloat("mousePosY", lastLookDirection.y);
            }

            UpdateAnimatorWeaponState(targetAnimator);
        }

        public void UpdateAnimations(Vector2 moveDirection, Vector2 lookDirection, bool isUnsheathed)
        {
            lastMoveDirection = moveDirection;
            lastLookDirection = lookDirection;
            lastUnsheathedState = isUnsheathed;
            isMoving = moveDirection.magnitude > 0.1f;

            SyncAnimatorState(playerAnimator);
            foreach (var equipmentAnimator in equipmentAnimators.Values) { SyncAnimatorState(equipmentAnimator); }
        }

        public void SetActionBool(string actionName, bool value)
        {
            if (playerAnimator != null) playerAnimator.SetBool(actionName, value);
            foreach (var equipmentAnimator in equipmentAnimators.Values)
            {
                if (equipmentAnimator != null) equipmentAnimator.SetBool(actionName, value);
            }
        }

        public void PlayCancelAnimation()
        {
            if (playerAnimator != null) playerAnimator.SetTrigger("cancelAction");
            foreach (var equipmentAnimator in equipmentAnimators.Values)
            {
                if(equipmentAnimator != null) equipmentAnimator.SetTrigger("cancelAction");
            }
        }

        public void SetEquipmentAnimator(PlayerEquipment.EquipmentSlot slot, Animator animator)
        {
            equipmentAnimators[slot] = animator;
            SyncAnimatorState(animator);
        }

        public void ClearEquipmentAnimator(PlayerEquipment.EquipmentSlot slot)
        {
            if (equipmentAnimators.ContainsKey(slot))
            {
                equipmentAnimators.Remove(slot);
            }
        }

        private void UpdateAnimatorWeaponState(Animator targetAnimator)
        {
            if (targetAnimator == null) return;
            targetAnimator.SetBool("isAxe", false);
            targetAnimator.SetBool("isBow", false);
            targetAnimator.SetBool("isDagger", false);
            targetAnimator.SetBool("isPolearm", false);
            targetAnimator.SetBool("isSword", false);
            targetAnimator.SetBool("isMace", false);
            targetAnimator.SetBool("isFistWeapon", false);

            if (currentWeaponData == null) return;

            if (currentWeaponData is BowData) targetAnimator.SetBool("isBow", true);
            else if (currentWeaponData is SwordData) targetAnimator.SetBool("isSword", true);
            else if (currentWeaponData is AxeData) targetAnimator.SetBool("isAxe", true);
            else if (currentWeaponData is PolearmData) targetAnimator.SetBool("isPolearm", true);
            else if (currentWeaponData is DaggerData) targetAnimator.SetBool("isDagger", true);
            else if (currentWeaponData is MaceData) targetAnimator.SetBool("isMace", true);
            else if (currentWeaponData is FistWeaponData) targetAnimator.SetBool("isFistWeapon", true);
        }

        public void SetWeaponType(WeaponData weaponData)
        {
            currentWeaponData = weaponData;

            SyncAnimatorState(playerAnimator);
            foreach (var equipmentAnimator in equipmentAnimators.Values) { SyncAnimatorState(equipmentAnimator); }
        }
    }
}