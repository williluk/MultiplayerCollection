using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;

public class DynamicsTargetType
{
    public static readonly SpireField<CardModel, TargetType> _dynmaicTargetType = new(() => TargetType.None);
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.TargetType))]
public class DynamicTargetTypePatch
{
    static void PostFix(CardModel __instance, ref TargetType __result)
    {
        GD.Print("----> Running CardModel PostFix");
        if (DynamicsTargetType._dynmaicTargetType.Get(__instance) != TargetType.None)
        {
            __result = DynamicsTargetType._dynmaicTargetType.Get(__instance);
        }
    }
}
