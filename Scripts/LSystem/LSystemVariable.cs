using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LSystemVariable
{
    [SerializeField]
    private string name;

    [SerializeField]
    private AvalibleType valueType;

    [SerializeField]
    private Color _Color;

    [SerializeField]
    private float _Float;

    [SerializeField]
    private int _Int;

    [SerializeField]
    private bool _Bool;

    [SerializeField]
    private Vector2 _Vector2;

    [SerializeField]
    private Vector3 _Vector3;

    [SerializeField]
    private string _String;

    public object Value 
    { 
        get 
        {
            switch (valueType)
            {
                case AvalibleType.String:
                    return _String;
                case AvalibleType.Bool:
                    return _Bool;
                case AvalibleType.Int:
                    return _Int;
                case AvalibleType.Float:
                    return _Float;
                case AvalibleType.Color:
                    return _Color;
                case AvalibleType.Vector2:
                    return _Vector2;
                case AvalibleType.Vector3:
                    return _Vector3;
                default:
                    throw new System.TypeAccessException("incorrect enum type");
            }
        } 
    }

    public string VariableName => name;

    public System.Type Type
    {
        get
        {
            switch (valueType)
            {
                case AvalibleType.String:
                    return typeof(string);
                case AvalibleType.Bool:
                    return typeof(bool);
                case AvalibleType.Int:
                    return typeof(int);
                case AvalibleType.Float:
                    return typeof(float);
                case AvalibleType.Color:
                    return typeof(Color);
                case AvalibleType.Vector2:
                    return typeof(Vector2);
                case AvalibleType.Vector3:
                    return typeof(Vector3);
                default:
                    throw new System.TypeAccessException("incorrect enum type");
            }
        }
    }

    public enum AvalibleType
    {
        Float,
        String,
        Int,
        Bool,
        Color,
        Vector2,
        Vector3
    }

    public static AvalibleType? TypeToEnum(Type type)
    {
        if (type == typeof(string))
            return AvalibleType.String;
        if (type == typeof(bool))
            return AvalibleType.Bool;
        if (type == typeof(int))
            return AvalibleType.Int;
        if (type == typeof(float))
            return AvalibleType.Float;
        if (type == typeof(Color))
            return AvalibleType.Color;
        if (type == typeof(Vector2))
            return AvalibleType.Vector2;
        if (type == typeof(Vector3))
            return AvalibleType.Vector3;
        throw new System.TypeAccessException("incorrect enum type");
    }
}


