using CharacterNamespace;
using UnityEngine;

namespace EnemyNamespace
{
    public class EnemyStats : CharacterStats
    {
        [Header("Base Stats")]
        public int baseMaxHealth = 50;
        public int baseDamage = 10;
        public float baseMoveSpeed = 3f;

        public int MaxHealth { get; private set; }
        public int Damage { get; private set; }
        //public float MovementSpeed { get; private set; }

        private void Awake()
        {
            ResetStats();
        }

        public void ResetStats()
        {
            MaxHealth = baseMaxHealth;
            Damage = baseDamage;
            MovementSpeed = baseMoveSpeed;
        }

        public void AddDamageModifier(float percentage)
        {
            Damage += (int)(baseDamage * percentage);
        }
    }
}
