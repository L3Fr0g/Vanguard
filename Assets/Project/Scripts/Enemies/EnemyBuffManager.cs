using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace EnemyNamespace
{
    public class EnemyBuffManager : MonoBehaviour
    {
        [Header("Buffs")]
        [SerializeField] private List<EnemyBuff> buffs;

        [Header("Component References")]
        [SerializeField] private EnemyStats enemyStats;

        private List<EnemyBuff> activeBuffs = new List<EnemyBuff>();

        private void Awake()
        {
            if (enemyStats == null) enemyStats = GetComponent<EnemyStats>();
        }

        public void UpdateBuffs(float currentHealth, float maxHealth)
        {
            enemyStats.ResetStats();
            activeBuffs.Clear();

            float healthPercentage = currentHealth / maxHealth;

            foreach (var buff in buffs)
            {
                if (healthPercentage <= buff.healthThreshold)
                {
                    ApplyBuff(buff);
                    activeBuffs.Add(buff);
                }
            }
        }

        private void ApplyBuff(EnemyBuff buff)
        {
            Debug.Log($"<color=cyan>Applying buff '{buff.buffName}' to {gameObject.name}</color>");

            if (buff.damageModifier > 0)
            {
                enemyStats.AddDamageModifier(buff.damageModifier);
            }
        }
    }
}