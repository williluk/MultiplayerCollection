using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rooms;
using MultiplayerCollection.MultiplayerCollectionCode.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode.Relics;


[Pool(typeof(NecrobinderRelicPool))]
public class RedTearstone() : CustomRelicModel
{
    private const string _hpThresholdKey = "HpThreshold";

    public override RelicRarity Rarity =>
        RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("HpThreshold", 25m),
        new DynamicVar("DamageBoostValue", 25m),
        new PowerVar<RedTearstonePower>(1m)
    ];
    
    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature.CurrentHp <= creature.MaxHp * (base.DynamicVars["HpThreshold"].BaseValue / 100))
        {
            await PowerCmd.Apply<RedTearstonePower>(new ThrowingPlayerChoiceContext(),creature, 1m + (base.DynamicVars["DamageBoostValue"].BaseValue / 100), creature, null);
        } 
    }
    
}