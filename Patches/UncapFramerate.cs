using BepInEx.Configuration;

using HarmonyLib;

using System.Reflection;

using UnityEngine;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class UncapFramerate {
    private static int newLimit;

    public static void Register(Harmony harmony, ConfigFile cfg) {
        var enabled = cfg.Bind("Shorthand Search", "Enable", true);
        newLimit = cfg.Bind("Uncap Framerate", "New Limit", 144).Value;

        if (enabled.Value) {
            harmony.PatchFromCaller();
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