using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MapGeneration : MonoBehaviour
{
    Mesh hexMesh;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Color> colors = new List<Color>();
    MeshCollider meshCollider;

    void Awake()
    {
        meshCollider = gameObject.AddComponent<MeshCollider>();
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "HexMesh";
    }

    // Triangulates all the Chunks
    public void Triangulate(HexChunk[,] chunks)
    {
        // Clears the current mesh before generating a new one
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        foreach(HexChunk chunk in chunks)
        {
            Triangulate(chunk);
        }

        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();

        hexMesh.RecalculateNormals();

        meshCollider.sharedMesh = hexMesh;
    }

    // Triangulates all the tiles in the chunk
    public void Triangulate(HexChunk chunk)
    {
        Triangulate(chunk.tiles);
    }

    public void Triangulate(HexTile[,] tiles)
    {
        foreach(HexTile tile in tiles)
        {
            Triangulate(tile);
        }
    }

    public void Triangulate(HexTile tile)
    {
        // Generate each triangle
        for(HexDirection direction = HexDirection.NW; direction <= HexDirection.SW; direction++)
        {
            Triangulate(tile, direction);
        }
    }

    public void Triangulate(HexTile tile, HexDirection direction)
    {
        Vector3 center = tile.position;
        Color color = tile.GetColor(direction);
        HexTile neighbor = tile.GetNeighbor(direction);

        bool isFirstProtrusion =  neighbor != null &&
                tile.GetNeighbor(direction.Next()) != null &&
                neighbor.GetNeighbor(direction.Next().Next()) != null &&
                tile.Elevation > tile.GetNeighbor(direction.Next()).Elevation &&
                neighbor.Elevation > neighbor.GetNeighbor(direction.Next().Next()).Elevation;

        bool isSecondProtrusion = neighbor != null &&
                tile.GetNeighbor(direction.Previous()) != null &&
                neighbor.GetNeighbor(direction.Previous().Previous()) != null &&
                tile.Elevation > tile.GetNeighbor(direction.Previous()).Elevation &&
                neighbor.Elevation > neighbor.GetNeighbor(direction.Previous().Previous()).Elevation;


        // If the neighboring tile has a different elevation
        if(neighbor != null && tile.Elevation > neighbor.Elevation)
        {
            // Creating the solid part of the triangle
            Vector3 vertex1 = tile.GetSolidCorner(direction);
            Vector3 vertex2 = tile.GetSecondSolidCorner(direction);
            center.y = vertex1.y = vertex2.y = tile.position.y;

            AddTriangle(center, vertex1, vertex2);
            AddTriangleColor(color, color, color);

            // If the tile's elevation is higher than its neighbors, create a slant
            // Creating the part of the triangle that merges/blends
            Vector3 vertex3 = tile.GetCorner(direction);
            Vector3 vertex4 = tile.GetSecondCorner(direction);

            vertex3.y = vertex4.y = neighbor.position.y;

            AddQuad(vertex1, vertex2, vertex3, vertex4);
            AddQuadColor(color, color, color, color);

            HexTile previousNeighbor = tile.GetNeighbor(direction.Previous());
            if(previousNeighbor != null && previousNeighbor.Elevation == tile.Elevation)
            {
                Color color2 = tile.GetColor(direction.Previous());
                Vector3 vertex5 = tile.GetCorner(direction.Previous()) + Hexagon.GetSolidCorner(direction.Next());
                AddTriangle(vertex5, vertex3, vertex1);
                AddTriangleColor(color2, color2, color2);
            }

            HexTile nextNeighbor = tile.GetNeighbor(direction.Next());
            if(nextNeighbor != null && nextNeighbor.Elevation == tile.Elevation)
            {
                Color color2 = tile.GetColor(direction.Next());
                Vector3 vertex5 = tile.GetCorner(direction.Next().Next()) + Hexagon.GetSolidCorner(direction);
                AddTriangle(vertex4, vertex5, vertex2);
                AddTriangleColor(color2, color2, color2);
            }
        }
        else 
        {
            if(isFirstProtrusion && !isSecondProtrusion)
            {
                Vector3 vertex1 = tile.GetCorner(direction);
                Vector3 vertex2 = tile.GetCorner(direction) + Hexagon.GetSolidCorner(direction.Next().Next());
                Vector3 vertex3 = tile.GetSecondSolidCorner(direction);

                AddTriangle(center, vertex1, vertex2);
                AddTriangleColor(color, color, color);

                AddTriangle(center, vertex2, vertex3);
                AddTriangleColor(color, color, color);       
            }
            else if(isSecondProtrusion && !isFirstProtrusion)
            {
                Vector3 vertex1 = tile.GetSolidCorner(direction);
                Vector3 vertex2 = tile.GetSecondCorner(direction) + Hexagon.GetSolidCorner(direction.Previous());
                Vector3 vertex3 = tile.GetSecondCorner(direction);

                AddTriangle(center, vertex1, vertex2);
                AddTriangleColor(color, color, color);

                AddTriangle(center, vertex2, vertex3);
                AddTriangleColor(color, color, color);       
            }
            else if(isFirstProtrusion && isSecondProtrusion)
            {
                Vector3 vertex1 = tile.GetSolidCorner(direction);
                Vector3 vertex2 = tile.GetSecondCorner(direction) + Hexagon.GetSolidCorner(direction.Previous());
                Vector3 vertex3 = tile.GetCorner(direction) + Hexagon.GetSolidCorner(direction.Next().Next());
                Vector3 vertex4 = tile.GetSecondSolidCorner(direction);

                AddTriangle(center, vertex1, vertex2);
                AddTriangleColor(color, color, color);

                AddTriangle(center, vertex2, vertex3);
                AddTriangleColor(color, color, color);

                AddTriangle(center, vertex3, vertex4);
                AddTriangleColor(color, color, color);
            }
            else
            {
                Vector3 vertex1 = tile.GetCorner(direction);
                Vector3 vertex2 = tile.GetSecondCorner(direction);
                center.y = vertex1.y = vertex2.y = tile.position.y;

                AddTriangle(center, vertex1, vertex2);
                AddTriangleColor(color, color, color);
            }
        }
    }

    // Adds a triangle giver three vertices
    void AddTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        int currentVertices = vertices.Count;

        vertices.Add(vertex1);
        vertices.Add(vertex2);
        vertices.Add(vertex3);

        triangles.Add(currentVertices);
        triangles.Add(currentVertices + 1);
        triangles.Add(currentVertices + 2);
    }

    void AddTriangleColor(Color color1, Color color2, Color color3)
    {
        colors.Add(color1);
        colors.Add(color2);
        colors.Add(color3);
    }

    // Adds a Quad (Secretly 2 triangles)
    void AddQuad(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 vertex4)
    {
        int currentVertices = vertices.Count;

		vertices.Add(vertex1);
		vertices.Add(vertex2);
		vertices.Add(vertex3);
		vertices.Add(vertex4);

		triangles.Add(currentVertices);
		triangles.Add(currentVertices + 2);
		triangles.Add(currentVertices + 1);
		triangles.Add(currentVertices + 1); // We reuse indexes/vertices so we only need 4 points instead of 6.
		triangles.Add(currentVertices + 2);
		triangles.Add(currentVertices + 3);
    }

    void AddQuadColor(Color color1, Color color2, Color color3, Color color4)
    {
        colors.Add(color1);
        colors.Add(color2);
        colors.Add(color3);
        colors.Add(color4);
    }
}
