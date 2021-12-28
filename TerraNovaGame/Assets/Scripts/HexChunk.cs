using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexChunk
{
    public HexTile[,] tiles;
    public int x;
    public int z;

    public HexChunk(int x, int z, HexMap map)
    {
        tiles = new HexTile[Hexagon.chunkSizeX, Hexagon.chunkSizeZ];
        this.x = x;
        this.z = z;
        GenerateTiles(map);
    }

    public void GenerateTiles(HexMap map)
    {
        for(int x = 0; x < Hexagon.chunkSizeX; x++)
        {
            for(int z = 0; z < Hexagon.chunkSizeZ; z++)
            {
                tiles[x, z] = new HexTile(x + (this.x * Hexagon.chunkSizeX), z + (this.z * Hexagon.chunkSizeZ), map, this);
            }
        }
    }

    public override string ToString()
    {
        return $"Chunk{(x, z)}";
    }
}