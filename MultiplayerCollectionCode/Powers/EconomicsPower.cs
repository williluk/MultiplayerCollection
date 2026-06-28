using BaseLib.Abstracts;
using BaseLib.Extensions;
using MultiplayerCollection.MultiplayerCollectionCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Runs;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;


public class EconomicsPower : CustomPowerModel
{
    //Loads from MutiplayerCollection/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
        
    private class Data
    {
        public int starsGained;

        public int triggerCount;
    }

    public override PowerType Type => PowerType.Debuff; 
    public override PowerStackType StackType => PowerStackType.Counter;
        
    // CODE GOES HERE

    public override int DisplayAmount => base.Amount - GetInternalData<Data>().starsGained % base.Amount;
    
    protected override object InitInternalData()
    {
        return new Data();
    }

    public override async Task AfterStarsGained(int amount, Player gainer)
    {
        if (gainer == base.Owner.Player)
        {
            Data data = GetInternalData<Data>();
            data.starsGained += 1;
            int triggers = (data.starsGained / base.Amount) - data.triggerCount;
            if (triggers > 0)
            {
                Flash();
                IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner) where c.IsAlive && c.IsPlayer && c != base.Owner select c;
                foreach (Creature item in enumerable)
                {
                    await PlayerCmd.GainEnergy(1 * triggers, item.Player);
                }
                data.triggerCount += triggers;
            }
            InvokeDisplayAmountChanged();
        }
    }
}