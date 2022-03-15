using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapManager))]
public class MapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        MapManager mapManager = target as MapManager;

        if (GUILayout.Button("Generate"))
        {
            mapManager.Init();
        }
    }
}
