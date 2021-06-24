using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class Grid 
{
    public int width;
    public int height;
    public float cellSize;
    private TextMesh[,] debugTextArray;
    public GameObject land;
    public float offsetX = UnityEngine.Random.Range(0f, 9999f);
    public float offsetZ = UnityEngine.Random.Range(0f, 9999f);
    public float noiseScale = UnityEngine.Random.Range(.33f, .75f);
 
    public int[,] gridArray;
    
    void Start()
    {
        
    }
    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];

        for (int x=0; x < gridArray.GetLength(0); x++) {
            for (int z=0; z < gridArray.GetLength(1); z++) {
                gridArray[x, z] = PerlinValue(x, z);
                //debugTextArray[x, z] = UtilsClass.CreateWorldText(gridArray[x, z].ToString(), null, GetWorldPosition(x, z) + new Vector3(cellSize * .5f, cellSize * .25f, cellSize * .5f), 20, Color.white, TextAnchor.MiddleCenter);
                //debugTextArray[x, z].text = gridArray[x, z].ToString();
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);

    }

    public Vector3 GetWorldPosition(int x, int z) {
        return new Vector3(x, 0, z) * cellSize;
    }

    public int PerlinValue(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            float xCoord = (float)x / width * noiseScale * Mathf.PI + offsetX;
            float zCoord = (float)z / height * noiseScale * Mathf.PI + offsetZ;
            return (Mathf.PerlinNoise(xCoord, zCoord) < 0.5 ? 1 : 0);
        }
        else
        {
            return (-1);
        }
    }
}
     