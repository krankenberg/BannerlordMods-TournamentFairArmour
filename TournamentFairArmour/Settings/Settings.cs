using System.Collections.Generic;
using TaleWorlds.SaveSystem;

namespace TournamentFairArmour.Settings
{
    public class Settings
    {
        [SaveableField(0)] public Dictionary<string, SavedEquipment> EquipmentsByCulture = new Dictionary<string, SavedEquipment>();
    }

    public class SavedEquipment
    {
        [SaveableField(0)] public string[] ItemSlots;
    }
}
