using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace TournamentFairArmour.Settings
{
    public class Settings
    {
        [SaveableField(0)] public readonly Dictionary<string, SavedEquipment> EquipmentsByCulture = new Dictionary<string, SavedEquipment>();
    }

    public class SavedEquipment
    {
        [SaveableField(0)] public readonly string[] ItemSlots = new string[(int) EquipmentIndex.NumEquipmentSetSlots];
    }
}
