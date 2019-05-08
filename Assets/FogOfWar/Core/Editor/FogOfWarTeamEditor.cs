using UnityEngine;
using UnityEditor;
using FoW;

[CustomEditor(typeof(FogOfWarTeam))]
public class FogOfWarTeamEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (Application.isPlaying && GUILayout.Button("Reinitialize"))
            ((FogOfWarTeam)target).Reinitialize();
    }
}
