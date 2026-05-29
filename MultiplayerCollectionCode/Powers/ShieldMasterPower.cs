using BaseLib.Abstracts;
using BaseLib.Extensions;
using MultiplayerCollection.MultiplayerCollectionCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;


namespace MultiplayerCollection.MultiplayerCollectionCode.Powers;


public class ShieldMasterPower : CustomPowerModel
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
        
    public override PowerType Type => PowerType.Debuff; 
    public override PowerStackType StackType => PowerStackType.Counter;
        
    // CODE GOES HERE
    private bool ShouldCancelTargeting()
    {
        if (NOverlayStack.Instance.ScreenCount <= 0)
        {
            return NCapstoneContainer.Instance.InUse;
        }
        return true;
    }
    
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.GainsBlock && cardPlay.Card.TargetType == TargetType.Self)
        {
            bool usingController = NControllerManager.Instance.IsUsingController;
            NTargetManager targetManager = NTargetManager.Instance;
            targetManager.StartTargeting(TargetType.AnyPlayer, new Vector2(0, 0), usingController ? TargetMode.Controller : TargetMode.ClickMouseToTarget, ShouldCancelTargeting, AllowHoveringNode);
            var target = NodeToPlayer(await targetManager.SelectionFinished());
            GD.Print(target != null);
            if (target != null)
            {
                cardPlay = new CardPlay
                {
                    Card = cardPlay.Card,
                    Target = target.Creature,
                    ResultPile = cardPlay.ResultPile,
                    Resources = cardPlay.Resources,
                    IsAutoPlay = cardPlay.IsAutoPlay,
                    PlayIndex = cardPlay.PlayIndex,
                    PlayCount = cardPlay.PlayCount,
                };
                CardCmd.AutoPlay(new GameActionPlayerChoiceContext(new PlayCardAction(cardPlay.Card, cardPlay.Target)),
                    cardPlay.Card, target.Creature);
            }
        }
    }
    
    
    private Player? NodeToPlayer(Node? node)
    {
        GD.Print(node.Name);
        if (node == null)
        {
            return null;
        }
        if (!(node is NMultiplayerPlayerState nMultiplayerPlayerState))
        {
            if (node is NRestSiteCharacter nRestSiteCharacter)
            {
                return nRestSiteCharacter.Player;
            }
            return null;
        }
        return nMultiplayerPlayerState.Player;
    }
    
    private bool AllowHoveringNode(Node node)
    {
        return true;
    }
}