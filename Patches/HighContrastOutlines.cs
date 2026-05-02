using BepInEx.Configuration;

using HarmonyLib;

using System.Collections.Generic;

using UnityEngine;

using Studio.Application.Services;
using System;
using Studio.Presentation.World;
using UI_Script.World;
using Studio.Presentation.Services;


namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class HighContrastOutlines {
    public static void Register(Harmony harmony, ConfigFile cfg) {
        var enabled = cfg.Bind("High Contrast Outlines", "Enable", true);
        if (enabled.Value) {
            Console.WriteLine("JFDOIsjoisdfjoijfdsoi");
            harmony.PatchFromCaller();
        }
    }

    private static readonly Dictionary<int, Material> edgeMaterialCache = [];

    [HarmonyPatch(typeof(BrickMaterialServiceBase), nameof(BrickMaterialServiceBase.GetEdgeMaterial))]
    [HarmonyPostfix]
    public static void Do(BrickMaterialServiceBase __instance, in BrickMaterialParameters parameters, ref Material __result) {
        var colorCode = parameters.ColorCode;
        var cacheKey = colorCode;

        if (edgeMaterialCache.TryGetValue(cacheKey, out var existing)) {
            __result = existing;
            return;
        }

        if (__result != __instance.CachedEdgeMaterial && __result != __instance.CachedLightEdgeMaterial) {
            return;
        }

        var studioColor = IColorLibrary.Instance.GetStudioColorForLDrawCode(colorCode);

        var faceRgb = studioColor.RGBValue;
        var faceColor = Lab.FromRgb(faceRgb[0], faceRgb[1], faceRgb[2]);

        var dark = faceColor.L < 0.75;
        var material = dark ? __instance.CachedEdgeMaterial : __instance.CachedLightEdgeMaterial;
        material = UnityEngine.Object.Instantiate(material);

        var edgeColor = new Lab {
            L = dark ? 0.8f : 0.5f,
            a = faceColor.a * 0.6f,
            b = faceColor.b * 0.6f,
        };

        __instance.SetColor(material, edgeColor.ToRgb());
        edgeMaterialCache[cacheKey] = material;
        __result = material;
    }
}