using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MultiplayerCollection.MultiplayerCollectionCode.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;


[Pool(typeof(NecrobinderCardPool))]
public class ChillTouch() : CustomCardModel(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1]
    {
        new DamageVar(5m, ValueProp.Move) 
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target"); 
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        await PowerCmd.Apply<ChillTouchPower>(new ThrowingPlayerChoiceContext(), play.Target, 1, base.Owner.Creature, this);

    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
    }
}