using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HexForest : MonoBehaviour
{
    public List<ForestType> forestTypes = new List<ForestType>();
    public HexMap map;

    public GameObject testCube;

    [System.Serializable]
    public class ForestType
    {
        public List<GameObject> trees = new List<GameObject>();
        public int density;

        public int tempMin;
        public int tempMax;
        public int precMin;
        public int precMax;
    }

    public void MakeForest()
    {
        for(float x = -Hexagon.outerRadius; x < map.tileCountX * (Hexagon.outerRadius * 3f/2f) + Hexagon.outerRadius; x += 3f)
        {
            for(float z = -2f * Hexagon.innerRadius; z < map.tileCountZ * (Hexagon.innerRadius * 2); z += 3f)
            {
                Vector3 position = new Vector3(x + Random.Range(-1f, 1f), 10, z + Random.Range(-1f, 1f));

                if(!map.IsOnMap(position))
                {
                    continue;
                }
                else if(map.FromPosition(position).Elevation == 0)
                {
                    continue;
                }
                

                HexTile tile = map.FromPosition(position);

                // Behold, the worst if statement ever
                if (
                    (!map.IsOnMap((position - tile.position) * 
                    (1 + Hexagon.solidFactor) + tile.position)) 
                    ||
                    (map.FromPosition((position - tile.position) * 
                    (1 + Hexagon.solidFactor) + tile.position).Elevation 
                    != tile.Elevation) && tile.numBorderElevations > 0)
                {
                    continue;
                }

                if(tile.precipitation >= 7 || Random.Range(0f, 12f) < tile.precipitation / 6f)
                {
                    position.y = tile.Elevation * Hexagon.elevationStep + Hexagon.GetOffset(position);
                    Instantiate<GameObject>(testCube, position, Quaternion.Euler(-90, 0, 0));
                }
            }
        }
    }
}