using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MultiplayerCollection.MultiplayerCollectionCode.Extensions;
using Logger = Godot.Logger;

namespace MultiplayerCollection.MultiplayerCollectionCode;

public class PoisonRestSiteOption : CustomRestSiteOption
{
    
	private IEnumerable<CardModel>? _selection;

	public override string OptionId => "POISON";
		
	public int PoisonCount { get; set; } = 1;

	public override string? CustomIconPath
	{
		get
		{
			var path = "extra/poisoners_kit_rest_site.png".ImagePath();
			return ResourceLoader.Exists(path) ? path : null;
		} 
		
	}

	public PoisonRestSiteOption(Player owner)
		: base(owner)
	{
		Log.Info("Set enabled");
	}

	public override async Task<bool> OnSelect()
	{
		MainFile.Logger.Info("------> PoisonRestSiteOption.OnSelect()");
		CardSelectorPrefs cardSelectorPrefs =
			new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, PoisonCount) { Cancelable = true, RequireManualConfirmation = true };
		CardSelectorPrefs prefs = cardSelectorPrefs;
		_selection = await CardSelectCmd.FromDeckForEnchantment(base.Owner, ModelDb.Enchantment<PoisonEnchantment>(), 1 ,prefs);
		if (!_selection.Any())
		{
			return false;
		}
		foreach (CardModel item in _selection)
		{
			CardCmd.Enchant<PoisonEnchantment>(item, 1);
		}
		return true;
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