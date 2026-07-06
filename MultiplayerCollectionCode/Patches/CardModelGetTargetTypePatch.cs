using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;

public class DynamicTargetType
{
    public static readonly SpireField<CardModel, TargetType> _dynamicTargetType = new(() => TargetType.None);
}


[HarmonyPatch(typeof(CardModel), "get_TargetType")]
static class CardModelGetTargetTypePatch
{

    public static void Postfix(CardModel __instance, ref TargetType __result)
    {
        if (DynamicTargetType._dynamicTargetType.Get(__instance) != TargetType.None)
        {
            __result = DynamicTargetType._dynamicTargetType.Get(__instance);
        }
    }
}
