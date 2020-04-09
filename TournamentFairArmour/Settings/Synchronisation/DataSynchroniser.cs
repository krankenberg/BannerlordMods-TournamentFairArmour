using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace TournamentFairArmour.Settings.Synchronisation
{
    public class DataSynchroniser
    {
        private const string SettingsSaveKey = "TournamentFairArmourSettings";

        public void SynchroniseData(IDataStore dataStore, Dictionary<string, Equipment> equipmentsByCulture)
        {
            Settings settings = null;
            if (dataStore.IsSaving)
            {
                settings = CreateSettings(equipmentsByCulture);
            }

            dataStore.SyncData(SettingsSaveKey, ref settings);

            if (dataStore.IsLoading)
            {
                LoadFromSettings(settings, equipmentsByCulture);
            }
        }

        private Settings CreateSettings(Dictionary<string, Equipment> equipmentsByCulture)
        {
            return ConvertEquipmentDictionaryToSettings(equipmentsByCulture);
        }

        private Settings ConvertEquipmentDictionaryToSettings(Dictionary<string, Equipment> equipmentsByCulture)
        {
            var settings = new Settings();

            foreach (var entry in equipmentsByCulture)
            {
                settings.EquipmentsByCulture[entry.Key] = CreateSavedEquipment(entry.Value);
            }

            return settings;
        }

        private SavedEquipment CreateSavedEquipment(Equipment equipment)
        {
            SavedEquipment savedEquipment = new SavedEquipment();

            foreach (var overriddenEquipmentIndex in TournamentFairArmourSubModule.OverriddenEquipmentIndices)
            {
                if (!equipment[overriddenEquipmentIndex].IsEmpty)
                {
                    savedEquipment.ItemSlots[(int) overriddenEquipmentIndex] = equipment[overriddenEquipmentIndex].Item.StringId;
                }
            }

            return savedEquipment;
        }

        private void LoadFromSettings(Settings settings, Dictionary<string, Equipment> equipmentsByCulture)
        {
            if (settings != null)
            {
                foreach (var savedEquipmentSet in settings.EquipmentsByCulture)
                {
                    equipmentsByCulture[savedEquipmentSet.Key] = CreateEquipment(savedEquipmentSet.Value);
                }
            }
        }

        private Equipment CreateEquipment(SavedEquipment savedEquipment)
        {
            Equipment equipment = new Equipment();

            var itemSlots = savedEquipment.ItemSlots;
            for (var i = 0; i < itemSlots.Length; i++)
            {
                var itemObject = MBObjectManager.Instance.GetObject<ItemObject>(itemSlots[i]);
                if (itemObject != null)
                {
                    equipment[i] = new EquipmentElement(itemObject);
                }
            }

            return equipment;
        }
    }
}
