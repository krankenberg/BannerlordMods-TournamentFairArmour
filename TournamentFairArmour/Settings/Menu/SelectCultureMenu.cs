using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TournamentFairArmour.Settings.Menu
{
    public class SelectCultureMenu : AbstractMenu
    {
        private const string MenuDescriptionText = "Select the culture, you want to configure the armour for. " +
                                                   "If a culture has no specific set, the default set will be used.\n \n" +
                                                   "Hover over a culture to see total armour.";

        private const string DisableStatusMenuTextKeyPrefix = TextKeyPrefix + "DISABLE_STATUS_";

        private SelectEquipmentIndexMenu _selectEquipmentIndexMenu;

        public SelectCultureMenu(Context context) : base(context, "select_culture_menu", MenuDescriptionText)
        {
        }

        protected override void AddMenuItems(CampaignGameStarter campaignGameStarter)
        {
            CreateSelectCultureMenuItem(campaignGameStarter, SubModule.DefaultEquipmentSetStringId, SubModule.DefaultEquipmentSetStringName);
            CreateMenuItemsForAllCultures(campaignGameStarter);
        }

        protected override void AfterCreateMenu(CampaignGameStarter campaignGameStarter)
        {
            base.AfterCreateMenu(campaignGameStarter);
            _selectEquipmentIndexMenu = new SelectEquipmentIndexMenu(Context);
            _selectEquipmentIndexMenu.CreateMenu(campaignGameStarter);
        }

        private void CreateMenuItemsForAllCultures(CampaignGameStarter campaignGameStarter)
        {
            ObjectManagerUtils.GetAllMainCultureObjects()
                .ForEach(cultureObject => CreateSelectCultureMenuItem(campaignGameStarter, cultureObject.StringId, cultureObject.Name.ToString()));
            CreateMenuItemForReset(campaignGameStarter);
        }

        private void CreateSelectCultureMenuItem(CampaignGameStarter campaignGameStarter, string cultureStringId, string cultureName)
        {
            campaignGameStarter.AddGameMenuOption(
                MenuId,
                GetMenuItemId(cultureStringId),
                cultureName + "{" + DisableStatusMenuTextKeyPrefix + cultureStringId + "}",
                menuCallbackArgs => InitialiseMenuItem(cultureStringId, menuCallbackArgs),
                menuCallbackArgs => SwitchToEquipmentPartChoiceMenu(cultureStringId, cultureName)
            );
        }

        protected override void OnOpenMenu(MenuCallbackArgs menuCallbackArgs)
        {
            GameTexts.SetVariable(DisableStatusMenuTextKeyPrefix + SubModule.DefaultEquipmentSetStringId, "");
            ObjectManagerUtils.GetAllMainCultureObjects()
                .ForEach(cultureObject =>
                {
                    var cultureId = cultureObject.StringId;
                    GameTexts.SetVariable(DisableStatusMenuTextKeyPrefix + cultureId,
                        Context.SettingsCampaignBehaviour.IsDisabled(cultureId) ? " (disabled)" : "");
                });
        }

        private bool InitialiseMenuItem(string cultureStringId, MenuCallbackArgs menuCallbackArgs)
        {
            var equipment = Context.SettingsCampaignBehaviour.GetEquipment(cultureStringId);
            menuCallbackArgs.Tooltip = new TextObject(ObjectManagerUtils.BuildTooltipString(equipment));
            menuCallbackArgs.optionLeaveType = GameMenuOption.LeaveType.Recruit;
            return true;
        }

        private void SwitchToEquipmentPartChoiceMenu(string cultureStringId, string cultureName)
        {
            Context.CurrentlySelectedCultureId = cultureStringId;
            Context.CurrentlySelectedCultureName = cultureName;
            _selectEquipmentIndexMenu.Open(MenuId);
        }

        private void CreateMenuItemForReset(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption(
                MenuId,
                GetMenuItemId("reset"),
                "Reset",
                menuCallbackArgs => ConfigureIconAndReturnMenuItemVisible(menuCallbackArgs, GameMenuOption.LeaveType.Escape),
                menuCallbackArgs => InquireForEquipmentReset()
            );
        }

        private void InquireForEquipmentReset()
        {
            InformationManager.ShowInquiry(new InquiryData(
                "Reset all sets",
                "Do you really want to reset all sets?",
                true,
                true,
                "Reset",
                "Cancel",
                ResetEquipment,
                () => { }
            ));
        }

        private void ResetEquipment()
        {
            Context.SettingsCampaignBehaviour.ResetAllEquipmentSets();
            GameMenu.SwitchToMenu(MenuId);
            InformationManager.DisplayMessage(new InformationMessage("All sets were reset."));
        }
    }
}
