using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridGenerator : MonoBehaviour
{
    public GameObject land;
    public Transform terrain;
    private Grid grid;
    private void Start() 
    {
        grid = new Grid(100, 100, 5f);
        GenerateTerrain();
    }
    // For every tile that has a value of one, a new ground object is created as a child of terrain.
    public void GenerateTerrain()
    {
        for (int x=0; x < grid.gridArray.GetLength(0); x++) 
        {
            for (int z=0; z < grid.gridArray.GetLength(1); z++) 
            {
                if (grid.PerlinValue(x, z) == 1)
                {
                    Instantiate(land, grid.GetWorldPosition(x, z) + new Vector3(grid.cellSize * .5f, grid.cellSize * .25f, grid.cellSize * .5f), Quaternion.identity, terrain);
                }
            }
        }
    }
}
