using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Managers;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(TileManager))]
public class TileManagerEditor : Editor
{
    private SerializedProperty m_IndicatorMap;
    private SerializedProperty m_GroundTileMaps;
    private SerializedProperty m_HalfCoverTileMaps;
    private SerializedProperty m_FullCoverTileMaps;

    public void OnEnable()
    {
        m_IndicatorMap = serializedObject.FindProperty("indicatorMap");
        m_GroundTileMaps = serializedObject.FindProperty("groundTilemaps");
        m_HalfCoverTileMaps = serializedObject.FindProperty("halfCoverTilemaps");
        m_FullCoverTileMaps = serializedObject.FindProperty("fullCoverTilemaps");
    }

    public override void OnInspectorGUI()
    {
        TileManager script = (TileManager)target;
        script.tileDatabase = (TileDatabase)AssetDatabase.LoadAssetAtPath(
            "Assets/Prefabs/CombatSystem/TileDatabase/TileDatabase.asset", typeof(TileDatabase));

        EditorGUILayout.PropertyField(m_IndicatorMap, true);
        EditorGUILayout.Space(10);
        EditorGUI.indentLevel++;

        GUIStyle style = new GUIStyle();
        style.richText = true;
        style.wordWrap = true;

        EditorGUILayout.LabelField("<color=white><b>Note:</b>" +
            "\nYou must include at least <b>one</b> ground tilemap. Half cover and full cover tilemaps are optional.</color> ", style);
        EditorGUILayout.Space();

        if (m_GroundTileMaps.arraySize < 1)
        {
            EditorGUILayout.LabelField("<color=red><b>ERROR: </b>" +
            "You do not have at least one ground map assigned!</color> ", style);
        }

        EditorGUILayout.PropertyField(m_GroundTileMaps, true);
        EditorGUILayout.PropertyField(m_HalfCoverTileMaps, true);
        EditorGUILayout.PropertyField(m_FullCoverTileMaps, true);

        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
