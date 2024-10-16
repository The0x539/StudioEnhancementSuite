using BepInEx.Configuration;

using HarmonyLib;

using StudioEnhancementSuite.FFI;

using System;
using System.Runtime.InteropServices;

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
    [HarmonyPostfix]
    public static void OnStartup() {
        if (SceneManager.GetActiveScene().name != "Studio for All") {
            return;
        }

        var shouldMaximize = PlayerPrefs.GetInt("Maximize", 0) != 0;

        if (shouldMaximize) {
            Console.WriteLine("maximize");
            MaximizeUnityWindow();
        } else {
            Console.WriteLine("do not maximize");
        }
    }

    [HarmonyPatch(typeof(Preferences), nameof(Preferences.SaveAll))]
    [HarmonyPostfix]
    public static void Save() {
        var hWnd = GameWorld.GetActiveWindow();
        var placement = GetWindowPlacement(hWnd);
        var currentlyMaximized = placement.showCmd == ShowWindowCmd.ShowMaximized;

        PlayerPrefs.SetInt("Maximize", currentlyMaximized ? 1 : 0);
    }

    private static void MaximizeUnityWindow() {
        var hWnd = GameWorld.GetActiveWindow();
        User32.ShowWindow(hWnd, ShowWindowCmd.ShowMaximized);
    }

    private static WindowPlacement GetWindowPlacement(nint hWnd) {
        var placement = new WindowPlacement();
        var success = User32.GetWindowPlacement(hWnd, ref placement);
        if (!success) {
            Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
        }
        return placement;
    }
}