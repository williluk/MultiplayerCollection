using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(RegentCardPool))]
public class RoyalSupport() : CustomCardModel(0,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DynamicVar("xBonus", 0m)];
    
    public override bool HasStarCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer select c;
        await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(), enumerable, (decimal)ResolveStarXValue() + base.DynamicVars["xBonus"].BaseValue, base.Owner.Creature, null);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["xBonus"].UpgradeValueBy(3);
    }
}