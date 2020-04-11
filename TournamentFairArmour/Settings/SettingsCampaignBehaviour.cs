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
        private List<string> _disabledCultures = new List<string>();

        private readonly DataSynchroniser _dataSynchroniser;

        public SettingsCampaignBehaviour(DataSynchroniser dataSynchroniser)
        {
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
            var settingsMenu = new SettingsMenu(this);
            settingsMenu.CreateMenu(campaignGameStarter);
        }

        public EquipmentElement GetEquipmentSlot(string cultureStringId, EquipmentIndex equipmentIndex)
        {
            return GetEquipment(cultureStringId)[equipmentIndex];
        }

        public Equipment GetEquipmentOrDefaultIfDisabled(string cultureStringId)
        {
            if (IsDisabled(cultureStringId))
            {
                _equipmentsByCulture.TryGetValue(SubModule.DefaultEquipmentSetStringId, out var equipment);
                if (equipment == null)
                {
                    DefaultEquipmentsByCulture.TryGetValue(SubModule.DefaultEquipmentSetStringId, out equipment);
                }

                return equipment;
            }

            return GetEquipment(cultureStringId);
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

        public void SetEquipmentElement(string cultureStringId, EquipmentIndex equipmentIndex, EquipmentElement equipmentElement)
        {
            if (!_equipmentsByCulture.ContainsKey(cultureStringId))
            {
                _equipmentsByCulture[cultureStringId] = DefaultEquipmentsByCulture.ContainsKey(cultureStringId)
                    ? DefaultEquipmentsByCulture[cultureStringId].Clone()
                    : DefaultEquipmentsByCulture[SubModule.DefaultEquipmentSetStringId];
            }

            _equipmentsByCulture[cultureStringId][equipmentIndex] = equipmentElement;
        }

        public void ResetEquipmentSet(string cultureStringId)
        {
            _equipmentsByCulture.Remove(cultureStringId);
            _disabledCultures.Remove(cultureStringId);
        }

        public void ResetAllEquipmentSets()
        {
            _equipmentsByCulture.Clear();
            _disabledCultures.Clear();
        }

        public bool IsDisabled(string cultureId)
        {
            return _disabledCultures.Contains(cultureId);
        }

        public void ToggleDisableStatus(string cultureId)
        {
            if (IsDisabled(cultureId))
            {
                _disabledCultures.Remove(cultureId);
            }
            else
            {
                _disabledCultures.Add(cultureId);
            }
        }
    }
}
