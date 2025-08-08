using UnityEngine;

namespace EnemyNamespace
{
    [CreateAssetMenu(fileName = "NewEnemyBuff", menuName = "Enemies/Buff")]
    public class EnemyBuff : ScriptableObject
    {
        public string buffName;
        [Header("Trigger Conditions")]
        [Range(0f, 1f)]
        public float healthThreshold = 0.2f;

        [Header("Effects")]
        [Range(0f, 5f)]
        public float damageModifier = 0.5f;
    }
}