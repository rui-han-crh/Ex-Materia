using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PositionOnTileMap))]
public class EditorPositionOnTilemap : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PositionOnTileMap script = (PositionOnTileMap)target;

        script.positionInGridSpace = script.GetCellPositionOnTilemap();
        script.positionInWorldSpace = script.GetWorldPositionOnTilemap();
    }
}
