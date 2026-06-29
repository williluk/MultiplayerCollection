using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MultiplayerCollection.MultiplayerCollectionCode;

public class ChargedCardModifier : CardModifier
{
    public decimal base_boost = 1.1m;
    public decimal boost_val = 1.1m;
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource)
    {
        if (cardSource == Owner)
        {
            return amount * boost_val;
        }
        return amount;
    }
}