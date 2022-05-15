using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTrees : MonoBehaviour
{
    public ForestElement[] forestElements;
    public HexMap map;

    [System.Serializable]
    public class ForestElement
    {
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
        Vector3 variation = new Vector3(Hexagon.blendFactor * Hexagon.innerRadius, 0, Hexagon.blendFactor * Hexagon.innerRadius);
        Vector3 startPosition = new Vector3(tile.position.x - (Hexagon.innerRadius), tile.position.y, tile.position.z - Hexagon.outerRadius);
        float elementSpacing = .75f;

        if(tile.Elevation == 0) return;
        
        for(float localX = 0; localX < Hexagon.innerRadius * 2f; localX += elementSpacing)
        {
            for(float localZ = 0; localZ < Hexagon.outerRadius * 2f; localZ += elementSpacing)
            {
                ForestElement forestElement = forestElements[0];
                if(tile.precipitation >= 7)
                { 
                    forestElement.SetDensity(tile.Elevation * 36);
                    if(forestElement.CanPlace())
                    {
                        Vector3 offset = new Vector3(Random.Range(-3.5f, 3.5f), 0, UnityEngine.Random.Range(-3.5f, 3.5f));
                        Vector3 position = tile.position + offset + new Vector3(localX, 0, localZ);
                        position.y += Hexagon.GetOffset(position) - .05f;
                        Vector3 scale = Vector3.one * Random.Range(45.0f, 65.0f);
                        if(map.IsOnMap(HexCoordinates.FromPosition(position)))
                        {
                            HexTile newTile = map.FromPosition(position);
                            if(newTile.position == tile.position)
                            {
                                Vector3 position2d = position;
                                position2d.y = newTile.position.y;

                                // Behold, the world's worst code.
                                if((newTile.numBorderElevations == 0) || (
                                    (map.IsOnMap((position2d - newTile.position) * 
                                    (1 + Hexagon.solidFactor) + newTile.position)) && map.FromPosition(
                                    (position2d - newTile.position) * 
                                    (1 + Hexagon.solidFactor) + newTile.position).Elevation 
                                    == newTile.Elevation))
                                {
                                    if(tile.precipitation > 8)
                                    {
                                        GameObject newElement = Instantiate(forestElement.GetRandomPrefab(), position, Quaternion.Euler(-90, UnityEngine.Random.Range(0, 360), 0));
                                        newElement.transform.localScale = scale;
                                        newTile.forestElements.Add(newElement);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
