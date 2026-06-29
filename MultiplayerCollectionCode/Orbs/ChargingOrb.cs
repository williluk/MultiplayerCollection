using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using BaseLib.Abstracts;
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

namespace MultiplayerCollection.MultiplayerCollectionCode;


public sealed class ChargingOrb : CustomOrbModel
{
    public override Color DarkenedColor => new Color("2d6e2d");

    // Reuse Dark Orb sounds - practical use of overrides
    public override string? CustomPassiveSfx => "event:/sfx/characters/defect/defect_lighting_passive";
    public override string? CustomEvokeSfx => "event:/sfx/characters/defect/defect_lighting_evoke";
    public override string? CustomChannelSfx => "event:/sfx/characters/defect/defect_lighting_channel";

    public override decimal PassiveVal => ModifyOrbValue(1m);
    public override decimal EvokeVal => ModifyOrbValue(10m);

    /*public override Node2D? CreateCustomSprite()
    {
        var container = new Node2D();
        // back layer: dark orb (green tint)
        string darkPath = SceneHelper.GetScenePath("orbs/orb_visuals/dark_orb");
        Node2D dark = PreloadManager.Cache.GetScene(darkPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(dark.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        dark.Modulate = new Color(0.1f, 0.5f, 0.1f, 1.0f);
        dark.Scale = new Vector2(1.1f, 1.1f);
        container.AddChild(dark);
        // front layer: glass orb (bright green core)
        string glassPath = SceneHelper.GetScenePath("orbs/orb_visuals/glass_orb");
        Node2D glass = PreloadManager.Cache.GetScene(glassPath)
            .Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        new MegaSprite(glass.GetNode("SpineSkeleton"))
            .GetAnimationState().SetAnimation("idle_loop");
        glass.Modulate = new Color(0.3f, 0.9f, 0.3f, 1.0f);
        container.AddChild(glass);
        return container;
    }*/

    // Trigger passive at end of turn - standard pattern for all orbs
    public override async Task BeforeTurnEndOrbTrigger(PlayerChoiceContext choiceContext)
        => await Passive(choiceContext, null);
    
    private List<ChargedCardModifier> _currentCharged = [];
    
    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        Trigger(); // fires the orb pulse animation - always call this first
        _currentCharged.Clear();
        for (int i = 0; i < PassiveVal; i++)
        {
            IEnumerable<Creature> allies = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature) where c.IsAlive && c.IsPlayer select c;
            Player random_ally = allies.TakeRandom(1, base.Owner.RunState.Rng.CombatTargets).FirstOrDefault().Player;
            CardPile pile = PileType.Hand.GetPile(random_ally);
            CardModel cardModel = base.Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards.Where((CardModel c) => c.Type == CardType.Attack));
            ChargedCardModifier m = new ChargedCardModifier();
            ChargedCardModifier.AddModifier(cardModel, m);
            _currentCharged.Add(m);
        }
    }
    
    public override async Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext choiceContext)
    {
        foreach (var m in _currentCharged)
        {
            m.boost_val += m.base_boost;
        }

        return [];
    }
}
