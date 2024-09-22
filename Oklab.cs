using System;

using UnityEngine;

namespace StudioEnhancementSuite;

public record struct Lab(float L, float a, float b) {
    public static Lab FromRgb(float r, float g, float b) => FromRgb(new(r, g, b));

    public static Lab FromRgb(Color c) {
        var l = 0.4122214708f * c.r + 0.5363325363f * c.g + 0.0514459929f * c.b;
        var m = 0.2119034982f * c.r + 0.6806995451f * c.g + 0.1073969566f * c.b;
        var s = 0.0883024619f * c.r + 0.2817188376f * c.g + 0.6299787005f * c.b;

        (l, m, s) = (MathF.Cbrt(l), MathF.Cbrt(m), MathF.Cbrt(s));

        return new Lab {
            L = 0.2104542553f * l + 0.7936177850f * m - 0.0040720468f * s,
            a = 1.9779984951f * l - 2.4285922050f * m + 0.4505937099f * s,
            b = 0.0259040371f * l + 0.7827717662f * m - 0.8086757660f * s,
        };
    }

    public Color ToRgb() {
        var l = this.L + 0.3963377774f * this.a + 0.2158037573f * this.b;
        var m = this.L - 0.1055613458f * this.a - 0.0638541728f * this.b;
        var s = this.L - 0.0894841775f * this.a - 1.2914855480f * this.b;

        (l, m, s) = (l * l * l, m * m * m, s * s * s);

        return new Color {
            r = +4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s,
            g = -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s,
            b = -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s,
            a = 1f,
        };
    }
}