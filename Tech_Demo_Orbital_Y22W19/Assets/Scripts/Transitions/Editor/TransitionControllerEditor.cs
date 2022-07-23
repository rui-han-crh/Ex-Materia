using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Transitions;

[CustomEditor(typeof(TransitionController))]
public class TransitionControllerEditor : Editor
{
    private TransitionController transitionController;

    public void OnEnable()
    {
        transitionController = (TransitionController)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Preview Active State"))
        {
            transitionController.SetAllTransitions(true);
        }

        if (GUILayout.Button("Preview Inactive State"))
        {
            transitionController.SetAllTransitions(false);
        }
        EditorGUILayout.EndHorizontal();
    }
}
