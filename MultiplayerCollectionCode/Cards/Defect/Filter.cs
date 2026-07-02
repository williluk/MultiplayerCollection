using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MultiplayerCollection.MultiplayerCollectionCode.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;


[Pool(typeof(DefectCardPool))]
public class Filter() : CustomCardModel(1, CardType.Power,
    CardRarity.Common, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1] {new PowerVar<FilterPower>(2m)};
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await PowerCmd.Apply<FilterPower>(new ThrowingPlayerChoiceContext(),base.Owner.Creature, base.DynamicVars["FilterPower"].BaseValue, base.Owner.Creature, null);

    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["FilterPower"].UpgradeValueBy(1m);
    }
}