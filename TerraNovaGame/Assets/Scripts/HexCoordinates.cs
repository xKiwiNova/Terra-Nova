﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HexCoordinates
{
    public int q
    {
        get;
        private set;
    }


    public int r
    {
        get;
        private set;
    }

    public int s
    {
        get;
        private set;
    }

    public HexCoordinates(int q, int r)
    {
        this.q = q;
        this.r = r;
        this.s = -r - q;
    }

    public HexCoordinates(int q, int r, int s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
    }

    public static HexCoordinates[] neighbors =
    {
        new HexCoordinates(-1, 1, 0),
        new HexCoordinates(0, 1, -1),
        new HexCoordinates(1, 0, -1),
        new HexCoordinates(1, -1, 0),
        new HexCoordinates(0, -1, 1),
        new HexCoordinates(-1, 0, 1)
    };    

    public static HexCoordinates FromOffsetCoordinates(int x, int z)
    {
        int q = x;
        int r = z - (x + (x & 1) ) / 2;
        return new HexCoordinates(q, r);
    }

    public Vector2Int ToOffsetCoordinates()
    {
        int x = q;
        int z = r + (q + (q & 1)) / 2;
        return new Vector2Int(x, z);
    }

    public HexCoordinates GetNeighbor(HexDirection direction)
    {
        return Add(this, neighbors[(int)direction]);
    }

    public bool IsOnMap(HexMap map)
    {
        int x = this.ToOffsetCoordinates().x;
        int z = this.ToOffsetCoordinates().y;
        return map.IsOnMap(x, z);
    }

    
    public static HexCoordinates FromPosition(Vector3 position)
    {
        float x = position.x;
        float z = position.z;

        float q = (x * (2f / 3f) ) / Hexagon.outerRadius;
        float r = ((x * (-1f / 3f) ) + ( Mathf.Sqrt(3f) / 3f)  * z) / Hexagon.outerRadius;
        return RoundCoordinate(q, r);
    }

    public Vector3 GetPosition()
    {
        Vector3 position = new Vector3();

        position.x = (q * Hexagon.outerRadius * 1.5f);
        position.z = (q * Hexagon.innerRadius) + (r * 2f * Hexagon.innerRadius);
        return position;
    }

    public static HexCoordinates RoundCoordinate(float q, float r)
    {
        float s = -r - q;

        int newQ = Mathf.RoundToInt(q);
        int newR = Mathf.RoundToInt(r);
        int newS = Mathf.RoundToInt(s);

        // Sometimes rounding returns an invalid coordinate, this section should fix that
        // It corrects the coordinate with the most difference when rounding
        float qDiff = Mathf.Abs(newQ - q);
        float rDiff = Mathf.Abs(newS - s);
        float sDiff = Mathf.Abs(newR - r);

        if(qDiff > rDiff && qDiff > sDiff)
        {
            newQ = -newR - newS;
        }
        else if(rDiff > sDiff)
        {
            newR = -newQ - newS;
        }
        else
        {
            newS = -newQ - newR;
        }

        return new HexCoordinates(newQ, newR, newS);
    }

    public HexCoordinates Add(HexCoordinates coord1, HexCoordinates coord2)
    {
        return new HexCoordinates(coord1.q + coord2.q, coord1.r + coord2.r, coord1.s + coord2.s);
    }

    public override string ToString()
    {
        return  q.ToString() + ", " + r.ToString() + ", " + s.ToString();
    }
    public string ToColorString()
    {
        return "<#D00000>" + q.ToString() + "</color>, <#00D000>" + r.ToString() + "</color>, <#0000D0>" +  s.ToString() + "</color>";
    }

    public string ToStringOnSeperateLines() 
    {
        return q.ToString() + "\n" + r.ToString() + "\n" +  s.ToString();
    }

    public string ToColorStringOnSeperateLines()
    {
        return "<#D00000>" + q.ToString() + "</color>\n<#00D000>" + r.ToString() + "</color>\n<#0000D0>" +  s.ToString() + "</color>";
    }
}

