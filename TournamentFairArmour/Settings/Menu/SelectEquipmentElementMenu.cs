using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TournamentFairArmour.Settings.Menu
{
    public class SelectEquipmentElementMenu : AbstractMenu
    {
        private const string EquipmentIndexNameMenuTextKey = TextKeyPrefix + "EQUIPMENT_INDEX_NAME";

        private const string MenuDescriptionText =
            "Select {" + EquipmentIndexNameMenuTextKey + "} for {" + SelectEquipmentIndexMenu.CultureNameMenuTextKey + "}-set\n \n" +
            "Hover over item to see armour.\n \n" +
            "Items are sorted by armour ascending and by total armour descending.";

        public SelectEquipmentElementMenu(Context context) : base(context, "select_equipment_element_menu", MenuDescriptionText)
        {
        }

        protected override void AddMenuItems(CampaignGameStarter campaignGameStarter)
        {
            CreateMenuItemForNone(campaignGameStarter);
            foreach (var equipmentIndex in SubModule.OverriddenEquipmentIndices)
            {
                string currentCultureId = null;
                var equipmentElements = ObjectManagerUtils.GetAllEquipmentElements(equipmentIndex);
                foreach (var equipmentElement in equipmentElements)
                {
                    string cultureId = ObjectManagerUtils.GetCultureStringId(equipmentElement);
                    if (currentCultureId != cultureId)
                    {
                        CreateMenuItemForCultureSeparator(campaignGameStarter, equipmentIndex, cultureId, ObjectManagerUtils.GetCultureName(equipmentElement));
                        currentCultureId = cultureId;
                    }

                    CreateMenuItemForEquipmentElement(campaignGameStarter, equipmentElement, equipmentIndex);
                }
            }
        }

        protected override void OnOpenMenu(MenuCallbackArgs menuCallbackArgs)
        {
            GameTexts.SetVariable(EquipmentIndexNameMenuTextKey, Context.CurrentlySelectedEquipmentIndexName);
        }

        private void CreateMenuItemForNone(CampaignGameStarter campaignGameStarter)
        {
            campaignGameStarter.AddGameMenuOption(
                MenuId,
                GetMenuItemId(SubModule.NoItemStringId),
                SubModule.NoItemStringName,
                menuCallbackArgs => ConfigureIconAndReturnMenuItemVisible(menuCallbackArgs, GameMenuOption.LeaveType.RansomAndBribe),
                menuCallbackArgs => SelectEquipmentElement(EquipmentElement.Invalid)
            );
        }

        private void CreateMenuItemForCultureSeparator(CampaignGameStarter campaignGameStarter, EquipmentIndex equipmentIndex,
            string cultureId, string cultureName)
        {
            campaignGameStarter.AddGameMenuOption(
                MenuId,
                GetMenuItemId((int) equipmentIndex + cultureId),
                cultureName,
                menuCallbackArgs => IsMenuSeparatorVisible(equipmentIndex, menuCallbackArgs),
                menuCallbackArgs => { }
            );
        }

        private void CreateMenuItemForEquipmentElement(CampaignGameStarter campaignGameStarter, EquipmentElement equipmentElement,
            EquipmentIndex equipmentIndex)
        {
            campaignGameStarter.AddGameMenuOption(
                MenuId,
                GetMenuItemId(equipmentElement.Item.StringId),
                equipmentElement.GetModifiedItemName().ToString(),
                menuCallbackArgs => IsMenuItemVisible(equipmentIndex, equipmentElement, menuCallbackArgs),
                menuCallbackArgs => SelectEquipmentElement(equipmentElement)
            );
        }

        private void SelectEquipmentElement(EquipmentElement equipmentElement)
        {
            Context.SettingsCampaignBehaviour
                .SetEquipmentElement(Context.CurrentlySelectedCultureId, Context.CurrentlySelectedEquipmentIndex, equipmentElement);
            LeaveMenu();
        }

        private bool IsMenuSeparatorVisible(EquipmentIndex equipmentIndex, MenuCallbackArgs menuCallbackArgs)
        {
            menuCallbackArgs.optionLeaveType = GameMenuOption.LeaveType.Recruit;
            return Context.CurrentlySelectedEquipmentIndex == equipmentIndex;
        }

        private bool IsMenuItemVisible(EquipmentIndex equipmentIndex, EquipmentElement equipmentElement, MenuCallbackArgs menuCallbackArgs)
        {
            menuCallbackArgs.Tooltip = new TextObject(ObjectManagerUtils.BuildTooltipString(equipmentElement));
            return Context.CurrentlySelectedEquipmentIndex == equipmentIndex;
        }
    }
}
