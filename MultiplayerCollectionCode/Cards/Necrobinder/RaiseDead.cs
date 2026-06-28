using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;


[Pool(typeof(NecrobinderCardPool))]
public class RaiseDead() : CustomCardModel(2, CardType.Skill,
    CardRarity.Rare, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    protected override IEnumerable<DynamicVar> CanonicalVars => [ 
        new HealVar(15),
        new PowerVar<DoomPower>(20)
    ];

    public override void AfterCreated()
    {
        //MainFile.Logger.Info("------> Raise dead after created");
        DeadAlliesHandler._canTargetDeadAllies.Set(this, true);
    }
    

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "cardPlay.Target"); 
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        //VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_bloody_impact");
        
        // Not sure why this is inverted, but seems IsDead is false if dead and true if alive
        if (!play.Target.IsDead)
        {
            //MainFile.Logger.Info("----> Target is not dead");
            await CreatureCmd.Heal(play.Target, base.DynamicVars.Heal.BaseValue);
            await PowerCmd.Apply<DoomPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars.Power<DoomPower>().BaseValue, base.Owner.Creature, play.Card);
        }
        else
        {
            //MainFile.Logger.Info("----> Targeting dead ally");
            await CreatureCmd.Heal(play.Target, base.DynamicVars.Heal.BaseValue);
            IReadOnlyList<Creature> enemies = base.CombatState.HittableEnemies;
            /*foreach (Creature target in (IEnumerable<Creature>) enemies)    
            {
                NCombatRoom instance = NCombatRoom.Instance;
                if (instance != null)
                    instance.CombatVfxContainer.AddChildSafely((Node) NSpikeSplashVfx.Create(target));
            }*/
            await PowerCmd.Apply<DoomPower>(new ThrowingPlayerChoiceContext(), enemies, base.DynamicVars.Power<DoomPower>().BaseValue, base.Owner.Creature, play.Card);
        }
        
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Heal.UpgradeValueBy(5);
    }
}