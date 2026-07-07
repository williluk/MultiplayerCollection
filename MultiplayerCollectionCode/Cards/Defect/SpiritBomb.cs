using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;



[Pool(typeof(DefectCardPool))]
public class SpiritBomb() : CustomCardModel(1, CardType.Skill,
    CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[1]
    {
        HoverTipFactory.FromCard<LendEnergy>()
    };
    
    
    
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await OrbCmd.Channel<SpiritOrb>(choiceContext, base.Owner);
        if (IsUpgraded)
        {
            await OrbCmd.Passive(choiceContext, base.Owner.PlayerCombatState.OrbQueue.Orbs.Last(), null);
        }
    }
}