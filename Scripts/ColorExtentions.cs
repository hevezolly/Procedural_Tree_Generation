using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtentions
{
    public static Color ToColor(this Vector4 original)
    {
        return new Color(original.x, original.y, original.z, original.w);
    }

    public static Vector4 ToVector(this Color original)
    {
        return new Vector4(original.r, original.g, original.b, original.a);
    }
}
