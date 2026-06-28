using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MultiplayerCollection.MultiplayerCollectionCode.Powers;


namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class BestowCurse() : CustomCardModel(1,
    CardType.Skill, CardRarity.Common,
    TargetType.AnyEnemy)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[1] {new PowerVar<BestowCursePower>(2m)};
    
    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target"); 
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<BestowCursePower>(new ThrowingPlayerChoiceContext(), play.Target, base.DynamicVars["BestowCursePower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["BestowCursePower"].UpgradeValueBy(1m);
    }
}