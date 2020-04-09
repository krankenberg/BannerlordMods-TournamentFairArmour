using System.Collections.Generic;
using System.Linq;
using SandBox.TournamentMissions.Missions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TournamentFairArmour.Settings;
using TournamentFairArmour.Settings.Synchronisation;

namespace TournamentFairArmour
{
    public class SubModule : MBSubModuleBase
    {
        internal const string DefaultEquipmentSetStringId = "default";
        private const string StringIdPrefix = "tournament_fair_armour_equipment_override_";

        internal static readonly EquipmentIndex[] OverriddenEquipmentIndices =
        {
            EquipmentIndex.Head,
            EquipmentIndex.Cape,
            EquipmentIndex.Body,
            EquipmentIndex.Gloves,
            EquipmentIndex.Leg
        };

        private SettingsCampaignBehaviour _settingsCampaignBehaviour;

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (game.GameType is Campaign campaign)
            {
                CampaignGameStarter campaignGameStarter = (CampaignGameStarter) gameStarterObject;
                _settingsCampaignBehaviour = new SettingsCampaignBehaviour(campaign, new DataSynchroniser());
                campaignGameStarter.AddBehavior(_settingsCampaignBehaviour);
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            if (game.GameType is Campaign)
            {
                var defaultEquipmentsByCulture = new Dictionary<string, Equipment>();
                AddDefaultEquipmentOverride(defaultEquipmentsByCulture);
                var allCharacterObjects = MBObjectManager.Instance.GetObjectTypeList<CharacterObject>();
                allCharacterObjects
                    .Where(characterObject => characterObject.StringId.StartsWith(StringIdPrefix))
                    .ToList()
                    .ForEach(characterObject => AddEquipmentOverride(characterObject, defaultEquipmentsByCulture));
                _settingsCampaignBehaviour.DefaultEquipmentsByCulture = defaultEquipmentsByCulture;
            }
        }

        private void AddDefaultEquipmentOverride(Dictionary<string, Equipment> equipmentSetByCulture)
        {
            var characterObject = MBObjectManager.Instance.GetObject<CharacterObject>(StringIdPrefix + DefaultEquipmentSetStringId);
            equipmentSetByCulture[DefaultEquipmentSetStringId] = characterObject.BattleEquipments.First();
        }

        private void AddEquipmentOverride(CharacterObject characterObject, Dictionary<string, Equipment> equipmentSetByCulture)
        {
            if (characterObject.Culture != null)
            {
                equipmentSetByCulture[characterObject.Culture.StringId] = characterObject.BattleEquipments.First();
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
            var equipment = GetEquipmentByCultureOfCurrentSettlement();
            if (equipment != null)
            {
                mission.AddListener(CreateTournamentFairArmourMissionListener(equipment));
            }
        }

        private Equipment GetEquipmentByCultureOfCurrentSettlement()
        {
            var cultureStringId = Settlement.CurrentSettlement?.Culture?.StringId;
            return _settingsCampaignBehaviour.GetEquipment(cultureStringId);
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
