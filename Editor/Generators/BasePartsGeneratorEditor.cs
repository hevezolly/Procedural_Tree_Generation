using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SinglePartGenerator), editorForChildClasses: true)]
public class BasePartsGeneratorEditor : Editor
{
    private SinglePartGenerator generator;
    private void OnEnable()
    {
        generator = target as SinglePartGenerator;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (generator == null)
            return;
        if (GUILayout.Button("Generate"))
            generator.GeneratePartsMesh();
        if (GUILayout.Button("Clear"))
            generator.ClearMesh();
    }
}
