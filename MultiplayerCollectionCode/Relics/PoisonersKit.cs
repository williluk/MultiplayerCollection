using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MultiplayerCollection.MultiplayerCollectionCode.Relics;

[Pool(typeof(SilentRelicPool))]
public class PoisonersKit() : CustomRelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    public override bool IsAllowed(IRunState runState)
    {
        return RelicModel.IsBeforeAct3TreasureChest(runState);
    }
    
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player == base.Owner)
        {
            return false;
        }
        options.Add(new PoisonRestSiteOption(player));
        return true;
    }
    
}