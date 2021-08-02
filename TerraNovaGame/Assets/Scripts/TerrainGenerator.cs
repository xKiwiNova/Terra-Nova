using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject land;
    public Transform terrain;
    public Grid<TerrainObject> grid;

    public float offsetX;
    public float offsetZ;
    public float noiseScale;
    

    public float xPosition;
    public float zPosition;

    public float secondaryOffset;
    public float tertiaryOffset;

    public Material grassMaterial;
    private Color grassColor;
    public Material leafMaterial;
    private Color leafColor;

    public ForestElement[] forestElements;
    private int forestSize = 10;
    private float elementSpacing = 5;

    public GameObject debugPlane;
    

    

    public class TerrainObject
    {
        private Grid<TerrainObject> grid;
        private int x;
        private int z;
        private int terrainValue;
        private int temperature;
        private int precipitation;
        public List<GameObject> forestElementList = new List<GameObject>();

        public TerrainObject(Grid<TerrainObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void AddForestElement(GameObject forestElement)
        {
            this.forestElementList.Add(forestElement);
        }

        public void ClearForestElements()
        {
            foreach(GameObject forestElement in forestElementList)
            {
                Destroy(forestElement);
            }
            forestElementList.Clear();
        }

        public void SetTerrainValue(int terrainValue)
        {
            this.terrainValue = terrainValue;
        }

        public int GetTerrainValue()
        {
            return terrainValue;
        }
        public void SetTemperature(int temperature)
        {
            this.temperature = temperature;
        }
        
        public int GetTemperature()
        {
            return temperature;
        }
        public void SetPrecipitation(int precipitation)
        {
            this.precipitation = precipitation;
        }
        
        public int GetPrecipitation()
        {
            return precipitation;
        }

    }

    void Start() 
    {
        grassColor = CalculateGrassColor();
        grassMaterial.SetColor("_BaseColor", grassColor);
        leafColor = CalculateLeafColor();
        leafMaterial.SetColor("_BaseColor", leafColor);

        offsetX = UnityEngine.Random.Range(0f, 9999f);
        offsetZ = UnityEngine.Random.Range(0f, 9999f);
        noiseScale = UnityEngine.Random.Range(.33f, .75f);

        secondaryOffset = UnityEngine.Random.Range(0f, 9999f);
        tertiaryOffset = UnityEngine.Random.Range(0f, 9999f);

        grid = new Grid<TerrainObject>(100, 100, 10f, Vector3.zero, (Grid<TerrainObject> g, int x, int z) => new TerrainObject(g, x, z));
        
        for (int x=0; x < grid.gridArray.GetLength(0); x++) 
        {
            for (int z=0; z < grid.gridArray.GetLength(1); z++) 
            {
                xPosition = (float)x / grid.width;
                zPosition = (float)z / grid.height;
                CalculatePrecipitation(x, z);
                CalculateTerrain(x, z);
                CalculateTemperature(x, z);
                GenerateTerrain(x, z);
            }
        }
        // SetGridObjectToTerrain();

        Renderer debugRenderer = debugPlane.GetComponent<Renderer>();
        debugRenderer.material.mainTexture = DebugTexture();

        bool debugMode = false;
        debugPlane.SetActive(false);
        if(debugMode)
        {
            debugPlane.SetActive(true);
        } 
    }

    private Texture2D DebugTexture()
    {
        Texture2D texture = new Texture2D(100, 100);
        for (int x=0; x < grid.gridArray.GetLength(0); x++) 
        {
            for (int z=0; z < grid.gridArray.GetLength(1); z++) 
            {
                //Land Type
                /*
                if(grid.GetGridObject(x, z).GetTerrainValue() == 1) {
                    Color color = new Color(0, 1, 0);
                }
                else {
                    Color color = new Color(0, 0, 0);
                }
                */

                //Precipitation
                //Color color = new Color(0, 0, ((float)grid.GetGridObject(x, z).GetPrecipitation() / 12.0f));

                //Temperature
                Color color = new Color(((float)grid.GetGridObject(x, z).GetTemperature() / 12.0f), 0.0f, 0.0f);

                texture.SetPixel(x, z, color);
            }
        }
        texture.Apply();
        return texture; 
    }

    private Color CalculateGrassColor()
    {
        Color color =  Color.HSVToRGB(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.7f, 0.85f), UnityEngine.Random.Range(0.7f, 0.85f));
        return color;
    }

    private Color CalculateLeafColor()
    {
        return new Color(grassColor.r * 0.5f, grassColor.g * 0.5f, grassColor.b * 0.5f, 1);
    }

    public void Update()
    {

    }

    /*private void SetGridObjectToTerrain()
    {
        for (int x=0; x < grid.gridArray.GetLength(0); x++) 
        {
            for (int z=0; z < grid.gridArray.GetLength(1); z++) 
            {
                grid.SetGridObject(x, z, PerlinValue(x, z));
            }
        }        
    }*/


    public void CalculateTemperature(int x, int z)
    {
        float xCoord = xPosition * .50f * Mathf.PI + UnityEngine.Random.Range(0f, 9999f);
        float zCoord = zPosition * .50f * Mathf.PI + UnityEngine.Random.Range(0f, 9999f);

        int temp = Mathf.Clamp(
            (int)(((zPosition * 0.9f) + Random.Range(-0.1f, 0.1f))  
            * 12.5f), 0, 12);
        grid.GetGridObject(x, z).SetTemperature(temp);

        // Debug.Log("(" + x + ", " + z + ") has a coordinate of (" + xCoord + ", " + zCoord + ") and a temperature of " + grid.GetGridObject(x, z).GetTemperature());
    }

    public void CalculatePrecipitation(int x, int z)
    {
        float xCoord = xPosition * .10f * Mathf.PI + UnityEngine.Random.Range(0f, 9999f);
        float zCoord = zPosition * .10f * Mathf.PI + UnityEngine.Random.Range(0f, 9999f);
        grid.GetGridObject(x, z).SetPrecipitation(Mathf.FloorToInt((Mathf.PerlinNoise((float)xCoord, (float)zCoord)) * 12.5f));
        // Debug.Log("(" + x + ", " + z + ") has a coordinate of (" + xCoord + ", " + zCoord + ") and a percipitation of " + grid.GetGridObject(x, z).GetPrecipitation());
    }

    public void CalculateTerrain(int x, int z)
    {
        {   
            /*
            float secondaryX = xPosition * 1f + secondaryOffset;
            float secondaryZ = zPosition * 1f + secondaryOffset;
            float secondaryPerlin = Mathf.PerlinNoise(secondaryX, secondaryZ) * UnityEngine.Random.Range(-.015f, .015f);

            float tertiaryX = xPosition * 5f + tertiaryOffset;
            float tertiaryZ = zPosition * 5f + tertiaryOffset;
            float tertiaryPerlin = Mathf.PerlinNoise(tertiaryX, tertiaryZ) * UnityEngine.Random.Range(-.007f, .007f);
            */

            float xCoord = xPosition * noiseScale * Mathf.PI + offsetX;
            float zCoord = zPosition * noiseScale * Mathf.PI + offsetZ;

            // Debug.Log("(" + x + ", " + z + ") has a coordinate of (" + xCoord + ", " + zCoord + ") and a PerlinValue of " + Mathf.PerlinNoise(xCoord, zCoord));
            // grid.gridArray[x, z] = ((Mathf.PerlinNoise((float)xCoord, (float)zCoord)) > 0.5 ? 1 : 0);
            grid.GetGridObject(x, z).SetTerrainValue(((Mathf.PerlinNoise((float)xCoord, (float)zCoord)) > 0.5 ? 1 : 0)); 
        }
    }

    // For every tile that has a value of one, a new ground object is created as a child of terrain.
    public void GenerateTerrain(int x, int z)
    {
        if (grid.GetGridObject(x, z).GetTerrainValue() == 1)
        {
            Instantiate(land, grid.GetWorldPosition(x, z) + new Vector3(grid.cellSize * .5f, grid.cellSize * .25f, grid.cellSize * .5f), Quaternion.identity, terrain);
            GenerateTrees(x, z);
        }   
    }

    [System.Serializable]
    public class ForestElement
    {
        [Range(1, 12)]
        public int density;

        public void SetDensity(int density)
        {
            this.density = density;
        }

        public bool CanPlace()
        {
            if(Random.Range(3, 24) < density)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public GameObject[] treePrefabs;
        public GameObject GetRandomPrefab()
        {
            return treePrefabs[UnityEngine.Random.Range(0, treePrefabs.Length)];
        }
    }

    public void GenerateTrees(int x, int z)
    {
        for(float localX = 0; localX < forestSize; localX += elementSpacing)
        {
            for(float localZ = 0; localZ < forestSize; localZ += elementSpacing)
            {
                ForestElement forestElement = forestElements[0];
                forestElement.SetDensity(grid.GetGridObject(x, z).GetPrecipitation());
                if(forestElement.CanPlace())
                {
                    Vector3 offset = new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), 0, UnityEngine.Random.Range(-1.5f, 1.5f));
                    Vector3 position = new Vector3(localX + .5f, 4, localZ + .5f) + grid.GetWorldPosition(x, z) + offset;
                    Vector3 scale = Vector3.one * UnityEngine.Random.Range(85.0f, 115.0f);
                    grid.GetXZ(position, out int newX, out int newZ);
                    if (newX >= 0 && newZ >= 0 && newX < grid.width && newZ < grid.height)
                    {
                        if(grid.GetGridObject(newX, newZ).GetTerrainValue() == 1)
                        {
                            GameObject newElement = Instantiate(forestElement.GetRandomPrefab(), position, Quaternion.Euler(-90, UnityEngine.Random.Range(0, 360), 0));
                            newElement.transform.localScale = scale;
                            grid.GetGridObject(newX, newZ).AddForestElement(newElement);
                        }
                    }
                }
            }
        }
    }
}
