using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace TournamentFairArmour.Settings
{
    public class Settings
    {
        public Dictionary<string, SavedEquipment> EquipmentsByCulture;
        public List<string> DisabledCultureSets;
    }

    public class SavedEquipment
    {
        public string[] ItemSlots;
    }
}
