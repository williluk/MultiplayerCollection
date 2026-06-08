using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MultiplayerCollection.MultiplayerCollectionCode;

namespace MultiplayerCollection.MultiplayerCollectionCode;

[HarmonyPatch(typeof(NCreature), "OnTargetingStarted")]
internal static class NCreatureOnTargetingStartedPatch
{
    private static void Postfix(NCreature __instance)
    {
        //DeadAlliesHandler.EnsureCorpseTargetable(__instance.Entity);
    }
}