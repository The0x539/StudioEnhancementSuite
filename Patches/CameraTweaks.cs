using BepInEx.Configuration;

using HarmonyLib;

using UnityEngine;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class CameraTweaks {
    static ConfigEntry<bool> preventLeftClickControl = null!;
    static ConfigEntry<float> sensitivity = null!;

    public static void Register(Harmony harmony, ConfigFile cfg) {
        var enabled = cfg.Bind("Camera Tweaks", "Enable", true);

        preventLeftClickControl = cfg.Bind("Camera Tweaks", "Prevent Left-Click Rotate/Pan", true);
        sensitivity = cfg.Bind("Camera Tweaks", "Sensitivity", 1.0f);

        if (enabled.Value) {
            harmony.PatchFromCaller();
        }
    }

    [HarmonyPatch(typeof(OrbitControl), nameof(OrbitControl.OnMouseMove))]
    [HarmonyPrefix]
    public static bool Hook(OrbitControl __instance, ref Vector2 delta) {
        if (preventLeftClickControl.Value) {
            if (Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2)) {
                __instance.IsOnControlOrbitControl = false; // Done by the method itself before its early-returns. Purpose unknown.
                // If left click is the only mouse button being pressed, stop the method from running and thus from responding to it.
                return false;
            }
        }

        if (sensitivity.Value != 1.0) {
            delta *= sensitivity.Value;
        }

        return true;
    }
}