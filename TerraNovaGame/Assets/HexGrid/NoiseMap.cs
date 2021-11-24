using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseMap
{
    public static float[,] GenerateNoiseMap(int width, int height, int seed, float scale, int octaves, float persistance, float lacunarity)
    {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for(int i = 0; i < octaves; i++)
        {
            float xOffset = prng.Next(-10000, 10000);
            float zOffset = prng.Next(-10000, 10000);
            octaveOffsets[i] = new Vector2(xOffset, zOffset);
        }

        float[,] noiseMap = new float[width, height];
        if(scale <= 0)
        {
            scale = .0001f + Mathf.Abs(scale);
        }

        float maxNoiseHeigt = float.MinValue;
        float minNoiseHeigt = float.MaxValue;

        for(int z = 0; z < height; z++)
        {
            for(int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency + octaveOffsets[i].x;
                    float sampleZ = z / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if(noiseHeight > maxNoiseHeigt)
                {
                    maxNoiseHeigt = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeigt)
                {
                    minNoiseHeigt = noiseHeight;
                }
                noiseMap[x,z] = noiseHeight;
            }
        }

        for(int z = 0; z < height; z++)
        {
            for(int x = 0; x < width; x++)
            {
                noiseMap[x, z] = Mathf.InverseLerp(maxNoiseHeigt, minNoiseHeigt, noiseMap[x,z]);
            }
        }

        return noiseMap;
    }
}
