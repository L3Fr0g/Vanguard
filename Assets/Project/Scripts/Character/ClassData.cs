using System.Collections.Generic;
using UnityEngine;
using InventoryNamespace;

namespace CharacterNamespace
{
    [CreateAssetMenu(fileName = "NewClassData", menuName = "Character/Class Data")]
    public class ClassData : ScriptableObject
    {
        [Header("Class Definition")]
        public PlayerClass playerClass;
        [TextArea(3,5)]
        public string classDesctiprion;

        [Header("Starting Abilities")]
        public List<Ability> startingAbilities;

        // Future additions could go here, e.g.:
        // public TalentTree talentTree;
        // public List<StatBonus> baseStatBonuses;
    }
}