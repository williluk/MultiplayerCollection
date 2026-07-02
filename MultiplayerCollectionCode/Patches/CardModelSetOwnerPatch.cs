using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;

/*public class CardModelSetOwnerPatch
{
    public static readonly SpireField<CardModel, bool> _ownerOverrideEnabled = new(() => false);
}


[HarmonyPatch(typeof(CardModel), "set_Owner")]
public class DynamicOwnerPatch
{
    public static bool Prefix(CardModel __instance, Player value)
    {
        if (CardModelSetOwnerPatch._ownerOverrideEnabled.Get(__instance))
        {
            AccessTools.Field(typeof(CardModel), "_owner").SetValue(__instance, value);
            return false;
        }

        return true;
    }
}*/
