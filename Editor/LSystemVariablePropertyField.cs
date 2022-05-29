using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LSystemVariable))]
public class LSystemVariablePropertyField : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 3;
       // return base.GetPropertyHeight(property, label) * 3;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var height = position.height / 3;
        var nameRect = new Rect(position.x, position.y, position.width, height);
        var typeRect = new Rect(position.x, position.y + height, position.width, height);
        var valueRect = new Rect(position.x, position.y + height * 2, position.width, height);
        var type = property.FindPropertyRelative("valueType");

        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("name"), new GUIContent());

        type.intValue = EditorGUI.Popup(typeRect, type.intValue, type.enumNames);
        var name = "_" + ((LSystemVariable.AvalibleType)type.intValue).ToString();

        var p = property.FindPropertyRelative(name);

        if (p != null)
            EditorGUI.PropertyField(valueRect, p, new GUIContent());
        else
            EditorGUI.LabelField(valueRect, "not found");
    }
}
