using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Save to File"))
        {
            SaveFile.Save();
            SaveManager.SerialiseToFile();
        }

        if (GUILayout.Button("Load from File"))
        {
            SaveManager.DeserialiseFromFile();
        }
    }
}
