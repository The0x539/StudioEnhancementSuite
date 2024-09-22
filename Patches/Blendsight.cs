using HarmonyLib;

using System;
using System.Diagnostics;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class Blendsight {
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

        var psi = new ProcessStartInfo("C:/Program Files/Blender Foundation/Blender 4.2/blender.exe");
        object[] args = [
            "--python", "C:/Users/Andrew/source/python/blendsight/blendsight.py",
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