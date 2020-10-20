using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TournamentFairArmour
{
    public class ObjectManagerUtils
    {
        public static List<CultureObject> GetAllMainCultureObjects()
        {
            return MBObjectManager.Instance.GetObjectTypeList<CultureObject>()
                .Where(cultureObject => cultureObject.IsMainCulture)
                .OrderBy(cultureObject => cultureObject.Name.ToString())
                .ToList();
        }

        public static IEnumerable<EquipmentElement> GetAllEquipmentElements(EquipmentIndex equipmentIndex)
        {
            return Items
                .FindAll(itemObject => Equipment.IsItemFitsToSlot(equipmentIndex, itemObject))
                .Select(itemObject => new EquipmentElement(itemObject))
                .OrderBy(GetCultureStringId)
                .ThenByDescending(CalculateArmourSum);
        }

        internal static string GetCultureStringId(EquipmentElement equipmentElement)
        {
            var cultureObject = equipmentElement.Item.Culture;
            if (cultureObject != null && cultureObject.IsMainCulture)
            {
                return cultureObject.StringId;
            }

            return "zzzOther";
        }

        internal static string GetCultureName(EquipmentElement equipmentElement)
        {
            var cultureObject = equipmentElement.Item.Culture;
            if (cultureObject != null && cultureObject.IsMainCulture)
            {
                return cultureObject.Name.ToString();
            }

            return "Other";
        }

        private static int CalculateArmourSum(EquipmentElement equipmentElement)
        {
            return equipmentElement.GetModifiedArmArmor() + equipmentElement.GetModifiedHeadArmor() + equipmentElement.GetModifiedBodyArmor() + equipmentElement.GetModifiedLegArmor();
        }

        public static string BuildTooltipString(EquipmentElement equipmentElement)
        {
            var messageParts = new List<string>();

            var cultureObject = equipmentElement.Item.Culture;
            if (cultureObject != null)
            {
                messageParts.Add(cultureObject.Name + "\n");
            }

            AddArmourValueString(messageParts, "Head", equipmentElement.GetModifiedHeadArmor());
            AddArmourValueString(messageParts, "Arm", equipmentElement.GetModifiedArmArmor());
            AddArmourValueString(messageParts, "Body", equipmentElement.GetModifiedBodyArmor());
            AddArmourValueString(messageParts, "Leg", equipmentElement.GetModifiedLegArmor());

            return string.Join("\n", messageParts);
        }

        public static string BuildTooltipString(Equipment equipment)
        {
            var messageParts = new List<string>();

            AddArmourValueString(messageParts, "Head", equipment.GetHeadArmorSum(), false);
            AddArmourValueString(messageParts, "Arm", equipment.GetArmArmorSum(), false);
            AddArmourValueString(messageParts, "Body", equipment.GetHumanBodyArmorSum(), false);
            AddArmourValueString(messageParts, "Leg", equipment.GetLegArmorSum(), false);

            return string.Join("\n", messageParts);
        }

        private static void AddArmourValueString(ICollection<string> messageParts, string armourName, float armourValue, bool omitZeroValues = true)
        {
            var roundedArmourValue = (int) Math.Round(armourValue);
            if (!omitZeroValues || roundedArmourValue != 0)
            {
                messageParts.Add($"{armourName} Armour: {roundedArmourValue}");
            }
        }
    }
}
