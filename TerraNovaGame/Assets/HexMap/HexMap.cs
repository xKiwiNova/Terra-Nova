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

    [SerializeField]
    private bool showDebugText = true;

    public int maxPrec;

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

    public Color32 grassColor;
    public Color32 leafColor;
    public Color32 rockColor = new Color32(220, 210, 160, 255);
    public Material leafMaterial;

    public HexForest hexForest;

    void Awake()
    {
        grassColor = Color.HSVToRGB(
            Random.Range(.01f, .99f), 
            Random.Range(.8f, .9f),
            Random.Range(.8f, .9f));
        
        leafColor = grassColor * new Color(.9f, .9f, .9f);
        leafMaterial.SetColor("_BaseColor", leafColor);

        tileCountX = chunkCountX * Hexagon.chunkSizeX;
        tileCountZ = chunkCountZ * Hexagon.chunkSizeZ;

        // Initializing the list of tiles and chunks
        chunks = new HexChunk[chunkCountX, chunkCountZ];
        tiles = new HexTile[tileCountX, tileCountZ];

        float[,] elevationNoiseMap = NoiseMap.GenerateNoiseMap(
            1080, 1080, 
            Random.Range(0, 99999), 
            Random.Range(10.0f, 20.0f), 
            4, .5f, 2);
        
        Hexagon.noiseMap = elevationNoiseMap;

        GenerateChunks();
        MakeDebugText();
        SetNeighbors();
        ApplyElevation();
        ApplyClimateData();

        Triangulate();
        hexForest.MakeForest();
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

            text.text = tile.x + ", " + tile.z;
            text.name = $"Label {tile.ToString()}";
            tile.uiText = text;
            text.transform.SetParent(tile.transform);

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
                tile.Elevation = (int)(Mathf.Clamp(Mathf.Round(elevationNoiseMap[x, z]), 0, 1));
                if(tile.Elevation == 0)
                {
                    tile.Color = rockColor;
                }
            }
        }

        foreach(HexTile tile in tiles)
        {
            foreach(HexTile neighbor in tile.neighbors)
            {
                if(neighbor != null && tile.Elevation > 1 + neighbor.Elevation)
                {
                    tile.Elevation--;
                }

                if(neighbor != null && tile.Elevation != neighbor.Elevation)
                {
                    tile.numBorderElevations++;
                }
            }
        }
    }

    void ApplyClimateData()
    {
        float[,] precipitationNoiseMap = NoiseMap.GenerateNoiseMap(
            tileCountX, tileCountZ, 
            Random.Range(0, 99999), 
            Random.Range(10.0f, 20.0f), 
            4, .5f, 2);
        
        for(int x = 0; x < tileCountX; x++)
        {
            for(int z = 0; z < tileCountZ; z++)
            {
                HexTile tile = GetTile(x, z);
                tile.precipitation = (int)(Mathf.Clamp(Mathf.Round(precipitationNoiseMap[x, z] * maxPrec), 1, maxPrec));
                
                //float temp = tile.precipitation / 12f;
                //tile.Color = new Color(1 - temp, 1 - temp, 1);
                if(tile.Elevation == 0)
                {
                    tile.Color = rockColor;
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

    public bool IsOnMap(HexCoordinates coordinates)
    {
        int x = coordinates.ToOffsetCoordinates().x;
        int z = coordinates.ToOffsetCoordinates().y;

        return IsOnMap(x, z);
    }

    public bool IsOnMap(Vector3 position)
    {
        return IsOnMap(HexCoordinates.FromPosition(position));
    }

    // Returns a tile based on a Vector3
    public HexTile FromPosition(Vector3 position)
    {
        return this.FromHexCoordinates(HexCoordinates.FromPosition(position));
    }

}
