using CombatSystem.Facade;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombatSystemManager))]
public class CombatSystemManagerEditor : Editor
{
    private SerializedProperty m_AllowGameOver;
    private SerializedProperty m_CombatSystemViewObject;
    private SerializedProperty m_StatusEffectsDatabase;

    private void OnEnable()
    {
        m_AllowGameOver = serializedObject.FindProperty("allowGameOverState");
        m_CombatSystemViewObject = serializedObject.FindProperty("combatSystemView");
        m_StatusEffectsDatabase = serializedObject.FindProperty("statusEffectsDatabase");
    }

    public override void OnInspectorGUI()
    {
        CombatSystemManager script = (CombatSystemManager)target;

        EditorGUI.indentLevel++;

        GUIStyle style = new GUIStyle();
        style.richText = true;
        style.wordWrap = true;
        EditorGUILayout.LabelField("<color=white><b>Note:</b>\n" +
            "Before using this object, ensure you have a UnitManager and a TileManager in the scene.</color>", style);

        EditorGUILayout.Space();

        int numberOfUnitManagers = FindObjectsOfType<UnitManager>().Length;
        if (numberOfUnitManagers > 1)
        {
            EditorGUILayout.LabelField("<color=red><b>ERROR:</b> There are too many UnitManagers in this scene!</color>", style);
        }
        else if (numberOfUnitManagers < 1)
        {
            EditorGUILayout.LabelField("<color=red><b>ERROR:</b> There is no UnitManager in this scene!</color>", style);
        }

        int numberOfTileManagers = FindObjectsOfType<TileManager>().Length;
        if (numberOfTileManagers > 1)
        {
            EditorGUILayout.LabelField("<color=red><b>ERROR:</b> There are too many TileManagers in this scene!</color>", style);
        }
        else if (numberOfTileManagers < 1)
        {
            EditorGUILayout.LabelField("<color=red><b>ERROR:</b> There is no TileManagers in this scene!</color>", style);
        }

        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        if (Application.isPlaying)
        {
            CombatSceneManager.Instance.SetGameOverAllowed(EditorGUILayout.Toggle("Allow Game Over State", CombatSceneManager.Instance.GameOverAllowed));
            CombatSceneManager.Instance.SetIsPaused(EditorGUILayout.Toggle("Pause Game", CombatSceneManager.Instance.IsPaused));
        } 
        else
        {
            m_AllowGameOver.boolValue = EditorGUILayout.Toggle("Allow Game Over State", m_AllowGameOver.boolValue);
        }

        EditorGUILayout.Space();

        string word = m_CombatSystemViewObject.objectReferenceValue == null ? "Create" : "Recreate";

        if (GUILayout.Button($"{word} CombatSystemView"))
        {
            if (m_CombatSystemViewObject.objectReferenceValue != null)
            {
                DestroyImmediate(m_CombatSystemViewObject.objectReferenceValue);
            }

            GameObject csvObj = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(
                "Assets/Prefabs/CombatSystem/CombatSystemViewPrefab/CombatSystemView.prefab", typeof(GameObject)));

            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                canvas = Instantiate((GameObject)AssetDatabase.LoadAssetAtPath(
                    "Assets/Prefabs/Canvas/Canvas.prefab", typeof(GameObject)));
                canvas.name = "Canvas";
            }
            csvObj.transform.SetParent(canvas.transform, false);
            csvObj.name = "CombatSystemView";
            m_CombatSystemViewObject.objectReferenceValue = csvObj;
        }

        EditorGUILayout.PropertyField(m_CombatSystemViewObject, true);

        m_StatusEffectsDatabase.objectReferenceValue = (UnitStatusEffectsFacade)AssetDatabase.LoadAssetAtPath(
                "Assets/Prefabs/CombatSystem/Skills/_UnitStatusEffectsDatabase.asset", typeof(UnitStatusEffectsFacade));

        EditorGUILayout.Space();

        if (GUILayout.Button("Create BattleWonInteractable Object"))
        {
            GameObject battleWonInteractable = new GameObject("BattleWonInteractable", typeof(Interactable));
            battleWonInteractable.tag = "BattleWonInteractable";
        }

        if (GUILayout.Button("Create BattleLostInteractable Object"))
        {
            GameObject battleLostInteractable = new GameObject("BattleLostInteractable", typeof(Interactable));
            battleLostInteractable.tag = "BattleLostInteractable";
        }

        serializedObject.ApplyModifiedProperties();
    }
}
