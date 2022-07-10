using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CombatSystemManager))]
public class CombatSystemManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CombatSystemManager script = (CombatSystemManager)target;

        EditorStyles.label.wordWrap = true;
        EditorGUILayout.LabelField("NOTE:");
        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField("Ensure you have a UnitManager, a TileMapFacade and a UnitStatusEffectsFacade in the scene.");
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();

        if (Application.isPlaying)
        {
            CombatSceneManager.Instance.SetGameOverAllowed(EditorGUILayout.Toggle("Allow Game Over State", CombatSceneManager.Instance.GameOverAllowed));
            CombatSceneManager.Instance.SetIsPaused(EditorGUILayout.Toggle("Pause Game", CombatSceneManager.Instance.IsPaused));
        }

        EditorGUILayout.Space();

        script.autocreateCombatSystemView = EditorGUILayout.Toggle("Autocreate", script.autocreateCombatSystemView);
        if (script.autocreateCombatSystemView)
        {
            script.combatSystemViewPrefab = 
                (GameObject)EditorGUILayout.ObjectField("Combat System View Prefab", script.combatSystemViewPrefab, typeof(GameObject), false);
        } 
        else
        {
            script.combatSystemView = 
                (GameObject)EditorGUILayout.ObjectField("Scene Combat System View", script.combatSystemView, typeof(GameObject), true);
        }
    }
}
