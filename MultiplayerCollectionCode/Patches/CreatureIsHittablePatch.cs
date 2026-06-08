using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace MultiplayerCollection.MultiplayerCollectionCode;


[HarmonyPatch(typeof(Creature), "get_IsHittable")]
public class CreatureIsHittablePatch
{   
    private static bool Prefix(Creature __instance, ref bool __result)
    {
        if (__instance.IsDead && __instance.IsPlayer)
        {
            __result = true;
            return false;
        }
        return true;
    }
}  