using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TournamentFairArmour.Settings.Menu;

namespace TournamentFairArmour.Settings
{
    public class SettingsMenu
    {
        private const string TownArenaMenuStringId = "town_arena";

        private const string TournamentFairArmourSettings = "_tournament_fair_armour_settings";

        private const string ConfigureTournamentSettingsMenuOptionStringId = TownArenaMenuStringId + TournamentFairArmourSettings;

        private const string ConfigureTournamentSettingsMenuItemDisplayText = "Configure Tournament Armour Settings";

        private SelectCultureMenu _selectCultureMenu;
        private readonly Context _context;

        public SettingsMenu(SettingsCampaignBehaviour settingsCampaignBehaviour)
        {
            _context = new Context(settingsCampaignBehaviour);
        }

        public void CreateMenu(CampaignGameStarter campaignGameStarter)
        {
            var townArenaMenu = GetMenu(TownArenaMenuStringId);
            campaignGameStarter.AddGameMenuOption(
                TownArenaMenuStringId,
                ConfigureTournamentSettingsMenuOptionStringId,
                ConfigureTournamentSettingsMenuItemDisplayText,
                menuCallbackArgs => AbstractMenu.ConfigureIconAndReturnMenuItemVisible(menuCallbackArgs, GameMenuOption.LeaveType.Submenu),
                OpenSelectCultureMenu,
                index: townArenaMenu.MenuOptions.Count() - 1
            );
            _selectCultureMenu = new SelectCultureMenu(_context);
            _selectCultureMenu.CreateMenu(campaignGameStarter);
        }

        private void OpenSelectCultureMenu(MenuCallbackArgs menuCallbackArgs)
        {
            _selectCultureMenu.Open(TownArenaMenuStringId);
        }

        private GameMenu GetMenu(string menuId)
        {
            var nextGameMenuId = Campaign.Current.GameMenuManager.NextGameMenuId;
            Campaign.Current.GameMenuManager.SetNextMenu(menuId);
            var townArenaMenu = Campaign.Current.GameMenuManager.NextMenu;
            Campaign.Current.GameMenuManager.SetNextMenu(nextGameMenuId);
            return townArenaMenu;
        }
    }
}
