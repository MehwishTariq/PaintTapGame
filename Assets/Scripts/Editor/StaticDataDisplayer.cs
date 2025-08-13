using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MaterialCreator))]
public class StaticDataDisplayer : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector (optional)
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Static List Data", EditorStyles.boldLabel);

        if (MaterialCreator.coloredMaterials != null && MaterialCreator.coloredMaterials.Count > 0)
        {
            for (int i = 0; i < MaterialCreator.coloredMaterials.Count; i++)
            {
                EditorGUILayout.LabelField($"{i + 1}. {MaterialCreator.coloredMaterials[i].ColorCount + " " + MaterialCreator.coloredMaterials[i].ColorName}");
            }
        }
        else
        {
            EditorGUILayout.LabelField("No data available.");
        }
    }
}
