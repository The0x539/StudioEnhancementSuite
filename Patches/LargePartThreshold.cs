using BepInEx.Configuration;

using HarmonyLib;

using UnityEngine;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class LargePartThreshold {
    private static float newThreshold = 320f;

    public static void Register(Harmony harmony, ConfigFile cfg) {
        var enabled = cfg.Bind("Large Part Threshold", "Enable", true);
        newThreshold = cfg.Bind("Large Part Threshold", "New Threshold", newThreshold).Value;

        if (enabled.Value) {
            harmony.PatchFromCaller();
        }
    }

    [HarmonyPatch(typeof(Ins_PageDesign_BOMItem), nameof(Ins_PageDesign_BOMItem.IsLargePart))]
    [HarmonyPostfix]
    public static void IsLargePart(LDrawPart part, ref bool __result) {
        var size = part.model.Bounds.size;
        var n = Mathf.Max(size.x, size.y, size.z);
        __result = n >= newThreshold;
    }
}