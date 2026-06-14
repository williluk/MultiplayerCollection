using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(RegentCardPool))]
public class Delegate() : CustomCardModel(0, CardType.Skill,
    CardRarity.Basic, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [(HoverTipFactory.FromKeyword(CardKeyword.Exhaust))];
    
    public override int CanonicalStarCost => 3;
        
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        IEnumerable<CardModel> cardToMove = await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), context: choiceContext, player: base.Owner, filter: null, source: this);
        if (cardToMove != null)
        {
            CardModel newCard = CardFactory.GetDistinctForCombat(play.Target.Player, cardToMove, 1, base.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
            if (newCard != null)
            {
                if (base.IsUpgraded)
                {
                    CardCmd.Upgrade(newCard);
                }
                await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, addedByPlayer: true);
            }
            await CardCmd.Exhaust(choiceContext, cardToMove.FirstOrDefault());
        }
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
    }
    
}