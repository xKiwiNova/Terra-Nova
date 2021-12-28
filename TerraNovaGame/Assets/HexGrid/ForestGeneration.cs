using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGeneration : MonoBehaviour
{
    public ForestElement[] forestElements;
    public HexGrid grid;

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
            return (Random.Range(0, 48) < density);
        }

        public GameObject[] treePrefabs;
        public GameObject GetRandomPrefab()
        {
            return treePrefabs[UnityEngine.Random.Range(0, treePrefabs.Length)];
        }
    }

    public void GenerateForest(Cell[] cells)
    {
        foreach (Cell cell in cells)
        {
            GenerateTrees(cell);
        }
    }

    public void GenerateTrees(Cell cell)
    {
        Vector3 startPosition = new Vector3(cell.Position.x - (HexMetrics.innerRadius), cell.Position.y, cell.Position.z - HexMetrics.outerRadius);
        float elementSpacing = 5;

        if(cell.elevation < 6) return;
        
        for(float localX = 0; localX < HexMetrics.innerRadius * 2f; localX += elementSpacing)
        {
            for(float localZ = 0; localZ < HexMetrics.outerRadius * 2f; localZ += elementSpacing)
            {
                ForestElement forestElement = forestElements[0];
                forestElement.SetDensity(cell.precipitation);
                if(forestElement.CanPlace())
                {
                    Vector3 offset = new Vector3(Random.Range(-1.5f, 1.5f), 0, UnityEngine.Random.Range(-1.5f, 1.5f));
                    Vector3 position = cell.Position + offset + new Vector3(localX, 0, localZ);
                    Vector3 scale = Vector3.one * Random.Range(85.0f, 115.0f);
                    try
                    {
                        Cell newCell = grid.GetCell(position);
                        if(newCell.elevation == cell.elevation)
                        {
                            GameObject newElement = Instantiate(forestElement.GetRandomPrefab(), position, Quaternion.Euler(-90, UnityEngine.Random.Range(0, 360), 0));
                            newElement.transform.localScale = scale;
                            newCell.forestElements.Add(newElement);
                        }
                    }
                    catch
                    {
                        
                    }
                }
            }
        }
    }
}
