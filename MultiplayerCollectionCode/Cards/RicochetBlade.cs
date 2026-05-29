using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class RicochetBlade() : CustomCardModel(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        
        List<Creature> items = base.Owner.Creature.CombatState.Allies.Where((Creature c) => c != null && c.IsAlive && c.IsPlayer && c != base.Owner.Creature).ToList();
        //List<RicochetBlade> list = Create(items.TakeRandom<Creature>(), 1, base.CombatState).ToList();
        //CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Hand, addedByPlayer: true));
    }

    public static IEnumerable<RicochetBlade> Create(Player owner, int amount, CombatState combatState)
    {
        List<RicochetBlade> list = new List<RicochetBlade>();
        for (int i = 0; i < amount; i++)
        {
            list.Add(combatState.CreateCard<RicochetBlade>(owner));
        }
        return list;
    }
}