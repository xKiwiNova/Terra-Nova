using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for storing chunks of tiles
public class HexChunk : MonoBehaviour
{
    public HexTile[,] tiles;
    public int x;
    public int z;
    public HexMapMesh mapMesh; // The chunk mesh

    public void InstantiateHexChunk(int x, int z, HexMap map)
    {
        tiles = new HexTile[Hexagon.chunkSizeX, Hexagon.chunkSizeZ];
        this.x = x;
        this.z = z;
        mapMesh = GetComponent<HexMapMesh>();
        GenerateTiles(map);
    }

    // Generates each tile in the chunk
    public void GenerateTiles(HexMap map)
    {
        for(int x = 0; x < Hexagon.chunkSizeX; x++)
        {
            for(int z = 0; z < Hexagon.chunkSizeZ; z++)
            {
                HexTile tile = tiles[x, z] = Instantiate(map.hexTilePrefab);
                tile.InstantiateHexTile(x + (this.x * Hexagon.chunkSizeX), z + (this.z * Hexagon.chunkSizeZ), map, this);
                tile.name = tile.ToString();
                tile.transform.SetParent(this.transform);
                tile.Color = map.grassColor;
            }
        }
    }

    // Generates the chunk mesh
    public void Triangulate()
    {
        mapMesh.Triangulate(this);
    }

    public override string ToString()
    {
        return $"Chunk{(x, z)}";
    }
}