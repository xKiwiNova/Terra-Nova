using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum for storing the directions of a Hexagon's triangles
public enum HexDirection
{
    NW, N, NE, SE, S, SW
}

public static class HexDirectionExtensions
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static HexDirection Previous(this HexDirection direction) 
    {
		return direction == HexDirection.NW ? HexDirection.SW : (direction - 1);
	}

	public static HexDirection Next(this HexDirection direction) 
    {
		return direction == HexDirection.SW ? HexDirection.NW : (direction + 1);
	}
}
