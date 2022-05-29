using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Branch
{
    public static int Size => sizeof(float) * 12;
    private static float minWidth => 0.003f;

    public Vector3 start;
    public Vector3 end;
    public float startWidth;
    public float endWidth;
    public Vector4 color;

    public Branch(Vector3 start, Vector3 end, Vector4 color, float startWidth, float endWidth, bool clamp=true) {
        this.start = start;
        this.end = end;
        this.startWidth = clamp? Mathf.Max(startWidth, minWidth) : startWidth;
        this.endWidth = clamp? Mathf.Max(endWidth, minWidth) : endWidth;
        this.color = color;
    }

    public Branch(Vector3 start, Vector3 end, Color color, float startWidth, float endWidth, bool clamp=true): 
        this(start, end, color.ToVector(), startWidth, endWidth, clamp)
    {}
}
