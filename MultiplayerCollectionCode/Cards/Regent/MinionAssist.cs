using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(RegentCardPool))]
public class MinionAssist() : CustomCardModel(0,
    CardType.Skill, CardRarity.Token,
    TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => 
        [
            new BlockVar(5, ValueProp.Move),
            new PowerVar<VigorPower>(5)
        ];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.GainBlock(play.Target, base.DynamicVars.Block, play, false);
        await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(), play.Target, base.DynamicVars.Power<VigorPower>().BaseValue, base.Owner.Creature, this, false);
    }
    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2);
        base.DynamicVars.Power<VigorPower>().UpgradeValueBy(2);
    }
}