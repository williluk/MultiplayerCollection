using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
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


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;


public class RoyalArmsPower : CustomPowerModel
{
    // THIS FIELD MUST BE SET BY THE CREATING CARD
    public static readonly SpireField<CardModel, Creature> _createdBy = new(() => null);
    
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
        
    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.None;
        
    // CODE GOES HERE
    public override Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
    {
        if (card.Owner.Creature != base.Owner && addedByPlayer 
                                              && RoyalArmsPower._createdBy.Get(card) == base.Owner)
        {
            CardCmd.Enchant<RoyalArmsEnchantment>(card, 1);
        }

        return Task.CompletedTask;
    }
}