using Facades;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitManager))]
public class UnitManagerEditor : Editor
{
    private SerializedProperty m_UnitFacades;
    private UnitManager script;

    private void OnEnable()
    {
        m_UnitFacades = serializedObject.FindProperty("unitFacades");
        script = (UnitManager)target;
        
        for (int i = 0; i < script.unitFacades.Length; i++)
        {
            if (!ReferenceEquals(script.unitFacades[i], null) ? false : (script.unitFacades[i] ? false : true))
            {
                m_UnitFacades.DeleteArrayElementAtIndex(i);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Remove All Units"))
        {
            foreach (UnitFacade uF in script.unitFacades)
            {
                if (uF != null)
                    DestroyImmediate(uF.gameObject);
            }
            script.unitFacades = new UnitFacade[0];
        }

        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(m_UnitFacades, true);

        if (GUILayout.Button("Add New Unit"))
        {
            GameObject unitGameObject = new GameObject($"Unit ({script.unitFacades.Length})", typeof(UnitFacade));
            unitGameObject.transform.SetParent(script.gameObject.transform);
            GameObject render = new GameObject("Render", typeof(SpriteRenderer));
            render.transform.SetParent(unitGameObject.transform);

            UnitFacade[] newArray = new UnitFacade[script.unitFacades.Length + 1];
            script.unitFacades.CopyTo(newArray, 0);
            newArray[script.unitFacades.Length] = unitGameObject.GetComponent<UnitFacade>();
            script.unitFacades = newArray;
        }

        Dictionary<Vector3, string> startingPositions = new Dictionary<Vector3, string>();

        foreach (UnitFacade unitFacade in script.unitFacades)
        {
            if (unitFacade == null)
            {
                continue;
            }

            if (!startingPositions.ContainsKey(unitFacade.transform.position))
            {
                startingPositions.Add(unitFacade.transform.position, unitFacade.name);
            }
            else
            {
                EditorGUILayout.Space(10);
                EditorGUI.indentLevel++;

                GUIStyle style = new GUIStyle();
                style.richText = true;
                style.wordWrap = true;

                EditorGUILayout.LabelField("<color=yellow><b>WARNING:</b>" +
                    $"\n{unitFacade.name} and {startingPositions[unitFacade.transform.position]} occupy the same space. " +
                    $"Please distance the two units such that they occupy different locations.</color> ", 
                    style);
                EditorGUI.indentLevel--;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
