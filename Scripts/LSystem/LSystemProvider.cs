using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSystem;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Dynamic;

[CreateAssetMenu(fileName = "new LSystem provider", menuName = "LSystem/Provider", order = 0)]
public class LSystemProvider : ScriptableObject, ILSystemProvider
{
    [SerializeField]
    private TextAsset text;

    [SerializeField]
    private LSystemVisualiseType type;

    [SerializeField]
    private bool useRandomSeed = true;
    [SerializeField]
    private int seed;

    public LSystemVisualiseType Type => type;


    [SerializeField]
    private List<LSystemVariable> variables;

    private Dictionary<string, object> GetParameters()
    {
        var dict = new Dictionary<string, object>();
        foreach (var v in variables)
        {
            if (dict.ContainsKey(v.VariableName))
                throw new ArgumentException("each l-system variable name should be unique");
            dict[v.VariableName] = v.Value;
        }
        return dict;
    }

    public ILSystem Compile()
    {
        var dict = GetParameters();
        if (useRandomSeed)
            seed = UnityEngine.Random.Range(0, int.MaxValue);
        var xmlProvider = new XmlLSystemProvider(dict, seed).Parse(text.text);

        return xmlProvider.Compile();
    }

   
}
