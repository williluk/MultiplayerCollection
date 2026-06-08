using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace MultiplayerCollection.MultiplayerCollectionCode;

[HarmonyPatch(typeof(NPlayerHand), "AreCardActionsAllowed")]
public class NPlayerHandAreCardActionsAllowedPatch
{
    private static bool ShouldBlock(NPlayerHand playerHand)
    {
        if (!(AccessTools.Field(typeof(NPlayerHand), "_combatState").GetValue(playerHand) is CombatState combatState))
        {
            return false;
        }
        Player me = LocalContext.GetMe(combatState);
        if (me?.Creature == null)
        {
            return false;
        }
        return me.Creature.IsDead;
    }
    
    private static bool Prefix(NPlayerHand __instance, ref bool __result)
    {
        if (!ShouldBlock(__instance))
        {
            return true;
        }
        __result = false;
        return false;
    }
}