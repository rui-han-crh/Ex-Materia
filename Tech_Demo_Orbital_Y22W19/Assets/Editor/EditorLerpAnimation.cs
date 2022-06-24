using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Extensions;

[CustomEditor(typeof(LerpAnimation))]
public class EditorLerpAnimation : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LerpAnimation lerpAnimation = (LerpAnimation)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Set Current Centre as Active"))
        {
            lerpAnimation.SetActiveAnchor(lerpAnimation.GetComponent<RectTransform>().CenterAnchor());
        }

        if (GUILayout.Button("Set Current Centre as Inactive"))
        {
            lerpAnimation.SetInactiveAnchor(lerpAnimation.GetComponent<RectTransform>().CenterAnchor());
        }

        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Preview Active Positon"))
        {
            lerpAnimation.Awake();
            lerpAnimation.GetComponent<RectTransform>().SetCenterAnchor(lerpAnimation.ActiveAnchor);
        }

        if (GUILayout.Button("Preview Inactive Positon"))
        {
            lerpAnimation.Awake();
            lerpAnimation.GetComponent<RectTransform>().SetCenterAnchor(lerpAnimation.InactiveAnchor);
        }

        GUILayout.EndHorizontal();
    }
}
