using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;

namespace TournamentFairArmour.Settings
{
    public class SettingsMenu
    {
        private const string TownArenaMenuStringId = "town_arena";
        private const string TownArenaTournamentFairArmourSettingsMenuStringId = TownArenaMenuStringId + "_tournament_fair_armour_settings";
        private const string ConfigureTournamentOptionsDisplayText = "Configure Tournament options...";

        private readonly Campaign _campaign;

        public SettingsMenu(Campaign campaign)
        {
            _campaign = campaign;
        }

        public void CreateMenu(CampaignGameStarter campaignGameStarter)
        {
            var townArenaMenu = GetTownArenaMenu();
            campaignGameStarter.AddGameMenuOption(
                TownArenaMenuStringId,
                TownArenaTournamentFairArmourSettingsMenuStringId,
                ConfigureTournamentOptionsDisplayText,
                ConfigureIconAndReturnMenuItemVisibility,
                OnMenuClick,
                index: townArenaMenu.MenuOptions.Count() - 1
            );
        }

        private GameMenu GetTownArenaMenu()
        {
            var nextGameMenuId = _campaign.GameMenuManager.NextGameMenuId;
            _campaign.GameMenuManager.SetNextMenu(TownArenaMenuStringId);
            var townArenaMenu = _campaign.GameMenuManager.NextMenu;
            _campaign.GameMenuManager.SetNextMenu(nextGameMenuId);
            return townArenaMenu;
        }

        private bool ConfigureIconAndReturnMenuItemVisibility(MenuCallbackArgs menuCallBackArgs)
        {
            menuCallBackArgs.optionLeaveType = GameMenuOption.LeaveType.Submenu;
            return true;
        }

        private void OnMenuClick(MenuCallbackArgs args)
        {
            InformationManager.DisplayMessage(new InformationMessage("OnMenuClick"));
        }
    }
}
