using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMetrics {

    public const float outerRadius = 50f; // Outer radius, distance from the center to its corners.
    public const float innerRadius = outerRadius * 0.866025404f; // Inner radius, distance from the center to its side. sqrt(3)/2 * outer radius.
    public const float solidFactor = 0.75f; // The part of the hex that is a solid color
	public const float blendFactor = 1f - solidFactor; // The part of the hex that blends

    static Vector3[] corners = { // Stores  the corners of the hexagon as Vector3 positions
        new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius) // This is the same as the first corner, so that when the computer tries to make a hexagon and looks for the "7th" corner, it will come back to the first one.
    };

    public static Vector3 GetFirstCorner (HexDirection direction) { 
		return corners[(int)direction];
	}

	public static Vector3 GetSecondCorner (HexDirection direction) {
		return corners[(int)direction + 1];
	}

    public static Vector3 GetFirstSolidCorner (HexDirection direction) {
		return corners[(int)direction] * solidFactor;
	}

	public static Vector3 GetSecondSolidCorner (HexDirection direction) {
		return corners[(int)direction + 1] * solidFactor;
	}

    public static Vector3 GetBridge (HexDirection direction) {
		return (corners[(int)direction] + corners[(int)direction + 1]) *
			blendFactor;
	}
}
    
