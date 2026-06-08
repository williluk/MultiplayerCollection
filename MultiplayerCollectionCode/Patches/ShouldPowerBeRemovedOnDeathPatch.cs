using BaseLib.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;

[HarmonyPatch(typeof(PowerModel), "ShouldPowerBeRemovedAfterOwnerDeath")]
internal static class PowerModelShouldPowerBeRemovedAfterOwnerDeathPatch
{
    
    private static bool Prefix(PowerModel __instance, ref bool __result)
    {
        MainFile.Logger.Info("ShouldPowerBeRemovedAfterOwnerDeath is returning false");
        if (__instance.Owner.IsPlayer)
        {
            __result = false;
        }
        return false;
    }
}