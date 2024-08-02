using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraController))]
public class CameraZoneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CameraController ctrl = (CameraController)target;
        if (GUILayout.Button("创建摄像机范围"))
        {
            // ctrl.GenZone();
        }
    }
}