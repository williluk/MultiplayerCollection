using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
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
using MultiplayerCollection.MultiplayerCollectionCode.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode;


public sealed class TargetingOrb : CustomOrbModel
{
    public override Color DarkenedColor => new Color("2d6e2d");
    public override string? CustomIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    // Reuse Dark Orb sounds - practical use of overrides
    public override string? CustomPassiveSfx => "event:/sfx/characters/defect/defect_lighting_passive";
    public override string? CustomEvokeSfx => "event:/sfx/characters/defect/defect_lighting_evoke";
    public override string? CustomChannelSfx => "event:/sfx/characters/defect/defect_lighting_channel";
    
    public override decimal PassiveVal => ModifyOrbValue(1m);
    public override decimal EvokeVal => ModifyOrbValue(3m);

    public override Node2D? CreateCustomSprite()
    {
        var container = new Node2D();
        // front layer: glass orb (bright green core)
        string glassPath = SceneHelper.GetScenePath("orbs/orb_visuals/glass_orb");
        Node2D glass = PreloadManager.Cache.GetScene(glassPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(glass.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        glass.Modulate = new Color(0.1f, 0.1f, 0.1f, 1.0f);
        container.AddChild(glass);
        return container;
    }

    // Trigger passive at end of turn - standard pattern for all orbs
    public override async Task AfterTurnStartOrbTrigger(PlayerChoiceContext choiceContext)
        => await Passive(choiceContext, null);
    
    
    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
         // fires the orb pulse animation - always call this first
        if (base.CombatState == null)
            return;
        ActivatePassive();
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer && c.Player != base.Owner select c;
        foreach (Creature item in enumerable)
        {
            await PowerCmd.Apply<TargetingOrbPower>(choiceContext, item, PassiveVal, base.Owner.Creature, null);
        }
    }

    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext choiceContext)
    {
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer && c.Player != base.Owner select c;
        foreach (Creature item in enumerable)
        {
            await PowerCmd.Apply<TargetingOrbPower>(choiceContext, item, EvokeVal, base.Owner.Creature, null);
        }
        return enumerable;
    }
}
