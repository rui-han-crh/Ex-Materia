using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ModifySaveData))]
public class ModifySaveDataEditor : Editor
{
    private SerializedProperty m_componentName;
    private SerializedProperty m_variableName;
    private SerializedProperty m_variableValue;
    private SerializedProperty m_valueType;

    private ModifySaveData script;

    private void OnEnable()
    {
        script = (ModifySaveData)target;
        m_componentName = serializedObject.FindProperty("componentName");
        m_variableName = serializedObject.FindProperty("variableName");

        m_valueType = serializedObject.FindProperty("valueType");

        SwitchValueType();
    }

    public override void OnInspectorGUI()
    {

        m_valueType.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup("Value Type", (ModifySaveData.ValueTypes)m_valueType.enumValueIndex));
        SwitchValueType();

        EditorGUILayout.PropertyField(m_componentName);
        EditorGUILayout.PropertyField(m_variableName);
        EditorGUILayout.PropertyField(m_variableValue);

        serializedObject.ApplyModifiedProperties();
    }

    private void SwitchValueType()
    {
        switch ((ModifySaveData.ValueTypes)m_valueType.enumValueIndex)
        {
            case ModifySaveData.ValueTypes.Bool:
                m_variableValue = serializedObject.FindProperty("boolValue");
                break;

            case ModifySaveData.ValueTypes.Integer:
                m_variableValue = serializedObject.FindProperty("intValue");
                break;

            case ModifySaveData.ValueTypes.Float:
                m_variableValue = serializedObject.FindProperty("floatValue");
                break;

            case ModifySaveData.ValueTypes.String:
                m_variableValue = serializedObject.FindProperty("stringValue");
                break;
        }
    }
}
