using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexForest : MonoBehaviour
{
    public ForestElement[] forestElements;
    public HexMap map;

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

    public void GenerateForest(HexTile[,] tiles)
    {
        foreach (HexTile tile in tiles)
        {
            GenerateTrees(tile);
        }
    }

    public void GenerateTrees(HexTile tile)
    {
        Vector3 startPosition = new Vector3(tile.position.x - (Hexagon.innerRadius), tile.position.y, tile.position.z - Hexagon.outerRadius);
        float elementSpacing = 5;

        if(tile.Elevation == 0) return;
        
        for(float localX = 0; localX < Hexagon.innerRadius * 2f; localX += elementSpacing)
        {
            for(float localZ = 0; localZ < Hexagon.outerRadius * 2f; localZ += elementSpacing)
            {
                ForestElement forestElement = forestElements[0];
                forestElement.SetDensity(tile.Elevation * 36);
                if(forestElement.CanPlace())
                {
                    Vector3 offset = new Vector3(Random.Range(-1.5f, 1.5f), 0, UnityEngine.Random.Range(-1.5f, 1.5f));
                    Vector3 position = tile.position + offset + new Vector3(localX, 0, localZ);
                    position.y += Hexagon.GetOffset(position) - .05f;
                    Vector3 scale = Vector3.one * Random.Range(45.0f, 65.0f);
                    try
                    {
                        HexTile newtile = map.FromPosition(position);
                        if(newtile.position == tile.position)
                        {
                            GameObject newElement = Instantiate(forestElement.GetRandomPrefab(), position, Quaternion.Euler(-90, UnityEngine.Random.Range(0, 360), 0));
                            newElement.transform.localScale = scale;
                            newtile.forestElements.Add(newElement);
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
