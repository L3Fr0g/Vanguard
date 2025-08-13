using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventoryNamespace;
using AnimationNamespace;

namespace CharacterNamespace
{
    public class PlayerAbilityManager : MonoBehaviour
    {
        [Header("Ability Configuration")]
        [SerializeField] private Transform projectileSpawnPoint;

        private bool isCasting = false;
        private Dictionary<Ability, float> abilityCooldowns = new Dictionary<Ability, float>();

        private Vector2 currentAimDirection;

        public bool actionInputWasCancelled = false;

        private AnimationController animationController;
        private Coroutine activeCastCoroutine;
        private PlayerControls playerControls;
        private PlayerEquipment playerEquipment;
        private PlayerStats playerStats;
        private Movement movement;
        public List<Ability> useableAbilities = new List<Ability>();

        private void Awake()
        {
            animationController = GetComponent<AnimationController>();
            playerControls = new PlayerControls();
            playerEquipment = GetComponent<PlayerEquipment>();
            playerStats = GetComponent<PlayerStats>();
            movement = GetComponent<Movement>();
        }

        private void OnEnable() => playerControls.Enable();
        private void OnDisable() => playerControls.Disable();

        public void UpdateAimDirection(Vector2 direction) => currentAimDirection = direction;

        public void UseAbility(Ability ability)
        {
            if (isCasting) return;
            if (abilityCooldowns.ContainsKey(ability) && Time.time < abilityCooldowns[ability]) return;
            if ((ability.abilitySlot == AbilitySlot.Primary || ability.abilitySlot == AbilitySlot.Secondary) && !playerEquipment.IsWeaponEquipped()) return;

            switch (ability.castType)
            {
                case CastType.Instant:
                    activeCastCoroutine = StartCoroutine(PerformInstantCast(ability));
                    break;
                case CastType.CastTime:
                    activeCastCoroutine = StartCoroutine(PerformCastWithDelay(ability));
                    break;
                case CastType.Charged:
                    activeCastCoroutine = StartCoroutine(PerformCharge(ability));
                    break;
            }
        }

        public void CancelAbility()
        {
            if (isCasting)
            {
                actionInputWasCancelled = true;
                animationController.PlayCancelAnimation();
                animationController.SetActionBool("action1", false);
                animationController.SetActionBool("action2", false);
                if (activeCastCoroutine != null) StopCoroutine(activeCastCoroutine);
                isCasting = false;
                Debug.Log("<color=orange>Cast Cancelled by Player!</color>");
            }
        }

        private IEnumerator PerformInstantCast(Ability ability)
        {
            isCasting = true;
            animationController.SetActionBool(ability.animationAction, true);
            yield return new WaitForSeconds(0.1f);
            Cast(ability);
        }

        private IEnumerator PerformCastWithDelay(Ability ability)
        {
            isCasting = true;
            Debug.Log($"Casting '{ability.abilityName}'... waiting {ability.minChargeTime}s");
            movement.SetMovementState(Movement.MovementState.Attacking);
            yield return new WaitForSeconds(ability.minChargeTime);
            Cast(ability);
            isCasting = false;
        }

        private IEnumerator PerformCharge(Ability ability)
        {
            isCasting = true;
            animationController.SetActionBool(ability.animationAction, true);
            float currentChargeTime = 0f;
            float minRequiredCharge = ability.minChargeTime * playerStats.AttackSpeed;
            movement.SetMovementState(Movement.MovementState.Attacking);
            Debug.Log($"Drawing arrow... Must hold for at least {minRequiredCharge:F2}s");

            while (currentChargeTime < minRequiredCharge)
            {
                if (!isCasting) { animationController.SetActionBool(ability.animationAction, false); yield break; }
                currentChargeTime += Time.deltaTime;
                yield return null;
            }

            Debug.Log("<color=lime>Arrow is drawn. Waiting for release...</color>");

            while (ability.isPressed)
            {
                if (!isCasting){ yield break; }
                currentChargeTime += Time.deltaTime;
                yield return null;
            }

            if (isCasting)
            {
                float chargeLevel = currentChargeTime / minRequiredCharge;
                Cast(ability, chargeLevel);
            }
        }

