using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace TournamentFairArmour
{
    public class ObjectManagerUtils
    {
        public static List<CultureObject> GetAllMainCultureObjects()
        {
            return MBObjectManager.Instance.GetObjectTypeList<CultureObject>()
                .Where(cultureObject => cultureObject.IsMainCulture)
                .OrderBy(cultureObject => cultureObject.Name.ToString())
                .ToList();
        }
    }
}
