using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterNamespace;
using System.Linq;

namespace EnemyNamespace
{
    public class EnemyAbilityManager : MonoBehaviour
    {
        [Header("Abilities")]
        [SerializeField] private List<EnemyAbility> abilities;

        [Header("Component References")]
        [SerializeField] private Animator animator;
        [SerializeField] private EnemyStats enemyStats;
        [SerializeField] private MeleeHitboxController meleeHitbox;

        private Dictionary<EnemyAbility, float> abilityCooldowns = new Dictionary<EnemyAbility, float>();

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (meleeHitbox == null) meleeHitbox = GetComponentInChildren<MeleeHitboxController>();
            if (enemyStats == null) enemyStats = GetComponent<EnemyStats>();
        }

        public bool CanUseAbility(EnemyAbility ability)
        {
            if (ability == null) return false;
            return !abilityCooldowns.ContainsKey(ability) || Time.time >= abilityCooldowns[ability];
        }

        public EnemyAbility GetPrimaryAbility()
        {
            return abilities.FirstOrDefault();
        }

        public IEnumerator UseAbility(EnemyAbility ability, Transform target)
        {
            if (!CanUseAbility(ability)) yield break;

            animator.SetTrigger(ability.animationTriggerName);

            abilityCooldowns[ability] = Time.time + ability.cooldown;

            yield return new WaitForSeconds(ability.windUpTime);

            if (meleeHitbox != null && target != null)
            {
                Vector2 direction = (target.position - transform.position).normalized;
                int finalDamage = enemyStats.Damage;
                meleeHitbox.PerformAttack(ability.damage, direction, ability.range, transform);
            }

            yield return new WaitForSeconds(ability.recoveryTime);
        }
    }
}