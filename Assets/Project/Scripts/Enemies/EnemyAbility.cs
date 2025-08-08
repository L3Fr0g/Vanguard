using UnityEngine;
using CharacterNamespace;

namespace EnemyNamespace
{
    [CreateAssetMenu(fileName = "NewEnemyAbility", menuName = "Enemies/Ability")]
    public class EnemyAbility : ScriptableObject
    {
        [Header("Ability Info")]
        public string abilityName;
        public string animationTriggerName = "Attack";

        [Header("Timings")]
        public float cooldown = 2f;
        public float windUpTime = 0.5f;
        public float recoveryTime = 0.5f;

        [Header("Combat Details")]
        public int damage = 10;
        public float range = 2f;
    }
}