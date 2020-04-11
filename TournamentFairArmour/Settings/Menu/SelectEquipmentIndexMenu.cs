using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TournamentFairArmour.Settings.Menu
{
    public class SelectEquipmentIndexMenu : AbstractMenu
    {
        internal const string CultureNameMenuTextKey = TextKeyPrefix + "CULTURE";
        private const string CurrentEquipmentMenuTextKeyPrefix = TextKeyPrefix + "CURRRENT_";
        private const string TotalArmourMenuTextKey = TextKeyPrefix + "TOTAL_ARMOUR";
        private const string ToggleDisableMenuTextKey = TextKeyPrefix + "TOGGLE_DISABLE";

        private const string MenuDescriptionText = "Configure Armour for {" + CultureNameMenuTextKey + "}-set.\n \n" +
                                                   "Total Armour:\n" +
                                                   "{" + TotalArmourMenuTextKey + "}";

        private readonly Dictionary<EquipmentIndex, string> _equipmentIndexNames = new Dictionary<EquipmentIndex, string>
        {
            {EquipmentIndex.Head, "Head"},
            {EquipmentIndex.Cape, "Cape"},
            {EquipmentIndex.Body, "Body"},
            {EquipmentIndex.Gloves, "Gloves"},
            {EquipmentIndex.Leg, "Leg"}
        };

        private SelectEquipmentElementMenu _selectEquipmentElementMenu;

        public SelectEquipmentIndexMenu(Context context) : base(context, "select_equipment_index_menu", MenuDescriptionText)
        {
        }

        protected override void AddMenuItems(CampaignGameStarter campaignGameStarter)
        {
            foreach (var equipmentIndexEntry in _equipmentIndexNames)
            {
                CreateMenuItemForEquipmentChoice(campaignGameStarter, equipmentIndexEntry.Key);
            }

            CreateMenuItemForEquipmentReset(campaignGameStarter);
            CreateMenuItemForToggleDisableCultureSet(campaignGameStarter);
        }

        protected override void AfterCreateMenu(CampaignGameStarter campaignGameStarter)
        {
            base.AfterCreateMenu(campaignGameStarter);
            _selectEquipmentElementMenu = new SelectEquipmentElementMenu(Context);
            _selectEquipmentElementMenu.CreateMenu(campaignGameStarter);
        }

        protected override void OnOpenMenu(MenuCallbackArgs menuCallbackArgs)
        {
            GameTexts.SetVariable(CultureNameMenuTextKey, Context.CurrentlySelectedCultureName);
            foreach (var equipmentIndexNameEntry in _equipmentIndexNames)
            {
                var equipmentElement = Context.SettingsCampaignBehaviour.GetEquipmentSlot(Context.CurrentlySelectedCultureId, equipmentIndexNameEntry.Key);
                var currentItemName = equipmentElement.IsEmpty ? SubModule.NoItemStringName : equipmentElement.GetModifiedItemName().ToString();
                GameTexts.SetVariable(CurrentEquipmentMenuTextKeyPrefix + equipmentIndexNameEntry.Value, currentItemName);
            }

            var equipment = Context.SettingsCampaignBehaviour.GetEquipment(Context.CurrentlySelectedCultureId);
            var totalArmourString = ObjectManagerUtils.BuildTooltipString(equipment);
            GameTexts.SetVariable(TotalArmourMenuTextKey, totalArmourString);
            GameTexts.SetVariable(ToggleDisableMenuTextKey, GetDisableToggleText());
        }

        private string GetDisableToggleText()
        {
            return Context.SettingsCampaignBehaviour.IsDisabled(Context.CurrentlySelectedCultureId) ? "Enable" : "Disable";
        }

        private void CreateMenuItemForEquipmentChoice(CampaignGameStarter campaignGameStarter, EquipmentIndex equipmentIndex)
        {
            string equipmentPartIdAsString = ((int) equipmentIndex).ToString();
            var equipmentIndexName = _equipmentIndexNames[equipmentIndex];
            campaignGameStarter.AddGameMenuOption(
                MenuId,
                GetMenuItemId(equipmentPartIdAsString),
                $"{equipmentIndexName} ({{{CurrentEquipmentMenuTextKeyPrefix + equipmentIndexName}}})",
                menuCallbackArgs => InitialiseMenuItem(equipmentIndex, menuCallbackArgs),
                menuCallbackArgs => SwitchToEquipmentChoiceMenu(equipmentIndex)
            );
        }

        private bool InitialiseMenuItem(EquipmentIndex equipmentIndex, MenuCallbackArgs menuCallbackArgs)
        {
            var equipmentElement = Context.SettingsCampaignBehaviour.GetEquipmentSlot(Context.CurrentlySelectedCultureId, equipmentIndex);
            if (!equipmentElement.IsEmpty)
            {
                menuCallbackArgs.Tooltip = new TextObject(ObjectManagerUtils.BuildTooltipString(equipmentElement));
            }

            menuCallbackArgs.optionLeaveType = GameMenuOption.LeaveType.HostileAction;
            return true;
        }

        private void CreateMenuItemForEquipmentReset(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption(
                MenuId,
                GetMenuItemId("reset"),
                "Reset",
                menuCallbackArgs => ConfigureIconAndReturnMenuItemVisible(menuCallbackArgs, GameMenuOption.LeaveType.Escape),
                menuCallbackArgs => InquireForEquipmentReset()
            );
        }

        private void CreateMenuItemForToggleDisableCultureSet(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption(
                MenuId,
                GetMenuItemId("disable"),
                "{" + ToggleDisableMenuTextKey + "}",
                IsToggleDisableMenuItemVisible,
                menuCallbackArgs => ToggleDisableStatus()
            );
        }

        private bool IsToggleDisableMenuItemVisible(MenuCallbackArgs menuCallbackArgs)
        {
            menuCallbackArgs.optionLeaveType = GameMenuOption.LeaveType.Wait;
            return Context.CurrentlySelectedCultureId != SubModule.DefaultEquipmentSetStringId;
        }

        private void ToggleDisableStatus()
        {
            Context.SettingsCampaignBehaviour.ToggleDisableStatus(Context.CurrentlySelectedCultureId);
            GameMenu.SwitchToMenu(MenuId);
        }

        private void InquireForEquipmentReset()
        {
            InformationManager.ShowInquiry(new InquiryData(
                $"Reset {Context.CurrentlySelectedCultureName}",
                $"Do you really want to reset the {Context.CurrentlySelectedCultureName}-set?",
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
            Context.SettingsCampaignBehaviour.ResetEquipmentSet(Context.CurrentlySelectedCultureId);
            GameMenu.SwitchToMenu(MenuId);
            InformationManager.DisplayMessage(new InformationMessage($"{Context.CurrentlySelectedCultureName}-set was reset."));
        }

        private void SwitchToEquipmentChoiceMenu(EquipmentIndex equipmentIndex)
        {
            Context.CurrentlySelectedEquipmentIndex = equipmentIndex;
            Context.CurrentlySelectedEquipmentIndexName = _equipmentIndexNames[equipmentIndex];
            _selectEquipmentElementMenu.Open(MenuId);
        }
    }
}
