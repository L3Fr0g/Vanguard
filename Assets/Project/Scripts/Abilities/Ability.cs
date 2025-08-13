using UnityEngine;
using UnityEngine.Rendering;

namespace InventoryNamespace
{
    public enum AbilityType { Active, Passive }
    public enum AttackType { Melee, Ranged }
    public enum CastType { Instant, CastTime, Charged, Channeled }

    public enum AbilitySlot
    {
        Primary,
        Secondary,
        Utility1,
        Utility2,
        Utility3
    }

    [CreateAssetMenu(fileName = "Ability", menuName = "Ability")]
    public class Ability : ScriptableObject
    {
        public string abilityName;
        public string abilityDescription;
        public string animationAction;
        public AbilityType abilityType;
        public AttackType attackType;
        public CastType castType;

        [Header("Binding")]
        public AbilitySlot abilitySlot;

        [Header("Assets & Stats")]
        public GameObject abilityPrefab;
        public Sprite icon;
        public bool isPressed = false;
        public int range;
        public float cooldown;
        public float castDuration;
        public float minChargeTime;
        public float manaCost;
        public float staminaCost;
        //public string methodName;

        [Header("Area of Effect")]
        public bool isAreaOfEffect = false;
        public float areaDuration = 3f;
        public float areaTickRate = 0.5f;
    }
}