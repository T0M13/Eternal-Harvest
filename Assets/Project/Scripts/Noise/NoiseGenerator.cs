using UnityEngine;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity, Vector2 offset, int seed, int numThresholds)
    {
        float[,] noiseMap = new float[width, height];

        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-100000, 100000) + offset.x;
            float offsetY = rng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                // Normalize the noise value from -1 to 1 to a range of 0 to 1
                noiseHeight = Mathf.InverseLerp(-1, 1, noiseHeight);

                // Apply thresholds based on the number of thresholds provided
                noiseMap[x, y] = ApplyThresholds(noiseHeight, numThresholds);
            }
        }

        return noiseMap;
    }

    // Helper function to map noise values to discrete thresholds
    private static float ApplyThresholds(float noiseValue, int numThresholds)
    {
        // Divide the range [0, 1] into numThresholds intervals
        float stepSize = 1.0f / numThresholds;

        // Find the closest threshold band the noise value fits in
        return Mathf.Floor(noiseValue / stepSize) * stepSize;
    }
}
