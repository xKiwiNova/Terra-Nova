using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Static class which stores important data for map generation
public static class Hexagon
{
    public static float outerRadius = 5; // This is an arbitrary number that I picked as the size of the hexagons
    public static float innerRadius = outerRadius * (Mathf.Sqrt(3f) / 2f);

    public static int chunkSizeX = 5;
    public static int chunkSizeZ = 5;

    public static float solidFactor = 0.7f;
    public static float blendFactor = 1 - solidFactor;

    public static float elevationStep = 1.5f;

    public static Vector3[] corners = 
    {
        new Vector3(-outerRadius, 0, 0),
        new Vector3(-outerRadius / 2f, 0, innerRadius),
        new Vector3(outerRadius / 2f, 0, innerRadius),
        new Vector3(outerRadius, 0, 0),
        new Vector3(outerRadius / 2f, 0, -innerRadius),
        new Vector3(-outerRadius / 2f, 0, -innerRadius),
        new Vector3(-outerRadius, 0, 0)
    };

    public static Vector3 GetCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)(direction.Next())];
    }

    public static Vector3 GetSolidCorner(HexDirection direction)
    {
        return GetCorner(direction) * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return GetSecondCorner(direction) * solidFactor;
    }
}
