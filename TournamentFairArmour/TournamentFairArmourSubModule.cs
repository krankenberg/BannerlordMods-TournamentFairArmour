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
        private const string DefaultEquipmentSetStringId = "default";

        internal static readonly EquipmentIndex[] OverriddenEquipmentIndices =
        {
            EquipmentIndex.Head,
            EquipmentIndex.Cape,
            EquipmentIndex.Body,
            EquipmentIndex.Gloves,
            EquipmentIndex.Leg
        };

        private Dictionary<string, Equipment> _equipmentSetByCulture;

        public override void OnGameInitializationFinished(Game game)
        {
            if (game.GameType is Campaign campaign)
            {
                _equipmentSetByCulture = new Dictionary<string, Equipment>();
                AddDefaultEquipmentOverride();
                var allCharacterObjects = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>();
                allCharacterObjects
                    .Where(characterObject => characterObject.StringId.StartsWith(StringIdPrefix))
                    .ToList()
                    .ForEach(AddEquipmentOverride);
            }
        }

        private void AddDefaultEquipmentOverride()
        {
            var characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(StringIdPrefix + DefaultEquipmentSetStringId);
            _equipmentSetByCulture[DefaultEquipmentSetStringId] = characterObject.BattleEquipments.First();
        }

        private void AddEquipmentOverride(CharacterObject characterObject)
        {
            if (characterObject.Culture != null)
            {
                _equipmentSetByCulture[characterObject.Culture.StringId] = characterObject.BattleEquipments.First();
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
            var equipmentSets = GetEquipmentSetByCultureOfCurrentSettlement();
            if (equipmentSets != null)
            {
                mission.AddListener(CreateTournamentFairArmourMissionListener(equipmentSets));
            }
            else
            {
                AddListenerWithDefaultEquipmentSets(mission);
            }
        }

        private Equipment GetEquipmentSetByCultureOfCurrentSettlement()
        {
            var cultureStringId = Settlement.CurrentSettlement?.Culture?.StringId;
            _equipmentSetByCulture.TryGetValue(cultureStringId ?? string.Empty, out var equipmentSet);
            return equipmentSet;
        }

        private void AddListenerWithDefaultEquipmentSets(Mission mission)
        {
            _equipmentSetByCulture.TryGetValue(DefaultEquipmentSetStringId, out var equipmentSets);
            if (equipmentSets != null)
            {
                mission.AddListener(CreateTournamentFairArmourMissionListener(equipmentSets));
            }
        }

        private TournamentFairArmourMissionListener CreateTournamentFairArmourMissionListener(Equipment equipmentSet)
        {
            var tournamentEquipment = new Equipment();
            foreach (var overriddenEquipmentIndex in OverriddenEquipmentIndices)
            {
                tournamentEquipment[overriddenEquipmentIndex] = equipmentSet[overriddenEquipmentIndex];
            }

            return new TournamentFairArmourMissionListener(tournamentEquipment);
        }
    }
}
