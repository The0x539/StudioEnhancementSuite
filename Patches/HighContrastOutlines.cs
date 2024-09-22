using BepInEx.Configuration;

using HarmonyLib;

using System.Collections.Generic;

using UnityEngine;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class HighContrastOutlines {
    public static void Register(Harmony harmony, ConfigFile cfg) {
        if (cfg.Bind("High Contrast Outlines", "Enable", true).Value) {
            harmony.PatchAll(typeof(HighContrastOutlines));
        }
    }

    private static readonly Dictionary<(int, bool), Material> edgeMaterialCache = [];

    [HarmonyPatch(typeof(DrawableElement), nameof(DrawableElement.getMaterialForEdge))]
    [HarmonyPatch(typeof(Ins_DrawableElement), nameof(Ins_DrawableElement.getMaterialForEdge))]
    [HarmonyPrefix]
    public static bool Do(object __instance, int colorCode, ref Material __result) {
        var insMode = __instance is Ins_DrawableElement && Ins_DrawableElement.IsForInsMode;
        var cacheKey = (colorCode, insMode);

        if (edgeMaterialCache.TryGetValue(cacheKey, out var existing)) {
            __result = existing;
            return false;
        }

        var studioColor = ColorLibrary.Instance.GetStudioColorForLDrawCode(colorCode);
        if (studioColor.CategoryIndex is not 0 or 2 or 3 or 4 or 7 or 8) {
            return true;
        }

        var faceRgb = studioColor.rgbValue;
        var faceColor = Lab.FromRgb(faceRgb[0], faceRgb[1], faceRgb[2]);

        var edgeColor = new Lab {
            L = faceColor.L < 0.75 ? 0.8f : 0.5f,
            a = faceColor.a * 0.6f,
            b = faceColor.b * 0.6f,
        };
        var edgeRgb = edgeColor.ToRgb();

        var resourcePath = insMode ? "Materials/Ins/MaterialForEdge" : "Materials/MaterialForEdge";
        var material = Object.Instantiate(Resources.Load<Material>(resourcePath));

        material.SetColor("_TintColor", edgeRgb);

        material.color = edgeRgb;
        //material.SetColor("_SpecColor", edgeRgb);
        //material.SetColor("_Color", edgeRgb);
        //material.SetColor("_Emission", edgeRgb);

        edgeMaterialCache[cacheKey] = material;
        __result = material;
        return false;
    }
}