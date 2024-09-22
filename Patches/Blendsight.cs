using BepInEx.Configuration;

using HarmonyLib;

using System;
using System.Diagnostics;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class Blendsight {
    private static string blenderPath = "C:/Program Files/Blender Foundation/Blender 4.2/blender.exe";
    private static string blendsightPath = "";

    public static void Register(Harmony harmony, ConfigFile cfg) {
        blenderPath = cfg.Bind("Blendsight", "Blender Path", blenderPath).Value;
        blendsightPath = cfg.Bind("Blendsight", "blendsight.py Path", "").Value;

        if (cfg.Bind("Blendsight", "Enable", false).Value) {
            harmony.PatchAll(typeof(Blendsight));
        }
    }

    [HarmonyPatch(typeof(MenuHandler), nameof(MenuHandler.OnHandleRenderByEyesight))]
    [HarmonyPrefix]
    public static bool RedirectEyesightToBlender(MenuHandler __instance, string filePath, KOGRendererConfig conf, CameraInformation cameraInformation) {
        var loadedFile = __instance._blStudioWrapper._loadedFileService.LoadedFile;
        var modelPath = loadedFile.filePath;

        if (string.IsNullOrEmpty(modelPath)) {
            throw new NotImplementedException("TODO: save to a temp file");
        } else if (loadedFile.IsModified) {
            throw new NotImplementedException("TODO: prompt to save changes");
        }

        var camPos = cameraInformation.position;
        var camRot = cameraInformation.rotation;

        var psi = new ProcessStartInfo(blenderPath);
        object[] args = [
            "--python", blendsightPath,
            "--",
            "-i", modelPath,
            "-o", filePath,
            "--camera-position", camPos.x, camPos.y, camPos.z,
            "--camera-rotation", camRot.w, camRot.x, camRot.y, camRot.z,
            "--clip-plane", cameraInformation.nearClipPlane, cameraInformation.farClipPlane,
            "--fov", cameraInformation.fieldOfView,
            "--resolution", conf.ImageWidth, conf.ImageHeight,
        ];
        foreach (var arg in args) {
            psi.ArgumentList.Add(arg.ToString());
        }
        Console.WriteLine(string.Join(' ', args));
        psi.UseShellExecute = false;
        //psi.RedirectStandardOutput = false;
        //psi.RedirectStandardError = false;

        Process.Start(psi);

        return false;
    }
}