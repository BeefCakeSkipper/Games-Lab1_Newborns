using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerMovement))]
public class SkipToNextScene : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerMovement player = (PlayerMovement)target;

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Next Scene"))
        {
            player.NextScene();
        }

        GUI.enabled = true;
    }
}
