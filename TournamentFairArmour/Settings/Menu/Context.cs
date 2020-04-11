using System.Collections.Generic;
using TaleWorlds.Core;

namespace TournamentFairArmour.Settings.Menu
{
    public class Context
    {
        public readonly SettingsCampaignBehaviour SettingsCampaignBehaviour;

        public readonly Stack<string> MenuIdStack = new Stack<string>();

        public string CurrentlySelectedCultureId;

        public string CurrentlySelectedCultureName;

        public EquipmentIndex CurrentlySelectedEquipmentIndex;

        public string CurrentlySelectedEquipmentIndexName;

        public Context(SettingsCampaignBehaviour settingsCampaignBehaviour)
        {
            SettingsCampaignBehaviour = settingsCampaignBehaviour;
        }
    }
}
