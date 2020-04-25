using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TournamentFairArmour
{
    public class DefaultEquipmentLoader
    {
        private const string StringIdPrefix = "tournament_fair_armour_equipment_override_";

        public Dictionary<string, Equipment> LoadDefaultEquipmentsByCulture()
        {
            var defaultEquipmentsByCulture = new Dictionary<string, Equipment>();
            FindEquipmentAndAddOverride(SubModule.DefaultEquipmentSetStringId, defaultEquipmentsByCulture);
            ObjectManagerUtils.GetAllMainCultureObjects()
                .ForEach(cultureObject => FindEquipmentAndAddOverride(cultureObject.StringId, defaultEquipmentsByCulture));
            return defaultEquipmentsByCulture;
        }

        private void FindEquipmentAndAddOverride(string cultureObjectStringId, Dictionary<string, Equipment> defaultEquipmentsByCulture)
        {
            var characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(StringIdPrefix + cultureObjectStringId);
            if (characterObject != null)
            {
                AddEquipmentOverride(cultureObjectStringId, characterObject, defaultEquipmentsByCulture);
            }
        }

        private void AddEquipmentOverride(string cultureStringId, CharacterObject characterObject, Dictionary<string, Equipment> equipmentSetByCulture)
        {
            if (characterObject.BattleEquipments.Any())
            {
                equipmentSetByCulture[cultureStringId] = characterObject.BattleEquipments.First();
            }
        }
    }
}
