using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;


public class PickpocketedCardModifier : CardModifier
{
    public Player originalOwner = null;
    
    public async Task ReturnToOriginalOwner()
    {
        CardModel newCard = base.Owner.CreateClone();
        AccessTools.Field(typeof(CardModel), "_owner").SetValue(newCard, originalOwner);
        if (newCard != null)
        {
            CardModifier.RemoveModifier(newCard, CardModifier.Modifiers(newCard).Where(c => c is PickpocketedCardModifier).FirstOrDefault());
            newCard.RemoveKeyword(ExtraKeywords.Stolen);
            CardCmd.PreviewCardPileAdd(
                await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, creator: null));
            await CardPileCmd.RemoveFromCombat(base.Owner);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            await ReturnToOriginalOwner();
        }
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card == base.Owner && (card.Pile.Type != PileType.Hand && card.Pile.Type != PileType.Play))
        {
            MainFile.Logger.Info("-----> Temporary card changed piles");
            await ReturnToOriginalOwner();
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        MainFile.Logger.Info($"-----> CardMod card played check baseOwner={base.Owner}, card={cardPlay.Card}");
        if (cardPlay.Card == base.Owner)
        {
            MainFile.Logger.Info("-----> Temporary card played");
            await ReturnToOriginalOwner();
        }
    }
}