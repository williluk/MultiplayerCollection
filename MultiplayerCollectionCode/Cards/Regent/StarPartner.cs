using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(RegentCardPool))]
public class StarPartner() : CustomCardModel(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        IEnumerable<CardModel> result = [];
            
        int num = base.IsUpgraded ? 2 : 1;
        for (int i = 0; i < num; i++)
        {
            IEnumerable<CardModel> pool = ModelDb.AllCards.Where((CardModel c) => c.MultiplayerConstraint == CardMultiplayerConstraint.MultiplayerOnly);
            result = CardFactory.GetDistinctForCombat(play.Target.Player, pool, 2, base.Owner.RunState.Rng.CombatCardGeneration);
        }
        await CardPileCmd.AddGeneratedCardsToCombat(result, PileType.Hand, addedByPlayer: true);
    }
    
}