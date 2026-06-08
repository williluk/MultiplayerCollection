using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;

[HarmonyPatch]
public class CardModelCanPlayPatch
{
    public static MethodBase TargetMethod()
    {
        return AccessTools.Method(
            typeof(CardModel),
            nameof(CardModel.CanPlay),
            [typeof(UnplayableReason).MakeByRefType(), typeof(AbstractModel).MakeByRefType()]
            )!;
    }
    
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref UnplayableReason reason, ref AbstractModel? preventer, ref bool __result)
    {
        /*
        MainFile.Logger.Info($"------> == PRE == CardModel.CanPlay: card={__instance.Id}, playable={__result}, reason={reason}" );
        */
        if (__result == false && reason == UnplayableReason.NoLivingAllies &&
            DeadAlliesHandler._canTargetDeadAllies.Get(__instance))
        {
            reason = UnplayableReason.None;
            __result = true;
            /*
            MainFile.Logger.Info($"------> == POST ==CardModel.CanPlay: card={__instance.Id}, playable={__result}, reason={reason}" );
        */
        }
    }
}