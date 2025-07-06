using UnityEngine;
using InventoryNamespace;
using System.Collections.Generic;

namespace AnimationNamespace
{
    public class AnimationController : MonoBehaviour
    {
        private Animator playerAnimator;
        private Dictionary<PlayerEquipment.EquipmentSlot, Animator> equipmentAnimators = new Dictionary<PlayerEquipment.EquipmentSlot, Animator>();

        void Awake()
        {
            playerAnimator = GetComponent<Animator>();
        }

        public void UpdateAnimations(Vector2 moveDirection, Vector2 lookDirection, bool isUnsheathed)
        {
            if (playerAnimator == null) return;
            playerAnimator.SetBool("isUnsheathed", isUnsheathed);
            bool isMoving = moveDirection.magnitude > 0.1f;
            playerAnimator.SetFloat("moveX", moveDirection.x);
            playerAnimator.SetFloat("moveY", moveDirection.y);
            if (isMoving)
            {
                playerAnimator.SetFloat("lastMoveX", moveDirection.x);
                playerAnimator.SetFloat("lastMoveY", moveDirection.y);
            }

            if (isUnsheathed)
            {
                playerAnimator.SetFloat("mousePosX", lookDirection.x);
                playerAnimator.SetFloat("mousePosY", lookDirection.y);
            }

            foreach (var equipmentAnimator in equipmentAnimators.Values)
            {
                if (equipmentAnimator != null)
                {
                    equipmentAnimator.SetBool("isUnsheathed", isUnsheathed);
                    equipmentAnimator.SetFloat("moveX", moveDirection.x);
                    equipmentAnimator.SetFloat("moveY", moveDirection.y);
                    if (isUnsheathed)
                    {
                        equipmentAnimator.SetFloat("mousePosY", lookDirection.y);
                        equipmentAnimator.SetFloat("mousePosX", lookDirection.x);
                    }
                }
            }
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
        }
        public void ClearEquipmentAnimator(PlayerEquipment.EquipmentSlot slot)
        {
            if (equipmentAnimators.ContainsKey(slot))
            {
                equipmentAnimators.Remove(slot);
            }
        }

        public void SetWeaponType(WeaponData weaponData)
        {
            if (playerAnimator == null) return;

            playerAnimator.SetBool("isAxe", false);
            playerAnimator.SetBool("isBow", false);
            playerAnimator.SetBool("isDagger", false);
            playerAnimator.SetBool("isPolearm", false);
            playerAnimator.SetBool("isSword", false);
            playerAnimator.SetBool("isMace", false);
            playerAnimator.SetBool("isFistWeapon", false);
            
            if (weaponData == null) return;

            if (weaponData is BowData) playerAnimator.SetBool("isBow", true);
            else if (weaponData is SwordData) playerAnimator.SetBool("isSword", true);
            else if (weaponData is AxeData) playerAnimator.SetBool("isAxe", true);
            else if (weaponData is PolearmData) playerAnimator.SetBool("isPolearm", true);
            else if (weaponData is DaggerData) playerAnimator.SetBool("isDagger", true);
            else if (weaponData is MaceData) playerAnimator.SetBool("isMace", true);
            else if (weaponData is FistWeaponData) playerAnimator.SetBool("isFistWeapon", true);
        }
    }
}