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
using MegaCrit.Sts2.Core.Entities.Cards;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;

  
public class ChillTouchPower : CustomPowerModel
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
    
    public override PowerType Type => PowerType.Debuff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? _, out decimal modifiedAmount)
    {
        if (target != base.Owner)
        {
            modifiedAmount = amount;
            return false;
        }
        if (canonicalPower.GetTypeForAmount(amount) != PowerType.Debuff)
        {
            modifiedAmount = amount;
            return false;
        }
        if (!canonicalPower.IsVisible)
        {
            modifiedAmount = amount;
            return false;
        }
        modifiedAmount = amount * 2;
        return true;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power == this)
        {
            if (applier != null)
            {
                // This debuff WAS applied by a card
            }
            else
            {
                // This debuff WAS NOT applied by a card
            }
        }
        return Task.CompletedTask;   
    }

    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        await PowerCmd.Decrement(this);
    }
}