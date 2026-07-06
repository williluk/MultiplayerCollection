using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace MultiplayerCollection.MultiplayerCollectionCode.Cards;

[Pool(typeof(TokenCardPool))]
public class Frey() : CustomCardModel(0, CardType.Attack,
    CardRarity.Token, TargetType.AnyEnemy)
{
    
    private decimal _extraDamageFromFreyPlays;

    private decimal ExtraDamageFromFreyPlays
    {
        get
        {
            return _extraDamageFromFreyPlays;
        }
        set
        {
            AssertMutable();
            _extraDamageFromFreyPlays = value;
        }
    }
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[2]
    {
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("Increase", 1m)
    };

    protected override async Task OnPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target, "play.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(play.Target)
            .WithHitVfxNode((Creature t) => (Node2D?)(object)NScratchVfx.Create(t, goingRight: true))
            .Execute(choiceContext);
        IEnumerable<Frey> freyCards = [];
        if (base.CombatState == null)
            return;
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer select c;
        foreach (Creature item in enumerable)
        {
            // Per ally code here
            freyCards = freyCards.Concat(item.Player.PlayerCombatState.AllCards.OfType<Frey>());
        }
            
        decimal baseValue = base.DynamicVars["Increase"].BaseValue;
        foreach (Frey item in freyCards)
        {
            item.BuffFromFreyPlay(baseValue);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Increase"].UpgradeValueBy(1m);
    }
    
    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        base.DynamicVars.Damage.BaseValue += ExtraDamageFromFreyPlays;
    }

    private void BuffFromFreyPlay(decimal extraDamage)
    {
        base.DynamicVars.Damage.BaseValue += extraDamage;
        ExtraDamageFromFreyPlays += extraDamage;
    }
    
    public static IEnumerable<Frey> Create(Player owner, int amount, ICombatState combatState)
    {
        List<Frey> list = new List<Frey>();
        for (int i = 0; i < amount; i++)
        {
            list.Add(combatState.CreateCard<Frey>(owner));
        }
        return list;
    }
}