using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;

namespace TournamentFairArmour.Settings.Menu
{
    public abstract class AbstractMenu
    {
        protected const string TextKeyPrefix = "TOURNAMENT_FAIR_ARMOUR_SETTINGS_";
        private const string LeaveDisplayText = "Back";
        private const string MenuItemPrefix = "tournament_fair_armour_settings_";
        private const string LeaveMenuItemIdSuffix = "leave";

        protected Context Context;

        protected string MenuId;
        private string _descriptionText;

        protected AbstractMenu(Context context, string menuId, string descriptionText)
        {
            Context = context;
            MenuId = "menu_" + MenuItemPrefix + menuId;
            _descriptionText = descriptionText;
        }

        protected abstract void AddMenuItems(CampaignGameStarter campaignGameStarter);

        protected virtual void AfterCreateMenu(CampaignGameStarter campaignGameStarter)
        {
        }

        internal void CreateMenu(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenu(
                MenuId,
                _descriptionText,
                OnOpenMenu,
                GameOverlays.MenuOverlayType.SettlementWithBoth);

            AddMenuItems(campaignGameStarter);
            AddLeaveMenuItem(campaignGameStarter);

            AfterCreateMenu(campaignGameStarter);
        }

        protected virtual void OnOpenMenu(MenuCallbackArgs menuCallbackArgs)
        {
        }

        internal void Open(string currentMenuId)
        {
            GameMenu.SwitchToMenu(MenuId);
            Context.MenuIdStack.Push(currentMenuId);
        }

        protected void LeaveMenu()
        {
            GameMenu.SwitchToMenu(Context.MenuIdStack.Pop());
        }

        internal static bool ConfigureIconAndReturnMenuItemVisible(MenuCallbackArgs menuCallbackArgs, GameMenuOption.LeaveType optionLeaveType)
        {
            menuCallbackArgs.optionLeaveType = optionLeaveType;
            return true;
        }

        private void AddLeaveMenuItem(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption(
                MenuId,
                GetMenuItemId(LeaveMenuItemIdSuffix),
                LeaveDisplayText,
                menuCallbackArgs => ConfigureIconAndReturnMenuItemVisible(menuCallbackArgs, GameMenuOption.LeaveType.Leave),
                menuCallbackArgs => LeaveMenu()
            );
        }

        protected string GetMenuItemId(string suffix)
        {
            return $"{MenuItemPrefix}{MenuId}_{suffix}";
        }
    }
}
