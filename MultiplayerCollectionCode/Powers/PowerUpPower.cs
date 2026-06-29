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
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Random;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;

public class PowerUpPower : CustomPowerModel
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
    public override PowerStackType StackType => PowerStackType.None;
    
    // CODE GOES HERE
    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card.Owner == base.Owner.Player && card.TargetType == TargetType.Self && card.Type == CardType.Power)
        {
            DynamicTargetType._dynamicTargetType.Set(card, TargetType.AnyAlly);
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardDrawnEarly(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner == base.Owner.Player && card.TargetType == TargetType.Self && card.Type == CardType.Power)
        {
            DynamicTargetType._dynamicTargetType.Set(card, TargetType.AnyAlly);
        }
        return Task.CompletedTask;
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner.Player && cardPlay.Card.TargetType == TargetType.Self && cardPlay.Card.Type == CardType.Power)
        {
            if (DynamicTargetType._dynamicTargetType.Get(cardPlay.Card) == TargetType.AnyAlly)
            {
                await CardCmd.AutoPlay(context, cardPlay.Card, cardPlay.Target);
            }
        }
    }
    
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner == base.Owner.Player && card.TargetType == TargetType.Self && card.Type == CardType.Power)
        {
            modifiedCost = originalCost + (decimal)base.Amount;
            return true;
        } 
        return false;
    }
}