using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PostProcessingController))]
public class PostProcessingControllerEditor : Editor
{
    private SerializedProperty m_GlobalVolumePrefab;
    private SerializedProperty m_RuntimeInstantiateVolume;

    private void OnEnable()
    {
        m_RuntimeInstantiateVolume = serializedObject.FindProperty("runtimeInstantiateVolume");
        m_GlobalVolumePrefab = serializedObject.FindProperty("globalVolumePrefab");
        m_GlobalVolumePrefab.objectReferenceValue = AssetDatabase.LoadAssetAtPath(
            "Assets/Prefabs/PostProcessing/Global Volume.prefab", typeof(GameObject));

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Please ensure that Post Processing is enabled in the Camera settings.", MessageType.Info);

        EditorGUILayout.PropertyField(m_RuntimeInstantiateVolume);
        if (!m_RuntimeInstantiateVolume.boolValue)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("globalVolume"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
