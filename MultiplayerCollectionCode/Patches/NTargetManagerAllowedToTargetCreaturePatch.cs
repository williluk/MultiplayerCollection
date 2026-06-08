using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MultiplayerCollection.MultiplayerCollectionCode;

namespace MultiplayerCollection.MultiplayerCollectionCode;

[HarmonyPatch(typeof(NTargetManager), "AllowedToTargetCreature")]
internal static class NTargetManagerAllowedToTargetCreaturePatch
{
    
    private static void Postfix(NTargetManager __instance, Creature creature, ref bool __result)
    {
        var x = (TargetType?)AccessTools.Field(typeof(NTargetManager), "_validTargetsType").GetValue(__instance);
        if (x != null)
        {
            MainFile.Logger.Info($"-----> AllowedToTargetCreature running: __result={__result}, creature.IsPlayer={creature.IsPlayer}, creature.IsDead={creature.IsDead}, x={x} ");
            if (__result == false && creature.IsPlayer && creature.IsDead && x == TargetType.AnyAlly)
            {
                MainFile.Logger.Info("-----> AllowedToTargetCreature modifying result");
                __result = true;
            }
        }
    }
}