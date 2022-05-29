using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new surface level", menuName = "Visuals/Surfae level")]
public class SurfaceLevel : ScriptableObject
{
    [SerializeField]
    private float _value = 1;
    
    public float Value { get => _value; set => _value = value; }

}
