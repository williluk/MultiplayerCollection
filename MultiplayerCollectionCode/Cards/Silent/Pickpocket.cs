using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards.Silent;

[Pool(typeof(SilentCardPool))]
public class Pickpocket() : CustomCardModel(2, CardType.Skill,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[1]
    {
        CardKeyword.Exhaust
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        int value = 15;
        if (DidAllyAttack(play.Target))
        {
            value += 10;
        }
        await PowerCmd.Apply<RoyaltiesPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, value, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
    
    bool DidAllyAttack(Creature target)
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