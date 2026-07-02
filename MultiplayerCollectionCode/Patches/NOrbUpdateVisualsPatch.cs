using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Orbs;

namespace MultiplayerCollection.MultiplayerCollectionCode;


[HarmonyPatch(typeof(NOrb), "UpdateVisuals")]
public class NOrbUpdateVisualsPatch
{   
    private static void Postfix(NOrb __instance)
    {
        MainFile.Logger.Info("-----> Update Visuals Postfix");
        if (__instance.Model != null || !__instance.IsNodeReady() || !CombatManager.Instance.IsInProgress)
        {
            MainFile.Logger.Info("-----> Inside return check");
            if (__instance.Model is SpiritOrb)
            {
                MainFile.Logger.Info("-----> Is Spirit Orb");
                MegaLabel evokeLabel = (MegaLabel)AccessTools.Field(typeof(NOrb), "_evokeLabel").GetValue(__instance);
                if (evokeLabel != null)
                {
                    MainFile.Logger.Info("-----> Changing evoke label");

                    ((CanvasItem)evokeLabel).Visible = true;
                    evokeLabel.SetTextAutoSize(__instance.Model.EvokeVal.ToString("0"));
                }
            }
        }
    }
}