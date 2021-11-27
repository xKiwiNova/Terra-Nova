using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HexGrid : MonoBehaviour
{
    private int cellCountX;
    private int cellCountZ;

    public int chunkCountX = 4;
    public int chunkCountZ = 4;

    public HexCell cellPrefab;
    HexCell[] cells;

    public HexGridChunk chunkPrefab;
    HexGridChunk[] chunks;

    public TextMeshProUGUI cellLabelPrefab;
    bool debugMode = true;

    public Color defaultColor = Color.white;
    public Color touchedColor = Color.green;

    float[,] elevationNoiseMap;
    float[,] precipitationNoiseMap;
    float[,] temperatureNoiseMap;

    
    void Awake()
    {   
        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        
        elevationNoiseMap = NoiseMap.GenerateNoiseMap(
            cellCountX, cellCountZ, 
            Random.Range(0, 99999), 
            Random.Range(10.0f, 20.0f), 
            4, .5f, 2);

        precipitationNoiseMap = NoiseMap.GenerateNoiseMap(
            cellCountX, cellCountZ, 
            Random.Range(0, 99999), 
            Random.Range(10.0f, 20.0f), 
            4, .5f, 2);
        
        temperatureNoiseMap = NoiseMap.GenerateNoiseMap(
            cellCountX, cellCountZ, 
            Random.Range(0, 99999), 
            Random.Range(15.0f, 25.0f), 
            4, .5f, 2);

        CreateChunks();
        CreateCells();
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for(int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for(int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(transform);
                chunk.name = $"Chunk ({x}, {z})";
            }
        }
    }

    void CreateCells()
    {
         cells = new HexCell[cellCountX * cellCountZ];

        for(int z = 0, i = 0; z < cellCountZ; z++)
        {
            for(int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    void OnEnable()
    {
        float[,] map1 = NoiseMap.GenerateNoiseMap(
            512, 512, 
            Random.Range(0, 99999), 
            Random.Range(5.0f, 10.0f), 
            4, .5f, 2);
            
        float[,] map2 = NoiseMap.GenerateNoiseMap(
            512, 512, 
            Random.Range(0, 99999), 
            Random.Range(5.0f, 10.0f), 
            4, .5f, 2);

        float[,] map3 = NoiseMap.GenerateNoiseMap(
            512, 512, 
            Random.Range(0, 99999), 
            Random.Range(5.0f, 10.0f), 
            4, .5f, 2);

        float[,] map4 = NoiseMap.GenerateNoiseMap(
            512, 512, 
            Random.Range(0, 99999), 
            Random.Range(5.0f, 10.0f), 
            4, .5f, 2);

        Vector4[] noiseData = new Vector4[512]; 

        for(int i = 0; i < 512; i++)
        {
            for(int j = 0; j < 512; j++)
            {
                HexMetrics.noiseMap.SetPixel(i, j, new Color(map1[i, j], map2[i, j],  map3[i, j],  map4[i, j]));
            }
        }
    }

    void Update()
    {
    
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates hexCoordinates = HexCoordinates.FromPosition(position);
        int index = hexCoordinates.X + hexCoordinates.Z * cellCountX + hexCoordinates.Z / 2;
        return cells[index];
    
    }

    // Given an inputed (x, z), the program can instantiate a new cell and append it to the list of cells.
    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        // The center of each cell is 2 * radius away from the center of the next cell in the x didrection
        // Uses interger division to displace evry other row
        position.x = (x + (z * 0.5f)  - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);

        // cell.transform.SetParent(transform, false); // The transforms worldposition is relative, not absolute.
        cell.transform.localPosition = position;

        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        // Creating the terrain data
        cell.Color = defaultColor;

        

        // Creating Neighbors

        if(x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }

        if(z > 0)
        {
            if(z % 2 == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if(x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if(x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        cell.name = $"Cell {cell.coordinates.ToString()}";

        // Shows the Coordinates of each tile
        TextMeshProUGUI text = Instantiate<TextMeshProUGUI>(cellLabelPrefab);
        // text.rectTransform.SetParent(gridCanvas.transform, false);
        text.rectTransform.anchoredPosition = new Vector2(position.x - 3.5f, position.z - 4f);
		text.text = cell.coordinates.ToColorStringOnSeperateLines();
        text.name = $"Label {cell.coordinates.ToString()}";
        cell.debugUIRect = text.rectTransform;

        if(!debugMode)
        {
            text.enabled = false;
        }

        int elevation = (int)((elevationNoiseMap[x, z] + 0.02f) * 12);
        int precipitation = (int)((precipitationNoiseMap[x, z] + 0.02f) * 12);
        int temperature = (int)((temperatureNoiseMap[x, z] + 0.02f) * 12);

        cell.GenerateTerrainData();
        cell.UpdateTerrainData(elevation, precipitation, temperature);
        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;

        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }
}
