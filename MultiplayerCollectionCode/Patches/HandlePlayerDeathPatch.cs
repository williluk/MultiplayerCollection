// sts2, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null
// MegaCrit.Sts2.Core.Combat.CombatManager

using System.ComponentModel;
using BaseLib.Patches.Utils;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MultiplayerCollection.MultiplayerCollectionCode;

[HarmonyPatch(typeof(CombatManager))]
internal class CombatManagerPatch
{
    public static async Task HandlePlayerDeathAsync(Player player)
    {
        MainFile.Logger.Info("-----> Attempting modified player death");
        if (CombatManager.Instance.IsInProgress)
        {
            MainFile.Logger.Info("-----> inside isInProgress");
            await PlayerCmd.SetEnergy(0m, player);
            await PlayerCmd.SetStars(0m, player);
            await CardPileCmd.Add(player.PlayerCombatState.Hand.Cards.ToList(), player.PlayerCombatState.DiscardPile);
            PlayerCmd.EndTurn(player, false);

            DeadAlliesHandler.EnsureCorpseTargetable(player.Creature);
        }
    }
    
    [HarmonyPatch("HandlePlayerDeath")]
    private static bool Prefix(Player player, ref Task __result)
    {
        __result = HandlePlayerDeathAsync(player);
        return false;
    }
    
    /*[HarmonyPatch("StartTurn")]
    private static void Postfix(CombatManager __instance, ref Task __result)
    {
        CombatState combatState =
            AccessTools.Field(typeof(CombatManager), "_state").GetValue(__instance) as CombatState;
        if (combatState != null)
        {
            MainFile.Logger.Info("-----> Running start turn postfix");
            foreach (Player player in combatState.Players)
            {
                if (player.Creature.IsDead)
                {
                    MainFile.Logger.Info("-----> Force ending player turn");
                    //PlayerCmd.EndTurn(player, false);
                }
            }
            
        }
    }*/
}

/*public class DynamicTargetType
{
    public static readonly SpireField<CardModel, TargetType> _dynamicTargetType = new(() => TargetType.None);
}*/



