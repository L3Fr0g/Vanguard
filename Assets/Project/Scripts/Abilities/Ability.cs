using UnityEngine;

namespace InventoryNamespace
{
    public enum AbilityType { Active, Passive }
    public enum AttackType { Melee, Ranged }
    public enum CastType { Instant, CastTime, Charged, Channeled }

    [CreateAssetMenu(fileName = "Ability", menuName = "Ability")]
    public class Ability : ScriptableObject
    {
        public string abilityName;
        public string abilityDescription;
        public AbilityType abilityType;
        public AttackType attackType;
        public CastType castType;
        public float cooldown;
        public float castDuration;
        public float castTime;
        public float manaCost;
        public float stamingCost;

        public Sprite icon;
        public GameObject abilityPrefab;
        public string methodName;
    }
}