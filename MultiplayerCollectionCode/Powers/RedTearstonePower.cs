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
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Monsters;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;

public class RedTearstonePower : CustomPowerModel
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
        
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[3]
    {
        new BlockVar(0, ValueProp.Move), 
        new DynamicVar("dmgBoost", 0.25m),
        new DynamicVar("totalBoost", base.Amount * base.DynamicVars["dmgBoost"].BaseValue),
    };
    // CODE GOES HERE
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        //GD.Print("----> Modifying damage value");
        if (!props.IsPoweredAttack())
        {
            //GD.Print("----> Power attack break");
            return 1m;
        }
        if (dealer != base.Owner)
        {
            //GD.Print("----> Dealer isn't owner");

            return 1m;
        }
        //GD.Print("----> Good");

        return 1.25m;
    }
}

