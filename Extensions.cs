using System;

namespace StudioEnhancementSuite;

public static class Extensions {
    public static ReadOnlySpan<char> TrimPrefix(this ReadOnlySpan<char> span, ReadOnlySpan<char> prefix)
        => span.StartsWith(prefix) ? span[prefix.Length..] : span;

    public static ReadOnlySpan<char> TrimSuffix(this ReadOnlySpan<char> span, ReadOnlySpan<char> suffix)
        => span.EndsWith(suffix) ? span[..^suffix.Length] : span;
}