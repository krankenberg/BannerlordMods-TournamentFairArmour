using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TournamentFairArmour.Settings.Synchronisation
{
    public class SettingsSaveDefiner : SaveableTypeDefiner
    {
        public SettingsSaveDefiner() : base(-1889155762)
        {
        }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(Settings), -1889155762);
            AddClassDefinition(typeof(SavedEquipment), -1889155761);
        }

        protected override void DefineContainerDefinitions()
        {
            ConstructContainerDefinition(typeof(Dictionary<string, SavedEquipment>));
        }
    }
}
