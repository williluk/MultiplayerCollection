using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;


[Pool(typeof(DefectCardPool))]
public class Cybernetics() : CustomCardModel(2,
    CardType.Skill, CardRarity.Common,
    TargetType.AnyAlly)
{
    public override OrbEvokeType OrbEvokeType => OrbEvokeType.Front;
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1] {new PowerVar<FocusPower>(1m)};
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[3]
    {
        HoverTipFactory.Static(StaticHoverTip.Evoke),
        HoverTipFactory.FromPower<FocusPower>(),
        HoverTipFactory.Static(StaticHoverTip.Channeling),
    };
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        if (base.Owner.PlayerCombatState.OrbQueue.Orbs.Count > 0)
        {
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
            OrbModel orb = base.Owner.PlayerCombatState.OrbQueue.Orbs.First();
            await OrbCmd.EvokeNext(choiceContext, base.Owner);
            await Cmd.CustomScaledWait(0.1f, 0.25f);
            await OrbCmd.AddSlots(play.Target.Player, 1);
            await PowerCmd.Apply<FocusPower>(choiceContext, play.Target, base.DynamicVars["FocusPower"].BaseValue, base.Owner.Creature, this);
            await OrbCmd.Channel(choiceContext, ModelDb.GetById<OrbModel>(orb.Id).ToMutable(), play.Target.Player);
        }
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars["FocusPower"].UpgradeValueBy(1m);
    }
}