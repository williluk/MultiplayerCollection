using BaseLib.Abstracts;
using BaseLib.Extensions;
using MultiplayerCollection.MultiplayerCollectionCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Runs;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;


public class LastStandPower : CustomPowerModel
{
    //Loads from MutiplayerCollection/images/powers/your_power.png

    private int turnCounter = 0;
    
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
        
    public override PowerType Type => PowerType.Buff; 
    public override PowerStackType StackType => PowerStackType.None;
        
    // CODE GOES HERE
    public override bool ShouldTakeExtraTurn(Player player)
    {
        if (turnCounter == 0)
        {
            return true;
        }
        return false;
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        turnCounter++;
        return Task.CompletedTask;
    }

    public async override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player && turnCounter >= 1)
        {
            IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner) where c.IsAlive && c.IsPlayer select c;
            foreach (Creature item in enumerable)
            {
                // Per ally code here
                await CreatureCmd.Kill(item, false);
            }
        }
    }
}