using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(RegentCardPool))]
public class GraciousGifts() : CustomCardModel(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    private CardModel? _mockSelectedCard;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        CardModel cardModel;
        if (_mockSelectedCard == null)
        {
            List<CardModel> cards = CardFactory.GetDistinctForCombat(base.Owner, ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(base.Owner.UnlockState, base.Owner.RunState.CardMultiplayerConstraint), 3, base.Owner.RunState.Rng.CombatCardGeneration).ToList();
            cardModel = await CardSelectCmd.FromChooseACardScreen(choiceContext, cards, base.Owner, canSkip: true);
            if (IsUpgraded)
            {
                CardCmd.Upgrade(cardModel);
            }
        }
        else
        {
            cardModel = _mockSelectedCard;
        }
        if (cardModel != null)
        {
            if (base.CombatState == null)
                return;
            IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer select c;
            
            foreach (Creature item in enumerable)
            {
                // Per ally code here
                CardModel newCard = CardFactory.GetDistinctForCombat(item.Player, [cardModel.CanonicalInstance], 1, base.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
                newCard.AddKeyword(CardKeyword.Ethereal);
                await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, creator: base.Owner);

            }
        }
    }
    
    
    public void MockSelectedCard(CardModel card)
    {
        AssertMutable();
        _mockSelectedCard = card;
    }
}