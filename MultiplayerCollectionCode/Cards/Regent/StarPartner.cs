using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(RegentCardPool))]
public class StarPartner() : CustomCardModel(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1]
    {
        new CardsVar(1)
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        IEnumerable<CardModel> result = [];
        
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        IEnumerable<CardModel> pool = ModelDb.AllCards.Where((CardModel c) => c.MultiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly && c.Rarity != CardRarity.Token);
        result = CardFactory.GetDistinctForCombat(base.Owner, pool, (int)base.DynamicVars.Cards.BaseValue, base.Owner.RunState.Rng.CombatCardGeneration);
        
        await CardPileCmd.AddGeneratedCardsToCombat(result, PileType.Hand, creator: base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1);
    }
}