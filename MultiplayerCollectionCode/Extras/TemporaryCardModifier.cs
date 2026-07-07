using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;

public class TemporaryCardModifier : CardModifier
{
    public async Task RemoveMe()
    {
        MainFile.Logger.Info("-----> Attempting temporary card removal");
        if (Owner != null)
        {
            if (Owner.Pile != null)
            {
                if (Owner.Pile.IsCombatPile)
                    await CardPileCmd.RemoveFromCombat(Owner);
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            await RemoveMe();
        }
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card == base.Owner && (card.Pile.Type != PileType.Hand && card.Pile.Type != PileType.Play))
        {
            MainFile.Logger.Info("-----> Temporary card changed piles");
            await RemoveMe();
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        MainFile.Logger.Info($"-----> CardMod card played check baseOwner={base.Owner}, card={cardPlay.Card}");
        if (cardPlay.Card == base.Owner)
        {
            MainFile.Logger.Info("-----> Temporary card played");
            await RemoveMe();
        }
    }
}