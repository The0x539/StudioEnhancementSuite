using BepInEx.Configuration;

using HarmonyLib;

using UnityEngine;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class UncapFramerate {
    private static int newLimit;

    public static void Register(Harmony harmony, ConfigFile cfg) {
        newLimit = cfg.Bind("Uncap Framerate", "New Limit", 144).Value;

        if (cfg.Bind("Uncap Framerate", "Enable", true).Value) {
            harmony.PatchAll(typeof(UncapFramerate));
        }
    }

    [HarmonyPatch(typeof(Application), nameof(Application.targetFrameRate), MethodType.Setter)]
    [HarmonyPrefix]
    public static void Uncap(ref int value) {
        if (value == 60) {
            value = newLimit;
        }
    }
}