        private void Cast(Ability ability, float chargeLevel = 1f)
        {
            if (ability == null) return;
            isCasting = false;
            animationController.SetActionBool(ability.animationAction, false);
            abilityCooldowns[ability] = Time.time + ability.cooldown;
            movement.SetMovementState(Movement.MovementState.Unsheathed);

            switch (ability.attackType)
            {
                case AttackType.Ranged:
                    HandleRangedCast(ability);
                    break;
                case AttackType.Melee:
                    HandleMeleeCast(ability);
                    break;
            }
        }

        private void HandleRangedCast(Ability ability)
        {
            GameObject projectileToSpawn = playerEquipment.GetEquippedProjectilePrefab();

            if (ability.isAreaOfEffect)
            {
                HandleAreaOfEffectCast(ability);
                return;
            }

            if (projectileToSpawn == null)
            {
                projectileToSpawn = ability.abilityPrefab;
            }

            if (projectileToSpawn == null)
            {
                Debug.LogWarning($"Ranged attack '{ability.abilityName} failed. No projectile available");
                return;
            }

            if (projectileToSpawn != null && projectileSpawnPoint != null)
            {
                float angle = Mathf.Atan2(currentAimDirection.y, currentAimDirection.x) * Mathf.Rad2Deg;
                Quaternion finalRotation = Quaternion.Euler(0f, 0f, angle);

                GameObject projectile = Instantiate(projectileToSpawn, projectileSpawnPoint.position, finalRotation);

                if (projectile.TryGetComponent<ProjectileController>(out var controller))
                {
                    int damage = Random.Range(playerStats.MinPhysicalDamage, playerStats.MaxPhysicalDamage + 1);
                    controller.Initialize(damage, ability.range, transform);
                }
            }
        }

        private void HandleAreaOfEffectCast(Ability ability)
        {
            Vector2 playerPosition = transform.position;
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float distance = Vector2.Distance(playerPosition, targetPosition);
            Vector2 direction = (targetPosition - playerPosition).normalized;

            if (distance > ability.range) //if cursor is > maxRange targetPosition = maxRange
            {
                targetPosition = playerPosition + (direction * ability.range);
            }

            GameObject AOEInstance = Instantiate(ability.abilityPrefab, targetPosition, Quaternion.identity);

            if (AOEInstance.TryGetComponent<AreaOfEffectController>(out var AOEController))
            {
                float totalDamage = Random.Range(playerStats.MinPhysicalDamage, playerStats.MaxPhysicalDamage + 1);
                AOEController.Initialize(totalDamage, ability.areaDuration, ability.areaTickRate, transform);
            }
        }

        private void HandleMeleeCast(Ability ability)
        {
            MeleeHitboxController hitbox = playerEquipment.GetEquippedMeleeHitbox();

            if (hitbox != null)
            {
                int damage = Random.Range(playerStats.MinPhysicalDamage, playerStats.MaxPhysicalDamage + 1);
                float range = 0f;
                var mainHandSlot = playerEquipment.equippedItems[PlayerEquipment.EquipmentSlot.MainHand];
                if (mainHandSlot?.itemData is WeaponData weapon) 
                {
                    range = weapon.range;
                }

                if (range <= 0) range = 1.5f;

                hitbox.PerformAttack(damage, currentAimDirection, range, transform);
            }
            else
            {
                Debug.LogWarning($"Tried to perform melee attack '{ability.abilityName}', but no MeleeHitboxController was found on the weapon!");
            }
        }

        public void AddAbilities(List<Ability> abilitiesToAdd)
        {
            if (abilitiesToAdd == null) return;
            foreach (var ability in abilitiesToAdd)
            {
                if (ability.abilityType == AbilityType.Active && !useableAbilities.Contains(ability))
                {
                    useableAbilities.Add(ability);
                    Debug.Log($"Ability '{ability.abilityName}' granted");
                }
            }
        }

        public void ClearAllAbilities()
        {
            useableAbilities.Clear();
            Debug.Log("All useable abilities have been cleared");
        }

        public void RemoveAbilites(List<Ability> abilities)
        {
            if (abilities == null) return;
            foreach (var ability in abilities)
            {
                if (useableAbilities.Contains(ability))
                {
                    useableAbilities.Remove(ability);
                    Debug.Log($"Ability {ability.abilityName} has been removed.");
                    //TODO: Remove ability to the players actionbar UI.
                }
            }
        }
    }
}