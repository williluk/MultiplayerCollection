using BaseLib.Abstracts;
using BaseLib.Extensions;
using MultiplayerCollection.MultiplayerCollectionCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.ValueProps;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;


public class ShieldMasterPower : CustomPowerModel
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
        
    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[3]
    {
        new BlockVar(0, ValueProp.Move), 
        new DynamicVar("blockBoost", 25m),
        new DynamicVar("totalBoost", base.Amount * base.DynamicVars["blockBoost"].BaseValue),
    };
    
    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card.Owner == base.Owner.Player && card.TargetType == TargetType.Self && card.GainsBlock)
        {
            CardModelGetTargetTypePatch._dynamicTargetType.Set(card, TargetType.AnyPlayer);
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardDrawnEarly(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner == base.Owner.Player && card.TargetType == TargetType.Self && card.GainsBlock)
        {
            CardModelGetTargetTypePatch._dynamicTargetType.Set(card, TargetType.AnyPlayer);
        }
        return Task.CompletedTask;
    }

    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource == null || cardPlay == null || cardPlay.Target == null)
            return 1m;
        if (CardModelGetTargetTypePatch._dynamicTargetType.Get(cardPlay.Card) != TargetType.AnyAlly)
            return 1m;
        if (!cardPlay.Target.IsPlayer)
            return 1m;
        if (target == base.Owner && cardPlay.Target != base.Owner)
        {
            base.DynamicVars.Block.BaseValue = block * (1m + (base.DynamicVars["blockBoost"].BaseValue * base.Amount / 100m));
            CreatureCmd.GainBlock(cardPlay.Target, base.DynamicVars.Block, cardPlay);
            base.DynamicVars.Block.BaseValue = 0; 
            return 0m;
        }
        return 1m;
    }
}