using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;


[HarmonyPatch(typeof(CardModel), "IsValidTarget")]
internal static class CardModelIsValidTargetPatch
{
    private static bool Prefix(CardModel __instance, Creature? target, ref bool __result)
    {
        MainFile.Logger.Info("------> Attempting modified target check");
        if (__instance.TargetType == TargetType.AnyAlly && target.IsPlayer && target.IsDead && DeadAlliesHandler._canTargetDeadAllies.Get(__instance))
        {
            __result = true;
            return false;
        }
        return true;
    }
}