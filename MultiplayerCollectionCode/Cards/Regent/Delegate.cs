using BaseLib.Abstracts;
using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Random;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;


[Pool(typeof(RegentCardPool))]
public class Delegate() : CustomCardModel(0, CardType.Skill,
    CardRarity.Basic, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[1]
    {
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    };
    
    public override int CanonicalStarCost => 3;
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        IEnumerable<CardModel> cardToMove = await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1), context: choiceContext, player: base.Owner, filter: null, source: this);
        if (cardToMove.Count() > 0)
        {
            CardModel newCard = cardToMove.FirstOrDefault().CreateClone();
            /*CardModelSetOwnerPatch._ownerOverrideEnabled.Set(newCard, true);
            newCard.Owner = play.Target.Player; */
            AccessTools.Field(typeof(CardModel), "_owner").SetValue(newCard, play.Target.Player);

            
            if (newCard != null)
            {   
                if (base.IsUpgraded)
                {
                    CardCmd.Upgrade(newCard);
                }
                CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, creator: base.Owner));
            }
            await CardPileCmd.RemoveFromCombat(cardToMove.FirstOrDefault());
        }
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        
    }
    
}