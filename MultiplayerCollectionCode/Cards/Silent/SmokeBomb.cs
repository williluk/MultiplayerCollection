using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using MultiplayerCollection.MultiplayerCollectionCode.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards.Silent;

[Pool(typeof(SilentCardPool))]
public class SmokeBomb() : CustomCardModel(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.AnyPlayer)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1] { new BlockVar(5, ValueProp.Move) };

    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        /*if (base.CombatState == null)
            return;
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer select c;
        await PowerCmd.Apply<BlurPower>(enumerable, 1, base.Owner.Creature, this);*/
        await PowerCmd.Apply<SmokeBombPower>(play.Target, 1, base.Owner.Creature, this);
        if (base.IsUpgraded)
        {
            await CreatureCmd.GainBlock(play.Target, base.DynamicVars.Block, play);
        }
        
    }
    
}