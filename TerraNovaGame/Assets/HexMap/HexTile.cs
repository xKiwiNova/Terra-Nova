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

    public List<GameObject> forestElements;

    // When changing elevation, this also motifies the transform and debugText
    public int Elevation
    {
        set
        {
            this.elevation = value;
            this.position.y = value * Hexagon.elevationStep;

            Vector3 newPosition = this.uiText.transform.localPosition;
            newPosition.z = -position.y;
            this.uiText.transform.localPosition = newPosition;

            this.transform.localPosition = position;
        }
        get
        {
            return this.elevation;
        }
    }

    public HexTile[] neighbors;

    public TextMeshProUGUI uiText;

    public Transform building;

    // Each triangle is a slightly different color
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
                tileColors[i] = tileColor * tileColorModifiers[i];
                tileColors[i].a = tileColor.a;
            }
        }
    }

    public int precipitation;
    public int temperature;
    public int fertility;

    private Color tileColor;
    public float[] tileColorModifiers;
    public Color[] tileColors;
    public int numBorderElevations;

    public void InstantiateHexTile(int x, int z, HexMap map, HexChunk chunk)
    {
        this.x = x;
        this.z = z;
        map.tiles[x, z] = this;
        this.hexCoordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        position = hexCoordinates.GetPosition();
        this.transform.localPosition = position;
        
        this.map = map;
        this.chunk = chunk;

        this.neighbors = new HexTile[6];

        tileColorModifiers = new float[6];
        for(int i = 0; i < 6; i++)
        {
            tileColorModifiers[i] = Random.Range(.95f, 1.05f);
        }

        this.tileColors = new Color[6];
        this.Color = new Color(1, 1, 1, 1);
        forestElements = new List<GameObject>();
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

    public void SetNeighbors()
    {
        for(HexDirection direction = HexDirection.NW; direction <= HexDirection.SW; direction++)
        {
            HexCoordinates neighborCoords = hexCoordinates.GetNeighbor(direction);
            if(neighborCoords.IsOnMap(map))
            {
                SetNeighbor(direction, map.FromHexCoordinates(neighborCoords));
            }
        }
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