using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;


[HarmonyPatch(typeof(RestSiteOption), "Generate")]
public class RestSiteOptionGeneratePatch
{
    public static void Postfix(RestSiteOption __instance, Player player, ref List<RestSiteOption> __result)
    {
        if (player.RunState.Players.Count > 1)
        {
            __result.Add(new TradeRestSiteOption(player));
        }
    }
}
