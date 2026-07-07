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
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MultiplayerCollection.MultiplayerCollectionCode.Cards.Silent;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;

public class SecondThatPower : CustomPowerModel
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
    
    public static readonly SpireField<CardModel, bool> _isAutoPlayedBySecondThat = new(() => false);

        
    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Skill && cardPlay.Card.Owner.Creature != base.Owner && !_isAutoPlayedBySecondThat.Get(cardPlay.Card))
        {
            CardModel copy = cardPlay.Card.CreateClone();
            AccessTools.Field(typeof(CardModel), "_owner").SetValue(copy, base.Owner.Player);
            copy.AddKeyword(ExtraKeywords.Temporary);
            CardModifier? mod = ModelDb.GetById<CardModifier>(ModelDb.GetId<TemporaryCardModifier>()).MutableClone() as CardModifier;
            copy.AddModifier(mod);
            _isAutoPlayedBySecondThat.Set(copy, true);
            await CardCmd.AutoPlay(context, copy, cardPlay.Card.CurrentTarget);
            await PowerCmd.Decrement(this);
        }
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == base.Owner.Side)
        {
            PowerCmd.Remove(this);
        }
        return Task.CompletedTask;
    }
}