using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RadialLayout))]
public class EditorRadialLayout : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        RadialLayout radialLayout = (RadialLayout)target;

        radialLayout.Awake();

        radialLayout.CalculateRadial();
    }
}
