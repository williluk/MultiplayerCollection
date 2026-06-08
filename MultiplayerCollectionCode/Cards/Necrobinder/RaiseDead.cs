using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;


[Pool(typeof(NecrobinderCardPool))]
public class RaiseDead() : CustomCardModel(2, CardType.Skill,
    CardRarity.Rare, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override void AfterCreated()
    {
        MainFile.Logger.Info("------> Raise dead after created");
        DeadAlliesHandler._canTargetDeadAllies.Set(this, true);
    }

    public override bool ShouldAllowTargeting(Creature target)
    {
        MainFile.Logger.Info("-----> ShouldAllowTargeting is true");
        return true;
    }

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target"); 
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_bloody_impact");
        int healVal = 10;
        await PowerCmd.Apply<DoomPower>(base.Owner.Creature, 20, base.Owner.Creature, play.Card);
        await CreatureCmd.Heal(play.Target, healVal);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}