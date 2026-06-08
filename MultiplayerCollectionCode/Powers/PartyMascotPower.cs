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
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Monsters;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;


public class PartyMascotPower : CustomPowerModel
{
    //Loads from MutiplayerCollection/images/powers/your_power.png
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
    public override PowerStackType StackType => PowerStackType.Counter;
        
    // CODE GOES HERE
    
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (delta >= 0m && creature.Monster is Osty && creature.PetOwner == base.Owner.Player && creature.CurrentHp - delta <= 0m)
        {
            GD.Print("----> Removing party mascot powwer");
            IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner)
                where c.IsAlive && c.IsPlayer
                select c;
            foreach (Creature item in enumerable)
            {
                await PowerCmd.Apply<StrengthPower>(item, base.Amount, base.Owner, null);
            }
            await PowerCmd.Remove(this);
        }
    }
}


