using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HexTile : MonoBehaviour
{
    public HexChunk chunk;
    public int x;
    public int z;

    public Vector3 position;
    private int elevation;

    public HexCoordinates hexCoordinates;

    public HexMap map;

    public int Elevation
    {
        set
        {
            this.elevation = value;
            this.position.y = value * Hexagon.elevationStep;

            Vector3 newPosition = this.uiText.transform.localPosition;
            newPosition.z = -position.y;
            this.uiText.transform.localPosition = newPosition;
        }
        get
        {
            return this.elevation;
        }
    }

    public HexTile[] neighbors;

    public TextMeshProUGUI uiText;

    public Color Color
    {
        get
        {
            return tileColor;
        }
        set
        {
            tileColor = value;
            for(int i = 0; i < 6; i++)
            {
                try
                {
                    tileColors[i] = tileColor * Random.Range(0.85f, 1.15f);
                    tileColors[i].a = tileColor.a;
                } catch{}
            }
        }
    }

    private Color tileColor;
    public Color[] tileColors;

    public void InstantiateHexTile(int x, int z, HexMap map, HexChunk chunk)
    {
        this.x = x;
        this.z = z;
        map.tiles[x, z] = this;

        position = new Vector3(
            (x * Hexagon.outerRadius * 1.5f) + Hexagon.outerRadius, // Since the tiles interlock, only a 1.5 offset is needed
            0f, (z * Hexagon.innerRadius * 2f) + Hexagon.innerRadius);

        if(x % 2 == 0) 
        {
            position.z += Hexagon.innerRadius; // This creates alternating interlocking collumns
        }
        this.transform.localPosition = position;
        this.map = map;

        this.hexCoordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        this.Color = new Color(1, 1, 1, 1);
        this.chunk = chunk;
        this.neighbors = new HexTile[6];
        this.tileColors = new Color[6];
    }

    public Color GetColor(HexDirection direction)
    {
        return this.tileColors[(int)direction];
    }

    public Vector3 GetCorner(HexDirection direction)
    {
        return Hexagon.GetCorner(direction) + position;
    }

    public Vector3 GetSecondCorner(HexDirection direction)
    {
        return Hexagon.GetSecondCorner(direction) + position;
    }

    public Vector3 GetSolidCorner(HexDirection direction)
    {
        return Hexagon.GetSolidCorner(direction) + position;
    }

    public  Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return Hexagon.GetSecondSolidCorner(direction) + position;
    }

    public Vector3 GetCliffCorner(HexDirection direction, int elevation)
    {
        return Hexagon.GetCliffCorner(direction, (this.Elevation - elevation)) + position;
    }

    public Vector3 GetSecondCliffCorner(HexDirection direction, int elevation)
    {
        return Hexagon.GetSecondCliffCorner(direction, (this.Elevation - elevation)) + position;
    }
    
    public void SetNeighbor(HexDirection direction, HexTile tile)
    {
        neighbors[(int)direction] = tile;
    }

    public HexTile GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public override string ToString()
    {
        return $"Tile{(x, z)}";
    }
}