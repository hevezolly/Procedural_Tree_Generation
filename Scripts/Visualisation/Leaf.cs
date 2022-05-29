using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Leaf
{
    public static int Size => sizeof(float) * 9;

    public Vector3 position;
    public float radius;
    public Vector4 color;
    public float power;

    public Leaf(Vector3 position, float radius, Vector4 color, float power)
    {
        this.position = position;
        this.radius = radius;
        this.color = color;
        this.power = power;
    }

    public Leaf(Vector3 position, float radius, Color color, float power) : this(position, radius, color.ToVector(), power) { }
}
