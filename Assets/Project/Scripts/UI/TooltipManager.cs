using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Collections.Generic;

namespace InventoryNamespace
{
    public class TooltipManager : MonoBehaviour
    {
        public static TooltipManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemInfoText;

        [Header("Rarity Colors")]
        [SerializeField] private Color commonColor = Color.white;
        [SerializeField] private Color uncommonColor = Color.green;
        [SerializeField] private Color rareColor = Color.blue;
        [SerializeField] private Color uniqueColor = new Color(0.5f, 0, 0.5f); // Purple
        [SerializeField] private Color legendaryColor = new Color(1f, 0.65f, 0); // Orange/Gold
        [SerializeField] private Color junkColor = Color.gray;

        private RectTransform tooltipRect;
        private Dictionary<ItemRarity, Color> rarityColors;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            rarityColors = new Dictionary<ItemRarity, Color>
            {
                { ItemRarity.Junk, junkColor },
                { ItemRarity.Common, commonColor },
                { ItemRarity.Uncommon, uncommonColor },
                { ItemRarity.Rare, rareColor },
                { ItemRarity.Unique, uniqueColor },
                { ItemRarity.Legendary, legendaryColor }
            };

            if (tooltipPanel != null)
            {
                tooltipRect = tooltipPanel.GetComponent<RectTransform>();
                tooltipPanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (tooltipPanel.activeSelf) tooltipRect.position = Input.mousePosition;
        }

        public void ShowTooltip(ItemData itemData)
        {
            if (itemData == null) return;

            itemNameText.text = itemData.itemName;
            itemNameText.color = rarityColors[itemData.rarity];

            StringBuilder sb = new StringBuilder();

            if (itemData is WeaponData weapon)
            {
                sb.AppendLine($"<color=yellow>Attack Speed:</color> {weapon.attackSpeed:F2}");
                sb.AppendLine($"<color=yellow>Physical Damage:</color> {weapon.minPhysicalDamage} - {weapon.maxPhysicalDamage}");
                if (weapon.minMagicDamage > 0 || weapon.maxMagicDamage > 0) sb.AppendLine($"<color=yellow>Magic Damage:</color> {weapon.minMagicDamage} - {weapon.maxMagicDamage}");
                if (itemData is BowData bow) sb.AppendLine($"<color=yellow>Range:</color> {bow.range}");
            }
            else if (itemData is ArmorData armor)
            {
                sb.AppendLine($"<color=yellow>Armor:</color> {armor.armor}");
                if (armor.health > 0) sb.AppendLine($"<color=green>+ {armor.health} Health</color>");
                if (armor.mana > 0) sb.AppendLine($"<color=blue>+ {armor.mana} Mana</color>");
                if (armor.magicResistance > 0) sb.AppendLine($"<color=purple>+ {armor.magicResistance} Magic Resistance</color>");
            }
            else if (itemData is QuiverData quiver)
            {
                sb.AppendLine($"<color=yellow>Bonus Damage:</color> {quiver.minPhysicalDamage} - {quiver.maxPhysicalDamage}");
            }
            else if (itemData is BagData bag)
            {
                sb.AppendLine($"<color=yellow>Capacity:</color> {bag.inventoryCapacityBonus} slots");
            }

            if (itemData is EquipmentData equipment)
            {
                if (equipment.strength > 0) sb.AppendLine($"<color=red>+ {equipment.strength} Strength</color>");
                if (equipment.agility > 0) sb.AppendLine($"<color=green>+ {equipment.agility} Agility</color>");
                if (equipment.intellect > 0) sb.AppendLine($"<color=blue>+ {equipment.intellect} Intellect</color>");
            }

            if (!string.IsNullOrEmpty(itemData.itemDescription))
            {
                sb.AppendLine();
                sb.AppendLine($"<i><color=#A0A0A0>{itemData.itemDescription}</color></i>");
            }

            itemInfoText.text = sb.ToString();
            tooltipPanel.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
        }

        public void HideTooltip()
        {
            if (tooltipPanel != null) tooltipPanel.SetActive(false);
        }
    }
}