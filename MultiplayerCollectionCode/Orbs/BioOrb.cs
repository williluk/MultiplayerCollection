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


public sealed class BioOrb : CustomOrbModel
{
    public override Color DarkenedColor => new Color("2d6e2d");
    public override string? CustomIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    // Reuse Dark Orb sounds - practical use of overrides
    public override string? CustomPassiveSfx => "event:/sfx/characters/defect/defect_lighting_passive";
    public override string? CustomEvokeSfx => "event:/sfx/characters/defect/defect_lighting_evoke";
    public override string? CustomChannelSfx => "event:/sfx/characters/defect/defect_lighting_channel";
    
    public override decimal PassiveVal => 1;
    public override decimal EvokeVal => 1;

    public override Node2D? CreateCustomSprite()
    {
        var container = new Node2D();
        // back layer: dark orb (green tint)
        string darkPath = SceneHelper.GetScenePath("orbs/orb_visuals/dark_orb");
        Node2D dark = PreloadManager.Cache.GetScene(darkPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(dark.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        dark.Modulate = new Color(0.3f, 0.8f, 0.6f, 1.0f);
        dark.Scale = new Vector2(1.1f, 1.1f);
        container.AddChild(dark);
        // front layer: glass orb (bright green core)
        string plasmaPath = SceneHelper.GetScenePath("orbs/orb_visuals/plasma_orb");
        Node2D plasma = PreloadManager.Cache.GetScene(plasmaPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(plasma.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        plasma.Modulate = new Color(0.3f, 0.8f, 0.6f, 1.0f);
        container.AddChild(plasma);
        return container;
    }

    // Trigger passive at end of turn - standard pattern for all orbs
    public override async Task BeforeTurnEndOrbTrigger(PlayerChoiceContext choiceContext)
        => await Passive(choiceContext, null);
    
    
    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        Trigger(); // fires the orb pulse animation - always call this first
        if (base.CombatState == null)
            return;
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer && c.Player != base.Owner select c;

        for (int i = 0; i < PassiveVal; i++)
        {
            Creature? targ = enumerable.TakeRandom(1, base.Owner.RunState.Rng.CombatTargets).FirstOrDefault();
            if (targ != null)
                await OrbCmd.Channel<BioOrb>(choiceContext, targ.Player);
        }
    }
    
    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext choiceContext)
    {
        await OrbCmd.AddSlots(base.Owner, 1);
        return [base.Owner.Creature];
    }
}
