using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;

namespace MultiplayerCollection.MultiplayerCollectionCode.Relics;


[Pool(typeof(DefectRelicPool))]
public class CoreShards() : CustomRelicModel
{
    private static readonly ModelId[] _multiplayerOrbs = new ModelId[3]
    {
        ModelDb.GetId<BioOrb>(),
        ModelDb.GetId<SpiritOrb>(),
        ModelDb.GetId<TargetingOrb>()
    };
    
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;
    
    
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner.Creature) && base.Owner.PlayerCombatState.TurnNumber <= 1)
        {
            if (base.Owner.Creature.CombatState == null)
                return;
            IEnumerable<Creature> enumerable = from c in base.Owner.Creature.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer && c != base.Owner.Creature select c;
            foreach (Creature item in enumerable)
            {
                // Per ally code here
                OrbModel orb = ModelDb.GetById<OrbModel>(item.Player.RunState.Rng.CombatOrbGeneration.NextItem(_multiplayerOrbs)).ToMutable();
                await OrbCmd.Channel(choiceContext, orb, item.Player);
            }
        }
    }
    
}