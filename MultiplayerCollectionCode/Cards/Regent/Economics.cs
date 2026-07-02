using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MultiplayerCollection.MultiplayerCollectionCode.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(RegentCardPool))]
public class Economics() : CustomCardModel(2, CardType.Power,
    CardRarity.Rare, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[2]
    {
        new DynamicVar("StarReq", 10), 
        new EnergyVar(1)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[1]
    {
        HoverTipFactory.Static(StaticHoverTip.Energy),
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<EconomicsPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars["StarReq"].BaseValue, base.Owner.Creature, null);

    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["StarReq"].UpgradeValueBy(-2);
    }
}