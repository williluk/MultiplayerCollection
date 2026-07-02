using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;

public class CardModelGetTargetTypePatch
{
    public static readonly SpireField<CardModel, TargetType> _dynamicTargetType = new(() => TargetType.None);
}


[HarmonyPatch(typeof(CardModel), "get_TargetType")]
static class DynamicTargetTypePatch
{

    public static void Postfix(CardModel __instance, ref TargetType __result)
    {
        if (CardModelGetTargetTypePatch._dynamicTargetType.Get(__instance) != TargetType.None)
        {
            __result = CardModelGetTargetTypePatch._dynamicTargetType.Get(__instance);
        }
    }
}
