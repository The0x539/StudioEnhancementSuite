using HarmonyLib;

using UnityEngine;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class UncapFramerate {
    [HarmonyPatch(typeof(Application), nameof(Application.targetFrameRate), MethodType.Setter)]
    [HarmonyPrefix]
    public static void Uncap(ref int value) {
        if (value == 60) {
            value = 144;
        }
    }
}