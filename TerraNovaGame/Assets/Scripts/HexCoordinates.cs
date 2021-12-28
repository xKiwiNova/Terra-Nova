using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HexCoordinates
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
            return Z + X;
        }
    }

    public int Z
    {
        get;
        private set;
    }

    public HexCoordinates(int x, int z)
    {
        this.X = x;
        this.Z = z;
    }
    
    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        int newX = x;
        int newZ = z - (x / 2) - (x % 2);
        return new HexCoordinates(newX, newZ);
    }

    public Vector2Int ToOffsetCoordinates()
    {
        int newX = X;
        int newZ = Z + (X / 2) + (X % 2);
        return new Vector2Int(newX, newZ);
    }

    public bool IsOnMap(HexMap map)
    {
        try {HexTile tile = map.FromHexCoordinates(this); return true;}
        catch {return false;}
    }

    public override string ToString()
    {
        return  X.ToString() + ", " + Y.ToString() + ", " + Z.ToString();
    }
    public string ToColorString()
    {
        return "<#D00000>" + X.ToString() + "</color>, <#00D000>" + Y.ToString() + "</color>, <#0000D0>" +  Z.ToString() + "</color>";
    }

    public string ToStringOnSeperateLines() 
    {
        return X.ToString() + "\n" + Y.ToString() + "\n" +  Z.ToString();
    }

    public string ToColorStringOnSeperateLines()
    {
        return "<#D00000>" + X.ToString() + "</color>\n<#00D000>" + Y.ToString() + "</color>\n<#0000D0>" +  Z.ToString() + "</color>";
    }
}

