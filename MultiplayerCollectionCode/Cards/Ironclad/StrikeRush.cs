using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(IroncladCardPool))]
public class StrikeRush() : CustomCardModel(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [ new DamageVar(5m, ValueProp.Move) ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[1]
    {
        HoverTipFactory.FromCard<StrikeIronclad>(),
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
            .Execute(choiceContext);

        List<Creature> items = [];
        if (base.Owner.Creature.CombatState != null)
            items = base.Owner.Creature.CombatState.Allies.Where((Creature c) => c.IsAlive && c.IsPlayer && c != base.Owner.Creature).ToList();
        
        for (int i = 0; i < items.Count; i++)
        {
            IEnumerable<CardModel> drawp = PileType.Draw.GetPile(items.TakeRandom(1, RunState.Rng.Niche).First().Player).Cards.Where(Filter).ToList();
            foreach (CardModel c in drawp)
            {
                if (base.IsUpgraded)
                {
                    CardCmd.Upgrade(c);
                }
                await CardCmd.AutoPlay(choiceContext, c, play.Target);
                return;
            }
        }
    }

    private bool Filter(CardModel card)
    {
        bool flag = card.IsBasicStrikeOrDefend && card.Tags.Contains(CardTag.Strike);
        bool flag2 = flag;
        // not sure what this stuff is for. Copied from All For One
        if (flag2)
        {
            CardType type = card.Type;
            bool flag3 = (uint)(type - 1) <= 2u;
            flag2 = flag3;
        }
        return flag2;
    }
}