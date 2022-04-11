using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Generates the tiles and chunks of the map
public class HexMap : MonoBehaviour
{
    public int chunkCountX = 3;
    public int tileCountX;
    public int chunkCountZ = 3;
    public int tileCountZ;

    // The map stores a 2D array of chunks, which each store a 2D of their tiles
    public HexChunk[,] chunks;
    public HexTile[,] tiles;

    // These "prefabs" (not actual prefabs) will be instantiated when the map is generated
    public TextMeshProUGUI tileLabelPrefab;
    public Canvas mapCanvas;

    public HexTile hexTilePrefab;
    public HexChunk hexChunkPrefab;

    private bool showDebugText = true;

    [SerializeField]
    public bool ShowDebugText
    {
        get
        {
            return showDebugText;
        }

        set
        {
            showDebugText = value;
            foreach(HexTile tile in tiles)
            {
                if(tile.uiText != null)
                {
                    tile.uiText.enabled = value;
                }
            }
        }
    }

    public LayerMask layer;
    public Color32 color;

    void Awake()
    {
        color = Color.HSVToRGB(
            Random.Range(.01f, .99f), 
            Random.Range(.8f, .9f),
            Random.Range(.7f, .9f));

        tileCountX = chunkCountX * Hexagon.chunkSizeX;
        tileCountZ = chunkCountZ * Hexagon.chunkSizeZ;

        // Initializing the list of tiles and chunks
        chunks = new HexChunk[chunkCountX, chunkCountZ];
        tiles = new HexTile[tileCountX, tileCountZ];

        float[,] elevationNoiseMap = NoiseMap.GenerateNoiseMap(
            128, 128, 
            Random.Range(0, 99999), 
            Random.Range(10.0f, 20.0f), 
            4, .5f, 2);
        Hexagon.noiseMap = elevationNoiseMap;

        GenerateChunks();
        MakeDebugText();
        SetNeighbors();
        ApplyElevation();

        Triangulate();
    }

    // Creates each chunk
    void GenerateChunks()
    {
        for(int x = 0; x < chunkCountX; x++)
        {
            for(int z = 0; z < chunkCountZ; z++)
            {
                HexChunk chunk = chunks[x, z] = Instantiate(hexChunkPrefab);
                chunk.InstantiateHexChunk(x, z, this);
                chunk.name = chunk.ToString();
                chunk.transform.SetParent(this.transform, false);
            }
        }
    }
    
    // Generates a marker for each tile that can show information like Coordinates
    void MakeDebugText()
    {
        foreach(HexTile tile in tiles)
        {
            TextMeshProUGUI text = Instantiate<TextMeshProUGUI>(tileLabelPrefab);
            text.rectTransform.SetParent(mapCanvas.transform, false);
            text.rectTransform.anchoredPosition = new Vector2(tile.position.x - 0.5f, tile.position.z + 1f);

            text.text = tile.hexCoordinates.ToColorString();
            text.name = $"Label {tile.ToString()}";
            tile.uiText = text;

            // Can be set to inactive
            if(!showDebugText) { tile.uiText.enabled = false; }
        }
    }

    // Sets the neighbors of each tile
    void SetNeighbors()
    {
        foreach(HexTile tile in tiles)
        {
            tile.SetNeighbors();
        }
    }

    // Applies elevation to each tile based on a Noise Map
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
                // tile.Color = Color.HSVToRGB((tile.Elevation - 1) / 8.0f, 0.75f, 1f);
            }
        }

        foreach(HexTile tile in tiles)
        {
            foreach(HexTile neighbor in tile.neighbors)
            {
                if(neighbor != null && tile.Elevation > 1 + neighbor.Elevation)
                {
                    tile.Elevation--;
                    tile.Color = Color.HSVToRGB((tile.Elevation - 1) / 8.0f, 0.75f, 1f);
                }
            }
        }
    }

    // Generates the map mesh
    public void Triangulate()
    {
        foreach(HexChunk chunk in chunks)
        {
            chunk.Triangulate();
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                // Debug.Log(HexCoordinates.FromPosition(hit.point));
            }
        }
    }

    // Returns a tile based on its HexCoordinates
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

    public bool IsOnMap(int x, int z)
    {
        bool validX = (x >= 0 && x < tileCountX);
        bool validZ = (z >= 0 && z < tileCountZ);

        return validX && validZ;
    }

    // Returns a tile based on a Vector3
    public HexTile FromPosition(Vector3 position)
    {
        return this.FromHexCoordinates(HexCoordinates.FromPosition(position));
    }
}
