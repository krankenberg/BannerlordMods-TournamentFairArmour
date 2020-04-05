using System.Collections.Generic;
using System.Linq;
using SandBox.TournamentMissions.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TournamentFairArmour
{
    public class TournamentFairArmourSubModule : MBSubModuleBase
    {
        private const string StringIdPrefix = "tournament_fair_armour_equipment_override_";
        private const string NeutralCultureStringId = "neutral_culture";

        internal static readonly EquipmentIndex[] OverriddenEquipmentIndices =
        {
            EquipmentIndex.Head,
            EquipmentIndex.Cape,
            EquipmentIndex.Body,
            EquipmentIndex.Gloves,
            EquipmentIndex.Leg
        };

        private Dictionary<string, IEnumerable<Equipment>> _equipmentSetsByCulture;

        public override void OnGameInitializationFinished(Game game)
        {
            _equipmentSetsByCulture = new Dictionary<string, IEnumerable<Equipment>>();
            var allCharacterObjects = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>();
            allCharacterObjects
                .Where(characterObject => characterObject.StringId.StartsWith(StringIdPrefix))
                .ToList()
                .ForEach(AddEquipmentOverride);
        }

        private void AddEquipmentOverride(CharacterObject characterObject)
        {
            if (characterObject.Culture != null)
            {
                _equipmentSetsByCulture[characterObject.Culture.StringId] = characterObject.BattleEquipments;
            }
        }

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            if (mission.HasMissionBehaviour<TournamentBehavior>())
            {
                AddTournamentFairArmourMissionListener(mission);
            }
        }

        private void AddTournamentFairArmourMissionListener(Mission mission)
        {
            var equipmentSets = GetEquipmentSetsByCultureOfCurrentSettlement();
            if (equipmentSets != null)
            {
                mission.AddListener(CreateTournamentFairArmourMissionListener(equipmentSets));
            }
            else
            {
                AddListenerWithDefaultEquipmentSets(mission);
            }
        }

        private IEnumerable<Equipment> GetEquipmentSetsByCultureOfCurrentSettlement()
        {
            var cultureStringId = Settlement.CurrentSettlement?.Culture?.StringId;
            _equipmentSetsByCulture.TryGetValue(cultureStringId ?? string.Empty, out var equipmentSets);
            return equipmentSets;
        }

        private void AddListenerWithDefaultEquipmentSets(Mission mission)
        {
            _equipmentSetsByCulture.TryGetValue(NeutralCultureStringId, out var equipmentSets);
            if (equipmentSets != null)
            {
                mission.AddListener(CreateTournamentFairArmourMissionListener(equipmentSets));
            }
        }

        private TournamentFairArmourMissionListener CreateTournamentFairArmourMissionListener(IEnumerable<Equipment> equipmentSets)
        {
            var tournamentEquipment = new Equipment();
            var randomEquipment = equipmentSets.GetRandomElement();
            foreach (var overriddenEquipmentIndex in OverriddenEquipmentIndices)
            {
                tournamentEquipment[overriddenEquipmentIndex] = randomEquipment[overriddenEquipmentIndex];
            }

            return new TournamentFairArmourMissionListener(tournamentEquipment);
        }
    }
}
