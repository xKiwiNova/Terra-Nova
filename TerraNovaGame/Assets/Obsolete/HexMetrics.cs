using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script stores important data for how the game generates Hexagons
public static class HexMetrics
{
    public const float outerRadius = 10f; // The distance between the center to the corners
    public const float innerRadius = outerRadius * 0.866025404f; // The distance between the center to the sides (rt3 / 2)

    // 75% of the hexagon is a solid color, but 25% is blended with its neighbors
    public const float solidFactor = 0.9f;
    public const float blendFactor = 1 - solidFactor;

    public const float elevationStep = 1.5f;
    public const float elevationPerturbStrength = .5f;

    public static Vector3[,] noiseMap = new Vector3[512, 512];
    public static float cellPerturbStrength = 4f;

    public const int chunkSizeX = 6; 
    public const int chunkSizeZ = 6;

    public static Vector3[] corners =
    {
        // Starts at the top point, moving clockwise
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, .5f * outerRadius),
        new Vector3(innerRadius, 0f, -.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -.5f * outerRadius),
        new Vector3(-innerRadius, 0f, .5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    // Gets the first and second corner of a triangle given a direction 
    // IE; given NE, it will get the North and NE corners to form the NE triangle
    
    // The SolidCorner Methods will get the corner of the hexagon that isn't blended.
    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }

    public static Vector3 SampleNoise(Vector3 position)
    {
        // Debug.Log(noiseMap.GetPixel((int)position.x % 512, (int)position.z % 512));
        
        try
        {
            return noiseMap[Mathf.Abs((int)position.x % 512), Mathf.Abs((int)position.z % 512)];
        }
        catch
        {
            Debug.Log($"{(int)position.x % 512}, {(int)position.z % 512}");
            return new Vector3(.5f, .5f, .5f);
        }
    }
}
