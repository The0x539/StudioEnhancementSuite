using HarmonyLib;

using System;
using System.Diagnostics;

namespace StudioEnhancementSuite;

public static class Extensions {
    public static ReadOnlySpan<char> TrimPrefix(this ReadOnlySpan<char> span, ReadOnlySpan<char> prefix)
        => span.StartsWith(prefix) ? span[prefix.Length..] : span;

    public static ReadOnlySpan<char> TrimSuffix(this ReadOnlySpan<char> span, ReadOnlySpan<char> suffix)
        => span.EndsWith(suffix) ? span[..^suffix.Length] : span;

    public static void PatchFromCaller(this Harmony harmony) {
        var type = new StackFrame(1).GetMethod().DeclaringType;
        harmony.PatchAll(type);
    }
}