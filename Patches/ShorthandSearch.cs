using BepInEx.Configuration;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;

namespace StudioEnhancementSuite.Patches;

[HarmonyPatch]
public static class ShorthandSearch {
    public static void Register(Harmony harmony, ConfigFile cfg) {
        var enabled = cfg.Bind("Shorthand Search", "Enable", true);
        if (enabled.Value) {
            harmony.PatchFromCaller();
        }
    }

    [HarmonyPatch(typeof(Bucket), nameof(Bucket.getBucketItems))]
    [HarmonyPostfix]
    public static void AddItems(List<BucketItem> repository, string keyword, List<BucketItem> __result) {
        var chars = keyword.ToLower().ToCharArray().Where(c => c != ' ').ToArray();
        foreach (var item in repository) {
            if (Matches(chars, item.Description)) {
                __result.Add(item);
            }
        }
    }

    private static bool Matches(in ReadOnlySpan<char> chars, string name) {
        if (name == null) return false;

        var i = 0;
        foreach (var word in name.ToLower().Split()) {
            if (word.StartsWith(chars[i])) {
                i++;
                if (i == chars.Length) {
                    return true;
                }
            }
        }

        return false;
    }
}