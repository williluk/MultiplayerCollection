using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace MultiplayerCollection.MultiplayerCollectionCode;

//You're recommended but not required to keep all your code in this package and all your assets in the MultiplayerCollection folder.
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string
        ModId = "MultiplayerCollection"; //At the moment, this is used only for the Logger and harmony names.

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        //If you want to use scripts defined in your mod for Godot scenes, uncomment the following line.
        //Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());

        Harmony harmony = new(ModId);
        Harmony.DEBUG = true;

        harmony.PatchAll();
        
        DeferredLogPatches(harmony);
        
        /*CustomLocTableManager.RegisterCustomLocTable(LocManager.Instance, "enchantments.json");
        CustomLocTableManager.RegisterCustomLocTable(LocManager.Instance, "rest_site_options.json");*/

    }
    
    static void DeferredLogPatches(Harmony harmony)
    {
        var tree = (SceneTree)Engine.GetMainLoop();
        Action callback = null;
        callback = () =>
        {
            tree.ProcessFrame -= callback;
            try
            {
                var allMethods = harmony.GetPatchedMethods();
                Logger.Info("MultiplayerCollection: === All Harmony Patches in Game ===");

                var modPatchCounts = new Dictionary<string, int>();
                int totalPatchedMethods = 0;

                foreach (var method in allMethods)
                {
                    var patchInfo = Harmony.GetPatchInfo(method);
                    if (patchInfo == null)
                    {
                        Logger.Info($"  NULL Patch detected at {method.DeclaringType?.Name}.{method.Name}");
                        continue;
                    }

                    totalPatchedMethods++;
                    var owners = patchInfo.Owners.ToList();
                    string methodName = $"{method.DeclaringType?.Name}.{method.Name}";
                    string prefixes = patchInfo.Prefixes != null ? string.Join(", ", patchInfo.Prefixes.Select(p => $"{p.owner}({p.priority})")) : "none";
                    string postfixes = patchInfo.Postfixes != null ? string.Join(", ", patchInfo.Postfixes.Select(p => $"{p.owner}({p.priority})")) : "none";
                    string transpilers = patchInfo.Transpilers != null ? string.Join(", ", patchInfo.Transpilers.Select(p => $"{p.owner}({p.priority})")) : "none";

                    Logger.Info($"  [PATCH] {methodName}");
                    Logger.Info($"    Owners: {string.Join(", ", owners)}");
                    if (patchInfo.Prefixes != null && patchInfo.Prefixes.Count > 0)
                        Logger.Info($"    Prefixes: {prefixes}");
                    if (patchInfo.Postfixes != null && patchInfo.Postfixes.Count > 0)
                        Logger.Info($"    Postfixes: {postfixes}");
                    if (patchInfo.Transpilers != null && patchInfo.Transpilers.Count > 0)
                        Logger.Info($"    Transpilers: {transpilers}");

                    foreach (var owner in owners.Distinct())
                    {
                        if (!modPatchCounts.ContainsKey(owner))
                            modPatchCounts[owner] = 0;
                        modPatchCounts[owner]++;
                    }
                }

                Logger.Info("MultiplayerCollection: === Patch Summary ===");
                Logger.Info($"  Total patched methods: {totalPatchedMethods}");
                foreach (var kvp in modPatchCounts.OrderByDescending(x => x.Value))
                {
                    Logger.Info($"  {kvp.Key}: {kvp.Value} methods");
                }
                Logger.Info("MultiplayerCollection: === End Patch Report ===");
            }
            catch (Exception ex)
            {
                Logger.Warn($"MultiplayerCollection: Failed to enumerate patches: {ex}");
            }
        };
        tree.ProcessFrame += callback;
    }
}