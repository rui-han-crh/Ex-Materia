using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(InduceEventByDataRead))]
public class InduceEventByDataReadEditor : Editor
{
    SerializedProperty m_UnityEvent;

    public void OnEnable()
    {
        m_UnityEvent = serializedObject.FindProperty("unityEvent");
    }

    public override void OnInspectorGUI()
    {
        InduceEventByDataRead script = (InduceEventByDataRead)target;
        EditorGUILayout.PropertyField(m_UnityEvent, true);

        EditorGUILayout.BeginHorizontal();
        script.showList = EditorGUILayout.Foldout(script.showList, "Show Data To Compare", true);
        GUILayout.FlexibleSpace();
        EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth / 30;

        script.listSize = Mathf.Max(0, EditorGUILayout.DelayedIntField(script.dataToCompare.Count));
        EditorGUIUtility.labelWidth = 0;

        if (script.listSize < script.dataToCompare.Count)
        {
            script.dataToCompare.RemoveRange(script.listSize, script.dataToCompare.Count - script.listSize);
        }

        while (script.listSize > script.dataToCompare.Count)
        {
            script.dataToCompare.Add(new InduceEventByDataRead.DataRead());
        }

        EditorGUILayout.EndHorizontal();
        if (script.showList)
        {
            EditorGUI.indentLevel++;
            foreach (InduceEventByDataRead.DataRead dataRead in script.dataToCompare)
            {
                dataRead.isUniversalSingletonData = EditorGUILayout.Toggle("Is Universal Singleton", dataRead.isUniversalSingletonData);

                if (!dataRead.isUniversalSingletonData)
                {
                    dataRead.gameObjectData = EditorGUILayout.ObjectField("Scene GameObject", dataRead.gameObjectData, typeof(GameObject), true) as GameObject;
                } 
                else
                {
                    dataRead.gameObjectData = null;
                }

                dataRead.monoBehaviourScript = EditorGUILayout.ObjectField("MonoBehaviour", dataRead.monoBehaviourScript, typeof(MonoBehaviour), true) as MonoBehaviour;

                dataRead.variableName = EditorGUILayout.TextField("Variable Name", dataRead.variableName);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Expected Value");

                dataRead.primitiveType = (InduceEventByDataRead.Primitive)EditorGUILayout.EnumPopup(dataRead.primitiveType);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                switch (dataRead.primitiveType)
                {
                    case InduceEventByDataRead.Primitive.Integer:
                        dataRead.ExpectedValue = EditorGUILayout.IntField(dataRead.ExpectedValue is int ? (int)dataRead.ExpectedValue : 0);
                        break;

                    case InduceEventByDataRead.Primitive.Float:
                        dataRead.ExpectedValue = EditorGUILayout.FloatField(dataRead.ExpectedValue is float ? (float)dataRead.ExpectedValue : 0);
                        break;

                    case InduceEventByDataRead.Primitive.String:
                        dataRead.ExpectedValue = EditorGUILayout.TextField(dataRead.ExpectedValue is string ? (string)dataRead.ExpectedValue : "");
                        break;

                    case InduceEventByDataRead.Primitive.Boolean:
                        GUILayout.FlexibleSpace();
                        dataRead.ExpectedValue = EditorGUILayout.Toggle(dataRead.ExpectedValue is bool ? (bool)dataRead.ExpectedValue : false);
                        break;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                DrawUILine(Color.grey);
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel--;
        }

        this.serializedObject.ApplyModifiedProperties();
    }

    private void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        r.xMin += EditorGUI.indentLevel * 8;
        EditorGUI.DrawRect(r, color);
    }

}
