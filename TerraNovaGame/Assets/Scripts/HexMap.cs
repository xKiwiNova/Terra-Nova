using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HexMap : MonoBehaviour
{
    public int chunkCountX = 3;
    public int tileCountX;
    public int chunkCountZ = 3;
    public int tileCountZ;

    public HexChunk[,] chunks;
    public HexTile[,] tiles;

    public MapGeneration mapGeneration;

    public TextMeshProUGUI tileLabelPrefab;
    public Canvas mapCanvas;

    void Awake()
    {
        tileCountX = chunkCountX * Hexagon.chunkSizeX;
        tileCountZ = chunkCountZ * Hexagon.chunkSizeZ;

        // Initializing the list of tiles
        chunks = new HexChunk[chunkCountX, chunkCountZ];
        tiles = new HexTile[tileCountX, tileCountZ];

        GenerateChunks();
        MakeDebugText();
        SetNeighbors();
        ApplyElevation();

        mapGeneration.Triangulate(chunks);
    }

    // Creates each chunk
    void GenerateChunks()
    {
        for(int x = 0; x < chunkCountX; x++)
        {
            for(int z = 0; z < chunkCountZ; z++)
            {
                chunks[x, z] = new HexChunk(x, z, this); // Needs to input the map the chunk is part of
            }
        }
    }
    
    void MakeDebugText()
    {
        foreach(HexTile tile in tiles)
        {
            TextMeshProUGUI text = Instantiate<TextMeshProUGUI>(tileLabelPrefab);
            text.rectTransform.SetParent(mapCanvas.transform, false);
            text.rectTransform.anchoredPosition = new Vector2(tile.position.x - 0.5f, tile.position.z + 1f);

            Vector2Int coords = tile.hexCoordinates.ToOffsetCoordinates();

            text.text = $"{tile.hexCoordinates.ToColorString()}";
            text.name = $"Label {tile.ToString()}";
            tile.uiText = text;
        }
    }

    void SetNeighbors()
    {
        foreach(HexTile tile in tiles)
        {
            int x = tile.hexCoordinates.X;
            int z = tile.hexCoordinates.Z;


            HexCoordinates coords = new HexCoordinates(x - 1, z + 1);
            if(coords.IsOnMap(this))
            {
                // Debug.Log($"{tile} : {coords} ({(HexDirection)i})");
                tile.SetNeighbor(HexDirection.NW, FromHexCoordinates(coords));
            }

            coords = new HexCoordinates(x, z + 1);
            if(coords.IsOnMap(this))
            {
                tile.SetNeighbor(HexDirection.N, FromHexCoordinates(coords));
            }

            coords = new HexCoordinates(x + 1, z);
            if(coords.IsOnMap(this))
            {
                tile.SetNeighbor(HexDirection.NE, FromHexCoordinates(coords));
            }

            coords = new HexCoordinates(x + 1, z - 1);
            if(coords.IsOnMap(this))
            {
                tile.SetNeighbor(HexDirection.SE, FromHexCoordinates(coords));
            }

            coords = new HexCoordinates(x, z - 1);
            if(coords.IsOnMap(this))
            {
                tile.SetNeighbor(HexDirection.S, FromHexCoordinates(coords));
            }

            coords = new HexCoordinates(x - 1, z);
            if(coords.IsOnMap(this))
            {
                tile.SetNeighbor(HexDirection.SW, FromHexCoordinates(coords));
            }
        }
    }

    void ApplyElevation()
    {
        float[,] elevationNoiseMap = NoiseMap.GenerateNoiseMap(
            tileCountX, tileCountZ, 
            Random.Range(0, 99999), 
            Random.Range(10.0f, 20.0f), 
            4, .5f, 2);
        for(int x = 0; x < tileCountX; x++)
        {
            for(int z = 0; z < tileCountZ; z++)
            {
                HexTile tile = GetTile(x, z);
                tile.Elevation = (int)(Mathf.Clamp((Mathf.Round(elevationNoiseMap[x, z] * 5) + 1), 1, 6));
                tile.color = Color.HSVToRGB((tile.Elevation - 1) / 8.0f, 0.75f, 1f);
            }
        }
    }
    
    public HexTile FromHexCoordinates(HexCoordinates coordinates)
    {
        return GetTile(coordinates.ToOffsetCoordinates().x, coordinates.ToOffsetCoordinates().y);
    }

    public HexChunk GetChunk(int x, int z)
    {
        return chunks[x, z];
    }

    public HexTile GetTile(int x, int z)
    {
       return tiles[x, z];
    }

    public HexTile FromPosition(Vector3 positon)
    {
        positon = transform.InverseTransformPoint(positon);
        return GetTile(1, 2);
    }
}
