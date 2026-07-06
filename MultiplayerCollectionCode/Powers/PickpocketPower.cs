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


public class PickpocketPower : CustomPowerModel
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
    public override PowerStackType StackType => PowerStackType.Single;

    public CardModel? WatchCard = null;
    public CardModel? HeldCard = null;

    // CODE GOES HERE
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        MainFile.Logger.Info($"------> HeldCard is {HeldCard}");
        if (cardPlay.Card == WatchCard && HeldCard != null)
        {
            await CardPileCmd.Add(HeldCard, PileType.Hand);
            //await CardPileCmd.AddGeneratedCardToCombat(HeldCard, PileType.Hand, creator: null);
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        MainFile.Logger.Info($"------> HeldCard is {HeldCard}");
        if (card == WatchCard && HeldCard != null)
        {
            await CardPileCmd.Add(HeldCard, PileType.Hand);
            //await CardPileCmd.AddGeneratedCardToCombat(HeldCard, PileType.Hand, creator: null);
            await PowerCmd.Remove(this);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await PowerCmd.Remove(this);
    }
}