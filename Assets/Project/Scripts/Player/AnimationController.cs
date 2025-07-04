using UnityEngine;
using InventoryNamespace;

namespace AnimationNamespace
{
    public class AnimationController : MonoBehaviour
    {
        private Animator playerAnimator;
        private Animator MHAnimator; // Main-Hand Animator

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

            if (MHAnimator != null)
            {
                MHAnimator.SetBool("isUnsheathed", isUnsheathed);
                MHAnimator.SetFloat("moveX", moveDirection.x);
                MHAnimator.SetFloat("moveY", moveDirection.y);
                if (isUnsheathed)
                {
                    MHAnimator.SetFloat("mousePosY", lookDirection.y);
                    MHAnimator.SetFloat("mousePosX", lookDirection.x);
                }
            }
        }

        public void SetActionBool(string actionName, bool value)
        {
            if (playerAnimator == null) return;
            playerAnimator.SetBool(actionName, value);
            if (MHAnimator != null)
            {
                MHAnimator.SetBool(actionName, value);
            }
        }

        public void PlayCancelAnimation()
        {
            if (playerAnimator == null) return;
            playerAnimator.SetTrigger("cancelAction");
            if (MHAnimator != null)
            {
                MHAnimator.SetTrigger("cancelAction");
            }
        }

        public void SetWeaponAnimator(Animator weaponAnimator) => MHAnimator = weaponAnimator;
        public void ClearWeaponAnimator() => MHAnimator = null;

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