using PointCloudExporter;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        Generator script = (Generator)target;

        GUILayout.Label("Initializing", EditorStyles.boldLabel);
        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }

        if (GUILayout.Button("Reset"))
        {
            script.Reset();
        }
    }
}

