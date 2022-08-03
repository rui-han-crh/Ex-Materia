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
            SerializedProperty dataToCompare = serializedObject.FindProperty("dataToCompare");
            for (int i = 0; i < dataToCompare.arraySize; i++)
            {
                SerializedProperty dataRead = dataToCompare.GetArrayElementAtIndex(i);
                SerializedProperty isUniversalSingletonData = dataRead.FindPropertyRelative("isUniversalSingletonData");

                isUniversalSingletonData.boolValue = EditorGUILayout.Toggle("Is Universal Singleton", isUniversalSingletonData.boolValue);


                SerializedProperty gameObjectData = dataRead.FindPropertyRelative("gameObjectData");
                if (!isUniversalSingletonData.boolValue)
                {
                    gameObjectData.objectReferenceValue = 
                        EditorGUILayout.ObjectField("Scene GameObject", gameObjectData.objectReferenceValue, typeof(GameObject), true) as GameObject;
                } 
                else
                {
                    gameObjectData.objectReferenceValue = null;
                }

                SerializedProperty monoBehaviourScript = dataRead.FindPropertyRelative("monoBehaviourScript");

                monoBehaviourScript.objectReferenceValue
                    = EditorGUILayout.ObjectField("MonoBehaviour", monoBehaviourScript.objectReferenceValue, typeof(MonoBehaviour), true) as MonoBehaviour;

                EditorGUILayout.PropertyField(dataRead.FindPropertyRelative("variableName"));

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Expected Value");

                EditorGUILayout.PropertyField(dataRead.FindPropertyRelative("primitiveType"), GUIContent.none);
                
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                switch ((InduceEventByDataRead.Primitive)dataRead.FindPropertyRelative("primitiveType").enumValueIndex)
                {

                    case InduceEventByDataRead.Primitive.Integer:
                        EditorGUILayout.PropertyField(dataRead.FindPropertyRelative("expectedInt"), GUIContent.none);
                        //dataRead.ExpectedValue = EditorGUILayout.IntField(dataRead.ExpectedValue is int ? (int)dataRead.ExpectedValue : 0);
                        break;

                    case InduceEventByDataRead.Primitive.Float:
                        EditorGUILayout.PropertyField(dataRead.FindPropertyRelative("expectedFloat"), GUIContent.none);
                        //dataRead.ExpectedValue = EditorGUILayout.FloatField(dataRead.ExpectedValue is float ? (float)dataRead.ExpectedValue : 0);
                        break;

                    case InduceEventByDataRead.Primitive.String:
                        EditorGUILayout.PropertyField(dataRead.FindPropertyRelative("expectedString"), GUIContent.none);
                        //dataRead.ExpectedValue = EditorGUILayout.TextField(dataRead.ExpectedValue is string ? (string)dataRead.ExpectedValue : "");
                        break;

                    case InduceEventByDataRead.Primitive.Boolean:
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.PropertyField(dataRead.FindPropertyRelative("expectedBool"), GUIContent.none);
                        //dataRead.ExpectedValue = EditorGUILayout.Toggle(dataRead.ExpectedValue is bool ? (bool)dataRead.ExpectedValue : false);
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
