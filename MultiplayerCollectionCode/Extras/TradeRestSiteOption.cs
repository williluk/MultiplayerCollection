using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using MultiplayerCollection.MultiplayerCollectionCode.Extensions;
using Logger = Godot.Logger;

namespace MultiplayerCollection.MultiplayerCollectionCode;

public class TradeRestSiteOption : CustomRestSiteOption
{
	private IEnumerable<CardModel>? _selection;

	public override string OptionId => "TRADE";

	public static readonly SpireField<Player, (Player?, CardModel?)> TradeOffer = new(() => (null, null));
	
	public int TradeCount { get; set; } = 1;
	
	private LocString? _description;

	public override LocString Description
	{
		get
		{
			if (_description == null)
			{
				_description = base.Description;
				_description.Add("HasTarget", variable: false);
				_description.Add("Name", "");
				_description.Add("TradePending", variable: false);
				_description.Add("PendingName", "");
				_description.Add("PendingCard", "");
			}
			return _description;
		}
	}

	public override string? CustomIconPath
	{
		get
		{
			var path = "extra/trade_rest_site.png".ImagePath();
			return ResourceLoader.Exists(path) ? path : null;
		} 
	}

	public TradeRestSiteOption(Player owner)
		: base(owner)
	{
		Log.Info("Set enabled");
	}

	public async Task OnTargetPlayer(Player player)
	{
		MainFile.Logger.Info("------> PoisonRestSiteOption.OnSelect()");
		CardSelectorPrefs cardSelectorPrefs =
			new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, TradeCount) { Cancelable = false, RequireManualConfirmation = true };
		CardSelectorPrefs prefs = cardSelectorPrefs;
		_selection = await CardSelectCmd.FromDeckGeneric(base.Owner, prefs);
		if (!_selection.Any())
		{
			return;
		}

		MainFile.Logger.Info($"------> Player found {player.NetId}");
			
		if (TradeOffer.Get(player).Item1 == base.Owner && TradeOffer.Get(player).Item2 != null)
		{
				
			await TradeCards(_selection.FirstOrDefault(), TradeOffer.Get(player).Item2);
			return;
		}
		
