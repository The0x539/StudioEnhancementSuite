using BepInEx.Configuration;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class ShortPartNames {
    public static void Register(Harmony harmony, ConfigFile cfg) {
        var enabled = cfg.Bind("Short Part Names", "Enable", true);
        if (enabled.Value) {
            harmony.PatchFromCaller();
        }
    }

    private static readonly Dictionary<string, string> shortNames = [];

    [HarmonyPatch(typeof(Bucket), nameof(Bucket.AddItem))]
    [HarmonyPrefix]
    public static void GatherShortNames(Bucket __instance, BucketItem item) {
        AddShortName(__instance, item);
    }

    [HarmonyPatch(typeof(BrickTexture), nameof(BrickTexture.ReDraw))]
    [HarmonyPostfix]
    public static void UseShortNames(BrickTexture __instance) {
        if (shortNames.TryGetValue(__instance.m_labelDesc.text, out var label)) {
            __instance.m_labelDesc.text = label;
        }
    }

    private static void AddShortName(Bucket bucket, BucketItem item) {
        if (shortNames.ContainsKey(item.description)) {
            return;
        }

        var fullName = item.description.AsSpan();
        var prefix = GetPrefix(bucket.bucketName);

        var shortened = fullName.TrimPrefix(prefix).ToString();
        foreach (var (longTerm, shortTerm) in abbreviations) {
            shortened = shortened.Replace(longTerm, shortTerm);
        }

        shortened = shortened.TrimStart([' ', ',']);

        if (prefix is "Slope" or "Slope, Inverted") {
            var i = shortened.IndexOf(' ');
            shortened = shortened.Insert(i, "°");
        }


        shortNames.Add(fullName.ToString(), shortened);
    }

    [SuppressMessage("Method Declaration", "Harmony003", Justification = "false positive")]
    private static ReadOnlySpan<char> GetPrefix(ReadOnlySpan<char> bucketName) => bucketName.TrimSuffix(", Decorated") switch {
        "Bionicle, Kanohi Mask" => "Bionicle Mask",
        "Door Frame" => "Door, Frame",
        "Electric, Battery Box" or "Electric, Light & Sound" or "Electric, Wire & Connector" or "Electric, Train" => "Electric",
        "Large Figure Part" => "Large Figure",
        "Minifigure, Body Wear" or "Minifigure, Headgear Accessory" => "Minifigure",
        "Minifigure, Legs" => "Hips and",
        "Minifigure, Torso" => "Torso",
        "Minifigure, Torso Assembly" => "Torso Plain /",
        "Minifigure, Torso Assembly, Decor." => "Torso",
        "Rubber Band & Belt" => "Rubber",
        "String Reel / Winch" => "String",
        "Technic, Connector" => "Technic",
        "Vehicle, Base" => "Vehicle",
        "Wheel, Tire & Tread" => "Tire",
        "Wheel, Accessory" => "Wheel Cover",
        "Mini Doll, Body Part" or "Mini Doll, Body Wear" => "Mini Doll",
        "Mini Doll, Legs Assembly" => "Mini Doll Hips and",
        "Mini Doll, Torso Assembly" => "Torso Mini Doll",
        "Projectile Launcher" => "Projectile",
        var x => x,
    };

    private static readonly (string, string)[] abbreviations = [
        ("with Stud on Side", "1-SNOT"),

        ("with Studs on 1 Side", "SNOT"),
        ("with Studs on Side", "SNOT"),

        ("with Studs on 2 Sides, Opposite", "2-SNOT"),
        ("with Studs on 2 Sides", "2-SNOT"),
        ("with Studs on Sides", "2-SNOT"),

        ("with Studs on 2 Sides, Adjacent", "C-SNOT"),
        ("with Studs on Side and Ends", "3-SNOT"),
        ("with Studs on 4 Sides", "4-SNOT"),

        (" x ", " × "),
        (" 1/4", "¼"),
        (" 1/2", "½"),
        (" 3/4", "¾"),
        (" 1/3", "⅓"),
        (" 2/3", "⅔"),
        (" ×", "×"), ("× ", "×"),
        (" degrees", "°"),
        (" without ", " ʷ/ₒ "),
        (" with ", " ʷ/ "),
        ("Inverted", "Inv."),
        ("Double", "Dbl."),
        ("Triple", "Tri."),
        ("Quadruple", "Quad."),
        ("Quarter", "Qtr."),
        ("Female", "♀️"),
        ("Male", "♂"),
        (" and ", " & "),
    ];
}