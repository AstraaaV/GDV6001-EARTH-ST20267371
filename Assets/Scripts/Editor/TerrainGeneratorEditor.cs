using UnityEngine;
using UnityEditor;

public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TerrainGenerator terrain = (TerrainGenerator)target;

        GUILayout.Space(10);

        if(GUILayout.Button("Regenerate Terrain", GUILayout.Height(30)))
        {
            terrain.GenerateTerrain();
        }
    }
}
