using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HexCoords
{
    public int X
    {
        get;
        private set;
    }

    public int Y
    {
        get
        {
            return -X - Z;
        }
    }

    public int Z
    {
        get;
        private set;
    }

    // The Hex Coordinates are stored on 3 axes: X (Northeast-Southwest), Y (Northwest-Southeast) Z (East-West),
    public HexCoords(int x, int z)
    {
        X = x;
        Z = z;
    }

    public static HexCoords FromOffsetCoordinates(int x, int z)
    {
        return new HexCoords((x - (z/2)), z); // Undoing the horizontal shift 
    }

    public override string ToString()
    {
        return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
    }

    public string ToStringOnSeperateLines() 
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" +  Z.ToString();
    }

    public string ToColorStringOnSeperateLines()
    {
        return "<#D00000>" + X.ToString() + "</color>\n<#00D000>" + Y.ToString() + "</color>\n<#0000D0>" +  Z.ToString() + "</color>";
    }

    public static HexCoords FromPosition(Vector3 position)
    {
        float offset = position.z / (HexMetrics.outerRadius * 3f);
        float x = (position.x / (HexMetrics.innerRadius * 2f)); // Getting x based on diameter.
        float y = -x;

        x -= offset;
        y -= offset;

        int xInt = Mathf.RoundToInt(x);
        int yInt = Mathf.RoundToInt(y);
        int zInt = Mathf.RoundToInt(-x - y);

        // If there is a rounding error, the coordinate with the largest rounding difference is discarded and recalculated from the other two.
        if(xInt + yInt + zInt != 0)
        {
            float xFloat = Mathf.Abs(x - xInt);
            float yFloat = Mathf.Abs(y - yInt);
            float zFloat = Mathf.Abs(-x - y - zInt);

            if(xFloat > yFloat && xFloat > zFloat)
            {
                xInt = -yInt - zInt;
            }
            else if(zFloat > yFloat)
            {
                zInt = -xInt - yInt;
            }
        }

        return new HexCoords(xInt, zInt);
    }
}
