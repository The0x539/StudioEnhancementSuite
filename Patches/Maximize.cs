using BepInEx.Configuration;

using HarmonyLib;

using StudioEnhancementSuite.FFI;

using System;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class Maximize {
    public static void Register(Harmony harmony, ConfigFile cfg) {
        var enabled = cfg.Bind("Maximize on Startup", "Enable", true);
        if (enabled.Value) {
            harmony.PatchFromCaller();
        }
    }

    // If you try to maximize too early, it just gets undone immediately. No idea why.
    // Even SceneManager.sceneUnloaded("Studio for all Initialization") is too early.
    [HarmonyPatch(typeof(Zenject.SceneContext), nameof(Zenject.SceneContext.Awake))]
    [HarmonyPrefix]
    public static void OnStartup() {
        if (SceneManager.GetActiveScene().name != "Studio for All") {
            return;
        }

        var shouldMaximize = PlayerPrefs.GetInt("Maximize", 0) != 0;

        if (shouldMaximize) {
            MaximizeUnityWindow();
        }
        CheckIfMaximized();
        Preferences.Instance.onScreenSizeChanged += CheckIfMaximized;
    }

    [HarmonyPatch(typeof(Preferences), nameof(Preferences.SaveAll))]
    [HarmonyPostfix]
    public static void Save() {
        PlayerPrefs.SetInt("Maximize", currentlyMaximized ? 1 : 0);
    }

    private static void MaximizeUnityWindow() {
        var hWnd = GameWorld.GetActiveWindow();
        User32.ShowWindow(hWnd, ShowWindowCmd.ShowMaximized);
    }

    private static bool currentlyMaximized = false;

    private static void CheckIfMaximized() {
        var hWnd = GameWorld.GetActiveWindow();
        var placement = GetWindowPlacement(hWnd);
        currentlyMaximized = placement.showCmd == ShowWindowCmd.ShowMaximized;
    }

    private static WindowPlacement GetWindowPlacement(nint hWnd) {
        var placement = new WindowPlacement();
        var success = User32.GetWindowPlacement(hWnd, ref placement);
        if (!success) throw new Exception("GetWindowPlacement failed");
        return placement;
    }
}