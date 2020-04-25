using System.Collections.Generic;
using Newtonsoft.Json;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TournamentFairArmour.Settings.Synchronisation
{
    public class DataSynchroniser
    {
        private const string SettingsSaveKey = "TournamentFairArmourSettings";

        public void SynchroniseData(IDataStore dataStore, Dictionary<string, Equipment> equipmentsByCulture, List<string> disabledCultures)
        {
            string settingsAsJson = null;
            if (dataStore.IsSaving)
            {
                Settings settings = CreateSettings(equipmentsByCulture, disabledCultures);

                settingsAsJson = JsonConvert.SerializeObject(settings);
            }

            dataStore.SyncData(SettingsSaveKey, ref settingsAsJson);

            if (dataStore.IsLoading)
            {
                if (settingsAsJson != null)
                {
                    Settings settings = JsonConvert.DeserializeObject<Settings>(settingsAsJson);
                    LoadFromSettings(settings, equipmentsByCulture, disabledCultures);
                }
            }
        }

        private Settings CreateSettings(Dictionary<string, Equipment> equipmentsByCulture, List<string> disabledCultures)
        {
            var settings = ConvertEquipmentDictionaryToSettings(equipmentsByCulture);
            settings.DisabledCultureSets = disabledCultures;
            return settings;
        }

        private Settings ConvertEquipmentDictionaryToSettings(Dictionary<string, Equipment> equipmentsByCulture)
        {
            var settings = new Settings();

            if (equipmentsByCulture.Count > 0)
            {
                settings.EquipmentsByCulture = new Dictionary<string, SavedEquipment>();

                foreach (var entry in equipmentsByCulture)
                {
                    settings.EquipmentsByCulture[entry.Key] = CreateSavedEquipment(entry.Value);
                }
            }

            return settings;
        }

        private SavedEquipment CreateSavedEquipment(Equipment equipment)
        {
            SavedEquipment savedEquipment = new SavedEquipment();
            savedEquipment.ItemSlots = new string[(int) EquipmentIndex.NumEquipmentSetSlots];

            foreach (var overriddenEquipmentIndex in SubModule.OverriddenEquipmentIndices)
            {
                if (!equipment[overriddenEquipmentIndex].IsEmpty)
                {
                    savedEquipment.ItemSlots[(int) overriddenEquipmentIndex] = equipment[overriddenEquipmentIndex].Item.StringId;
                }
            }

            return savedEquipment;
        }

        private void LoadFromSettings(Settings settings, Dictionary<string, Equipment> equipmentsByCulture, List<string> disabledCultures)
        {
            if (settings.EquipmentsByCulture != null)
            {
                foreach (var savedEquipmentSet in settings.EquipmentsByCulture)
                {
                    equipmentsByCulture[savedEquipmentSet.Key] = CreateEquipment(savedEquipmentSet.Value);
                }
            }

            if (settings.DisabledCultureSets != null)
            {
                disabledCultures.Clear();
                disabledCultures.AddRange(settings.DisabledCultureSets);
            }
        }

        private Equipment CreateEquipment(SavedEquipment savedEquipment)
        {
            Equipment equipment = new Equipment();

            var itemSlots = savedEquipment.ItemSlots;
            for (var i = 0; i < itemSlots.Length; i++)
            {
                var itemSlot = itemSlots[i];
                if (itemSlot != null)
                {
                    var itemObject = MBObjectManager.Instance.GetObject<ItemObject>(itemSlot);
                    if (itemObject != null)
                    {
                        equipment[i] = new EquipmentElement(itemObject);
                    }
                }
            }

            return equipment;
        }
    }
}
