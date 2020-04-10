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
        internal const string DefaultEquipmentSetStringName = "Default";
        internal const string NoItemStringId = "none";
        internal const string NoItemStringName = "None";

        internal static readonly EquipmentIndex[] OverriddenEquipmentIndices =
        {
            EquipmentIndex.Head,
            EquipmentIndex.Cape,
            EquipmentIndex.Body,
            EquipmentIndex.Gloves,
            EquipmentIndex.Leg
        };

        private readonly DefaultEquipmentLoader _defaultEquipmentLoader;
        private SettingsCampaignBehaviour _settingsCampaignBehaviour;

        public SubModule()
        {
            _defaultEquipmentLoader = new DefaultEquipmentLoader();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignGameStarter = (CampaignGameStarter) gameStarterObject;
                _settingsCampaignBehaviour = new SettingsCampaignBehaviour(new DataSynchroniser());
                campaignGameStarter.AddBehavior(_settingsCampaignBehaviour);
            }
        }

        public override void OnGameInitializationFinished(Game game)
        {
            if (game.GameType is Campaign)
            {
                _settingsCampaignBehaviour.DefaultEquipmentsByCulture = _defaultEquipmentLoader.LoadDefaultEquipmentsByCulture();
            }
        }

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            if (mission.HasMissionBehaviour<TournamentBehavior>())
            {
                AddOverrideSpawnArmourMissionListener(mission);
            }
        }

        private void AddOverrideSpawnArmourMissionListener(Mission mission)
        {
            var equipment = GetEquipmentByCultureOfCurrentSettlement();
            if (equipment != null)
            {
                mission.AddListener(new OverrideSpawnArmourMissionListener(equipment));
            }
        }

        private Equipment GetEquipmentByCultureOfCurrentSettlement()
        {
            var cultureStringId = Settlement.CurrentSettlement?.Culture?.StringId;
            return _settingsCampaignBehaviour.GetEquipment(cultureStringId);
        }
    }
}
