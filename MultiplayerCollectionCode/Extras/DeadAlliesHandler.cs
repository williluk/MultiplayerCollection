using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace MultiplayerCollection.MultiplayerCollectionCode;

public class DeadAlliesHandler
{
    public static readonly SpireField<CardModel, bool> _canTargetDeadAllies = new(() => false);

    public static void EnsureCorpseTargetable(Creature creature)
    {
        if (!creature.IsDead || !creature.IsPlayer)
        {
            MainFile.Logger.Info("----> EnsureCorpseTargetable(): creature is not corpse");
            return;
        }
        NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (nCreature != null)
        {
            MainFile.Logger.Info($"----> EnsureCorpseTargetable(): === PRE === isInteractable={nCreature.IsInteractable}, mouseFilter={nCreature.Hitbox.MouseFilter}, focusMode={nCreature.Hitbox.FocusMode}");
            nCreature.ToggleIsInteractable(on: true);
            nCreature.Hitbox.MouseFilter = (Control.MouseFilterEnum)0;
            nCreature.Hitbox.FocusMode = (Control.FocusModeEnum)2;
            MainFile.Logger.Info($"----> EnsureCorpseTargetable(): === PRE === isInteractable={nCreature.IsInteractable}, mouseFilter={nCreature.Hitbox.MouseFilter}, focusMode={nCreature.Hitbox.FocusMode}");
            /*object? value = AccessTools.Field(typeof(NCreature), "_stateDisplay").GetValue(nCreature);
            Control val = (Control)((value is Control) ? value : null);
            if (val != null)
            {
                ((CanvasItem)val).Visible = !NCombatUi.IsDebugHidingHpBar;
                Color modulate = ((CanvasItem)val).Modulate;
                modulate.A = 1f;
                ((CanvasItem)val).Modulate = modulate;
            }*/
        } else 
        {
            MainFile.Logger.Info("----> EnsureCorpseTargetable(): creature node is null");
        }
    }
}
