using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Static class which stores important data for Hexagonal tiles
public static class Hexagon
{
    // The distance between the center and the vertices
    public static float outerRadius = 5;
    // The distance between the center and the sides
    public static float innerRadius = outerRadius * (Mathf.Sqrt(3f) / 2f);

    // How many tiles in a chunk
    public static int chunkSizeX = 5;
    public static int chunkSizeZ = 5;

    // The solid part of a hexagon is the part that will always be flat.
    public static float solidFactor = 0.7f;
    // The blended part is the part that may be elevated or inclined
    public static float blendFactor = 1 - solidFactor;

    // The amount per "unit" of elevation that the tile is elevated.
    public static float elevationStep = 1.5f;
    public static float[,] noiseMap;

    // A list of vectors for the corner vertices in relation to the center
    public static Vector3[] corners = 
    {
        new Vector3(-outerRadius, 0, 0), // Starting at the first NW corner
        new Vector3(-outerRadius / 2f, 0, innerRadius),
        new Vector3(outerRadius / 2f, 0, innerRadius),
        new Vector3(outerRadius, 0, 0),
        new Vector3(outerRadius / 2f, 0, -innerRadius),
        new Vector3(-outerRadius / 2f, 0, -innerRadius), // Ending at the first SW corner
        new Vector3(-outerRadius, 0, 0) // This is the first corner, also the last SW corner
    };

    // Gets the first corner of a triangle given the direction of the triangle
    public static Vector3 GetCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }


    // Gets the second corner of the aforementioned triangle
    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)(direction.Next())];
    }

    // Gets the solid corners
    public static Vector3 GetSolidCorner(HexDirection direction)
    {
        return GetCorner(direction) * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return GetSecondCorner(direction) * solidFactor;
    }

    public static float GetOffset(Vector3 position)
    {
        (int, int) index = ( Mathf.Abs((int)(position.x % 128)), Mathf.Abs((int)(position.z % 128)) );
        return (noiseMap[index.Item1, index.Item2]);
    }
}