		TradeOffer.Set(base.Owner, (player, _selection.FirstOrDefault()));
		RestSiteRoom room = (RestSiteRoom)player.RunState.CurrentRoom;
		RestSiteSynchronizer? sync = (RestSiteSynchronizer?)AccessTools.Field(typeof(RestSiteRoom), "_synchronizer").GetValue(room);
		IEnumerable<RestSiteOption> options = from o in sync.GetOptionsForPlayer(player) where o is TradeRestSiteOption select o;
		options.FirstOrDefault().Description.Add("TradePending", variable: true);
		options.FirstOrDefault().Description.Add("PendingName", PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, base.Owner.NetId));
		options.FirstOrDefault().Description.Add("PendingCard", _selection.FirstOrDefault().Title);
	}

	public async Task TradeCards(CardModel card1, CardModel card2)
	{
		CardModel newCard1 = base.Owner.RunState.CloneCard(card1);
		CardModel newCard2 = base.Owner.RunState.CloneCard(card2);
		
		AccessTools.Field(typeof(CardModel), "_owner").SetValue(newCard1, card2.Owner);
		AccessTools.Field(typeof(CardModel), "_owner").SetValue(newCard2, card1.Owner);
		
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(newCard1, PileType.Deck));
		CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(newCard2, PileType.Deck));

		await CardPileCmd.RemoveFromDeck(card1);
		await CardPileCmd.RemoveFromDeck(card2);
	}
	
	
	public override async Task<bool> OnSelect()
	{
		uint choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(base.Owner);
		Player target = null;
		if (LocalContext.IsMe(base.Owner))
		{
			NRestSiteRoom.Instance.AnimateDescriptionDown();
			NRestSiteButton buttonForOption = NRestSiteRoom.Instance.GetButtonForOption(this);
			Vector2 startPosition = ((Control)buttonForOption).GlobalPosition + ((Control)buttonForOption).Size / 2f;
			bool usingController = NControllerManager.Instance.IsUsingController;
			NTargetManager targetManager = NTargetManager.Instance;
			targetManager.StartTargeting(TargetType.AnyPlayer, startPosition, usingController ? TargetMode.Controller : TargetMode.ClickMouseToTarget, ShouldCancelTargeting, AllowHoveringNode);
			if (usingController)
			{
				List<NRestSiteCharacter> list = NRestSiteRoom.Instance.characterAnims.Where((NRestSiteCharacter c) => c.Player != base.Owner).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					list[num].Hitbox.SetFocusMode((Control.FocusModeEnum)2);
					list[num].Hitbox.FocusNeighborTop = ((Node)list[num].Hitbox).GetPath();
					list[num].Hitbox.FocusNeighborBottom = ((Node)list[num].Hitbox).GetPath();
					Control hitbox = list[num].Hitbox;
					NodePath path;
					if (num <= 0)
					{
						path = ((Node)list[list.Count - 1].Hitbox).GetPath();
					}
					else
					{
						path = ((Node)list[num - 1].Hitbox).GetPath();
					}
					hitbox.FocusNeighborLeft = path;
					list[num].Hitbox.FocusNeighborRight = ((num < list.Count - 1) ? ((Node)list[num + 1].Hitbox).GetPath() : ((Node)list[0].Hitbox).GetPath());
				}
				list.FirstOrDefault()?.Hitbox.TryGrabFocus();
			}
			((GodotObject)targetManager).Connect(NTargetManager.SignalName.NodeHovered, Callable.From<Node>((Action<Node>)OnNodeHovered), 0u);
			((GodotObject)targetManager).Connect(NTargetManager.SignalName.NodeUnhovered, Callable.From<Node>((Action<Node>)OnNodeUnhovered), 0u);
			try
			{
				target = NodeToPlayer(await targetManager.SelectionFinished());
				RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(base.Owner, choiceId, PlayerChoiceResult.FromPlayerId(target?.NetId));
			}
			finally
			{
				((GodotObject)targetManager).Disconnect(NTargetManager.SignalName.NodeHovered, Callable.From<Node>((Action<Node>)OnNodeHovered));
				((GodotObject)targetManager).Disconnect(NTargetManager.SignalName.NodeUnhovered, Callable.From<Node>((Action<Node>)OnNodeUnhovered));
				if (usingController)
				{
					foreach (NRestSiteCharacter characterAnim in NRestSiteRoom.Instance.characterAnims)
					{
						characterAnim.Hitbox.SetFocusMode((Control.FocusModeEnum)0);
					}
				}
			}
		}
		else
		{
			ulong? num2 = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(base.Owner, choiceId)).AsPlayerId();
			if (num2.HasValue)
			{
				target = base.Owner.RunState.GetPlayer(num2.Value);
			}
		}
		NRestSiteRoom.Instance?.AnimateDescriptionUp();
		Description.Add("HasTarget", variable: false);
		NRestSiteRoom.Instance?.GetButtonForOption(this)?.RefreshTextState();
		if (target != null)
		{
			await OnTargetPlayer(target);
			return true;
		}
		return false;
	}

	private void OnNodeHovered(Node node)
	{
		Player player = NodeToPlayer(node);
		if (player != null)
		{
			Description.Add("HasTarget", variable: true);
			Description.Add("Name", PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, player.NetId));
			NRestSiteRoom.Instance?.GetButtonForOption(this)?.RefreshTextState();
		}
	}

	private void OnNodeUnhovered(Node _)
	{
		Description.Add("HasTarget", variable: false);
		NRestSiteRoom.Instance?.GetButtonForOption(this)?.RefreshTextState();
	}

	private Player? NodeToPlayer(Node? node)
	{
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

	private bool ShouldCancelTargeting()
	{
		if (NOverlayStack.Instance.ScreenCount <= 0)
		{
			return NCapstoneContainer.Instance.InUse;
		}
		return true;
	}

	private bool AllowHoveringNode(Node node)
	{
		return !LocalContext.IsMe(NodeToPlayer(node));
	}
	
	/*public override async Task DoLocalPostSelectVfx(CancellationToken ct = default(CancellationToken))
	{
		((Node)(object)NRun.Instance?.GlobalUi.CardPreviewContainer).AddChildSafely((Node?)(object)NCardSmithVfx.Create(_selection.ToArray()));
		await Cmd.CustomScaledWait(1f, 2f, ignoreCombatEnd: false, ct);
	}

	public override Task DoRemotePostSelectVfx()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		NRestSiteCharacter nRestSiteCharacter = NRestSiteRoom.Instance?.Characters.First((NRestSiteCharacter c) => c.Player == base.Owner);
		NCardSmithVfx nCardSmithVfx = NCardSmithVfx.Create();
		if (nCardSmithVfx == null)
		{
			return Task.CompletedTask;
		}
		((Node)(object)nRestSiteCharacter)?.AddChildSafely((Node?)(object)nCardSmithVfx);
		((Node2D)nCardSmithVfx).Position = Vector2.Zero;
		return Task.CompletedTask;
	}*/
}