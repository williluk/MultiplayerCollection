using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using MultiplayerCollection.MultiplayerCollectionCode.Cards;
using MultiplayerCollection.MultiplayerCollectionCode.Extensions;

namespace MultiplayerCollection.MultiplayerCollectionCode;


public sealed class SpiritOrb : CustomOrbModel
{
    public override Color DarkenedColor => new Color("2d6e2d");
    public override string? CustomIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    // Reuse Dark Orb sounds - practical use of overrides
    public override string? CustomPassiveSfx => "event:/sfx/characters/defect/defect_lighting_passive";
    public override string? CustomEvokeSfx => "event:/sfx/characters/defect/defect_lighting_evoke";
    public override string? CustomChannelSfx => "event:/sfx/characters/defect/defect_lighting_channel";

    private decimal _evokeVal = 0m;
    
    public override decimal PassiveVal => ModifyOrbValue(10m);
    public override decimal EvokeVal => _evokeVal;

    public override Node2D? CreateCustomSprite()
    {
        var container = new Node2D();
        // back layer: dark orb (green tint)
        string lightingPath = SceneHelper.GetScenePath("orbs/orb_visuals/lightning_orb");
        Node2D dark = PreloadManager.Cache.GetScene(lightingPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(dark.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        dark.Modulate = new Color(0.3f, 0.3f, 0.8f, 1.0f);
        dark.Scale = new Vector2(1.2f, 1.2f);
        container.AddChild(dark);
        // front layer: glass orb (bright green core)
        string plasmaPath = SceneHelper.GetScenePath("orbs/orb_visuals/plasma_orb");
        Node2D plasma = PreloadManager.Cache.GetScene(plasmaPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(plasma.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        plasma.Modulate = new Color(0.5f, 0.5f, 0.9f, 0.8f);
        container.AddChild(plasma);
        return container;
    }

    // Trigger passive at end of turn - standard pattern for all orbs
    public override async Task AfterTurnStartOrbTrigger(PlayerChoiceContext choiceContext)
        => await Passive(choiceContext, null);
    
    
    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        Trigger(); // fires the orb pulse animation - always call this first
        if (base.CombatState == null)
            return;
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer && c.Player != base.Owner select c;
        foreach (Creature item in enumerable)
        {
            // Per ally code here
            LendEnergy card = base.CombatState.CreateCard<LendEnergy>(item.Player);
            card.mySpiritOrb = this;
            card.DynamicVars["boostValue"].BaseValue = PassiveVal;
            CardModifier? mod = ModelDb.GetById<CardModifier>(ModelDb.GetId<TemporaryCardModifier>()).MutableClone() as CardModifier;
            card.AddModifier(mod);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, creator: base.Owner));
        }
    }
    
    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext choiceContext)
    {
        List<Creature> enemies = base.CombatState.HittableEnemies.Where((Creature e) => e.IsHittable).ToList();
        if (EvokeVal <= 0m)
        {
            return Array.Empty<Creature>();
        }
        await CreatureCmd.Damage(choiceContext, enemies, EvokeVal, ValueProp.Unpowered, base.Owner.Creature);
        return enemies;
    }

    public Task BoostDamage(decimal damage)
    {
        _evokeVal += damage;
        return Task.CompletedTask;
    }
    
}
