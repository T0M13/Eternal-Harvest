#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGenerator = (MapGenerator)target;

        // Default inspector
        DrawDefaultInspector();

        // Noise showcase toggle
        if (mapGenerator.showNoise && mapGenerator.NoiseTexture != null)
        {
            GUILayout.Label("Noise Preview:");
            GUILayout.Box(mapGenerator.NoiseTexture);
        }

        // Button to generate everything
        if (GUILayout.Button("Generate Everything"))
        {
            mapGenerator.GenerateEverything();
        }

        // Button to generate only placeholders
        if (GUILayout.Button("Generate Placeholders Only"))
        {
            mapGenerator.GeneratePlaceholdersOnly();
        }

        // Button to refresh the display
        if (GUILayout.Button("Refresh Display Only"))
        {
            mapGenerator.RefreshDisplayOnly();
        }

        if (GUILayout.Button("Clear Placeholder Map"))
        {
            mapGenerator.ClearPlaceHolderMap();
        }
    }
}
#endif
