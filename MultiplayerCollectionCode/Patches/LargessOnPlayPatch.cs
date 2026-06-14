using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MultiplayerCollection.MultiplayerCollectionCode.Powers;

namespace MultiplayerCollection.MultiplayerCollectionCode;

[HarmonyPatch(typeof(Largesse), "OnPlay")]
public class LargessOnPlayPatch
{
    private static async Task LargessOnPlay(Largesse __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await CreatureCmd.TriggerAnim(__instance.Owner.Creature, "Cast", __instance.Owner.Character.CastAnimDelay);
        CardModel cardModel = CardFactory.GetDistinctForCombat(cardPlay.Target.Player,
            ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(cardPlay.Target.Player.UnlockState,
                cardPlay.Target.Player.RunState.CardMultiplayerConstraint), 1,
            __instance.Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault();
        if (cardModel != null)
        {
            if (__instance.IsUpgraded)
            {
                CardCmd.Upgrade(cardModel);
            }
            RoyalArmsPower._createdBy.Set(cardModel, __instance.Owner.Creature);
            await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);
        }
    }
    
    private static bool Prefix(Largesse __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay,
        ref Task __result)
    {
        __result = LargessOnPlay(__instance, choiceContext, cardPlay);
        return false;
    }
}