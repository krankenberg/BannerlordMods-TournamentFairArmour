using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TournamentFairArmour.Settings.Synchronisation;

namespace TournamentFairArmour.Settings
{
    public class SettingsCampaignBehaviour : CampaignBehaviorBase
    {
        public Dictionary<string, Equipment> DefaultEquipmentsByCulture = new Dictionary<string, Equipment>();

        private Dictionary<string, Equipment> _equipmentsByCulture = new Dictionary<string, Equipment>();

        private readonly Campaign _campaign;

        private readonly DataSynchroniser _dataSynchroniser;

        public SettingsCampaignBehaviour(Campaign campaign, DataSynchroniser dataSynchroniser)
        {
            _campaign = campaign;
            _dataSynchroniser = dataSynchroniser;
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
        }

        public override void SyncData(IDataStore dataStore)
        {
            _dataSynchroniser.SynchroniseData(dataStore, _equipmentsByCulture);
        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            var settingsMenu = new SettingsMenu(_campaign);
            settingsMenu.CreateMenu(campaignGameStarter);
        }

        public Equipment GetEquipment(string cultureStringId)
        {
            Equipment equipment = TryGetEquipment(_equipmentsByCulture, cultureStringId) ?? TryGetEquipment(DefaultEquipmentsByCulture, cultureStringId);
            return equipment;
        }

        private Equipment TryGetEquipment(Dictionary<string, Equipment> dictionary, string cultureStringId)
        {
            dictionary.TryGetValue(cultureStringId ?? string.Empty, out var equipment);
            if (equipment == null)
            {
                dictionary.TryGetValue(SubModule.DefaultEquipmentSetStringId, out equipment);
            }

            return equipment;
        }
    }
}
