using BaseLib.Abstracts;
using BaseLib.Patches.Features;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards.Silent;

[Pool(typeof(SilentCardPool))]

public class FollowUp() : CustomCardModel(1, CardType.Attack,
    CardRarity.Common, TargetType.AllEnemies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1]
    {
        new DamageVar(10m, ValueProp.Move)
    };

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (CombatState != null)
        {
            var enemies = CombatState.HittableEnemies;
            IEnumerable<Creature> targets = from e in enemies where Filter(e) select e;

            foreach (var t in targets)
            {
                await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(t)
                    .WithHitFx("vfx/vfx_attack_slash")
                    .Execute(choiceContext);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(5);
    }

    bool Filter(Creature target)
    {
        if (target.IsAlive && CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>().Count((DamageReceivedEntry e) =>
                e.Receiver == target && e.Result.Props.IsPoweredAttack() &&
                e.HappenedThisTurn(CombatState) && e.Dealer != null &&
                e.Dealer != base.Owner.Creature && e.Dealer.Side == base.Owner.Creature.Side) >= 1)
        {
            return true;
        }
        return false;
    }
}