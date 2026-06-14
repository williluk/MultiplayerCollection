using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace MultiplayerCollection.MultiplayerCollectionCode.Relics;

[Pool(typeof(RegentRelicPool))]
public class CommandSeat() : CustomRelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;

    private bool _active;

    public override Task BeforeCombatStart()
    {
        if (base.Owner.Creature.CombatState != null)
        {
            IEnumerable<Creature> enumerable = from c in base.Owner.Creature.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer && c != base.Owner.Creature select c;
            foreach (Creature item in enumerable)
            {
                // Per ally code here
                PowerCmd.Apply<StrengthPower>(item, 2, base.Owner.Creature, null);
            }
            _active = true;
        }

        return Task.CompletedTask;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature == base.Owner.Creature && _active == false && base.Owner.Creature.CombatState != null) 
        {
            IEnumerable<Creature> enumerable = from c in base.Owner.Creature.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer && c != base.Owner.Creature select c;
            foreach (Creature item in enumerable)
            {
                // Per ally code here
                PowerCmd.Apply<StrengthPower>(item, -2, base.Owner.Creature, null);
            }
            _active = false;
        }
        return Task.CompletedTask;
    }
}