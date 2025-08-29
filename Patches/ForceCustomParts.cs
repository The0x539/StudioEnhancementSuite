using BepInEx.Configuration;

using HarmonyLib;

using Studio.DataModel.Info;

using System.Collections.Generic;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class ForceCustomParts {
    public static void Register(Harmony harmony, ConfigFile _cfg) {
        //var enabled = cfg.Bind("Force Custom Parts", "Enable", true);
        if (true) {
            harmony.PatchFromCaller();
        }
    }

    private static readonly HashSet<string> modelFilenames = [
        "32203.dat",
        "32204.dat",
        "32205.dat",
        "32206.dat",
        "32207.dat",
        "32208.dat",
        "32210.dat",
        "32211.dat",
        "32212.dat",
        "32213.dat",
        "32214.dat",
        "32216.dat",
        "32218.dat",
        "32221.dat",
        "32225.dat",
        "32227.dat",
        "32228.dat",
        "32229.dat",
        "32230.dat",
        "32242.dat",
        "32246.dat",
        "33298.dat",
        "76319.dat",
    ];

    [HarmonyPatch(typeof(PartIdInfoManager), nameof(PartIdInfoManager.GetFirstMappingInfoFromLDraw))]
    [HarmonyPostfix]
    public static void MaskResult(string ldrawItemNo, ref PartInfo? __result) {
        if (modelFilenames.Contains(ldrawItemNo)) {
            __result = null;
        }
    }
}