using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;

using MultiplayerCollection.MultiplayerCollectionCode.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

// DEBUG THIS IN A DOUBLE NECROBINDER CASE
[Pool(typeof(NecrobinderCardPool))]
public class PartyMascot() : CustomCardModel(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.AllAllies)
{
    protected override bool ShouldGlowRedInternal => base.Owner.IsOstyMissing;
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1] {new PowerVar<PartyMascotPower>(2m)};
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[1]
    {
        HoverTipFactory.FromPower<StrengthPower>(),
    };
    
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (!Osty.CheckMissingWithAnim(base.Owner))
        {
            if (base.CombatState == null)
                return;
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

            IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
                where c.IsAlive && c.IsPlayer
                select c;
            foreach (Creature item in enumerable)
            {
                await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), item, base.DynamicVars["PartyMascotPower"].BaseValue,
                    base.Owner.Creature, this);
                
            }
            // also hit self
            await PowerCmd.Apply<PartyMascotPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, -base.DynamicVars["PartyMascotPower"].BaseValue,  base.Owner.Creature, this);
            //await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars["PartyMascotPower"].BaseValue, base.Owner.Creature, this);
            //await PowerCmd.Apply<PartyMascotPower>(base.Owner.Creature, -base.DynamicVars["PartyMascotPower"].BaseValue, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["PartyMascotPower"].UpgradeValueBy(1m);
    }
}