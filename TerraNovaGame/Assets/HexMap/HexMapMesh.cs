using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Generates the mesh for the map
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMapMesh : MonoBehaviour
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

    // Triangulates all the tiles in the chunk
    public void Triangulate(HexChunk chunk)
    {
        // Clears any old data before generating new data
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        // Generates the data for the mesh
        Triangulate(chunk.tiles);

        // Applies that data
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();

        hexMesh.RecalculateNormals();

        meshCollider.sharedMesh = hexMesh;
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
        Color32 rockColor = tile.map.rockColor;
        Vector3 center = tile.position;
        Color color = tile.GetColor(direction);
        HexTile neighbor = tile.GetNeighbor(direction);

        // If the triangle next to this one is inclined but this one is not
        bool isFirstProtrusion =  neighbor != null &&
                tile.GetNeighbor(direction.Next()) != null &&
                neighbor.GetNeighbor(direction.Next().Next()) != null &&
                tile.Elevation > tile.GetNeighbor(direction.Next()).Elevation &&
                neighbor.Elevation > neighbor.GetNeighbor(direction.Next().Next()).Elevation;

        // If the triangle previous to this one is inclined but this one is not
        bool isSecondProtrusion = neighbor != null &&
                tile.GetNeighbor(direction.Previous()) != null &&
                neighbor.GetNeighbor(direction.Previous().Previous()) != null &&
                tile.Elevation > tile.GetNeighbor(direction.Previous()).Elevation &&
                neighbor.Elevation > neighbor.GetNeighbor(direction.Previous().Previous()).Elevation;


        // If the neighboring tile has a different elevation
        if(neighbor != null && tile.Elevation > neighbor.Elevation)
        {
            // Creating the solid part of the triangle
            Vector3 vertex1 = tile.GetCorner(direction);
            Vector3 vertex2 = tile.GetSecondCorner(direction);
            center.y = vertex1.y = vertex2.y = tile.position.y;

            AddTriangle(center, vertex1, vertex2);
            AddTriangleColor(color, color, color);

            // If the tile's elevation is higher than its neighbors, create a slant
            Vector3 vertex3 = tile.GetExtendedCorner(direction);
            Vector3 vertex4 = tile.GetSecondExtendedCorner(direction);

            vertex3.y = vertex4.y = neighbor.position.y;

            AddQuad(vertex1, vertex2, vertex3, vertex4);
            AddQuadColor(rockColor, rockColor, rockColor, rockColor);

            // Creates the final inverted corners at the intersections between three Hexagons where two are higher than the other
            HexTile previousNeighbor = tile.GetNeighbor(direction.Previous());
            if(previousNeighbor != null && previousNeighbor.Elevation == tile.Elevation)
            {
                Color color2 = tile.GetColor(direction.Previous());
                Vector3 vertex5 = tile.GetCorner(direction.Previous()) + Hexagon.GetCorner(direction.Next());
                AddTriangle(vertex5, vertex3, vertex1);
                AddTriangleColor(rockColor, rockColor, rockColor);
            }

            HexTile nextNeighbor = tile.GetNeighbor(direction.Next());
            if(nextNeighbor != null && nextNeighbor.Elevation == tile.Elevation)
            {
                Color color2 = tile.GetColor(direction.Next());
                Vector3 vertex5 = tile.GetCorner(direction.Next().Next()) + Hexagon.GetCorner(direction);
                AddTriangle(vertex4, vertex5, vertex2);
                AddTriangleColor(rockColor, rockColor, rockColor);
            }
        }
        else 
        {
            // If this is only a First Protruision
            if(isFirstProtrusion && !isSecondProtrusion)
            {
                Vector3 vertex1 = tile.GetCorner(direction);
                Vector3 vertex2 = tile.GetCorner(direction) + Hexagon.GetCorner(direction.Next().Next());
                Vector3 vertex3 = tile.GetSecondCorner(direction);

                AddTriangle(center, vertex1, vertex2);
                AddTriangleColor(color, color, color);

                AddTriangle(center, vertex2, vertex3);
                AddTriangleColor(color, color, color);       
            }
            // If this is only a second protrusion
            else if(isSecondProtrusion && !isFirstProtrusion)
            {
                Vector3 vertex1 = tile.GetSolidCorner(direction);
                Vector3 vertex2 = tile.GetSecondCorner(direction) + Hexagon.GetCorner(direction.Previous());
                Vector3 vertex3 = tile.GetSecondCorner(direction);

                //AddTriangle(center, vertex1, vertex2);
                //AddTriangleColor(Color.blue, Color.blue, Color.blue);

                AddTriangle(center, vertex2, vertex3);
                AddTriangleColor(color, color, color);       
            }
            // If both
            else if(isFirstProtrusion && isSecondProtrusion)
            {
                Vector3 vertex1 = tile.GetCorner(direction);
                Vector3 vertex2 = tile.GetSecondCorner(direction) + Hexagon.GetCorner(direction.Previous());
                Vector3 vertex3 = tile.GetCorner(direction) + Hexagon.GetCorner(direction.Next().Next());
                Vector3 vertex4 = tile.GetSecondCorner(direction);

                AddTriangle(center, vertex1, vertex2);
                AddTriangleColor(color, color, color);

                AddTriangle(center, vertex2, vertex3);
                AddTriangleColor(color, color, color);

                AddTriangle(center, vertex3, vertex4);
                AddTriangleColor(color, color, color);
            }
            // If neither
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

    Vector3 GetOffset(Vector3 vertex)
    {
        Vector3 offset = new Vector3(0, Hexagon.GetOffset(vertex), 0);
        return vertex + offset;
    }

    // Adds a triangle given three vertices
    void AddTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        int currentVertices = vertices.Count;

        vertices.Add(GetOffset(vertex1));
        vertices.Add(GetOffset(vertex2));
        vertices.Add(GetOffset(vertex3));

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

		vertices.Add(GetOffset(vertex1));
		vertices.Add(GetOffset(vertex2));
		vertices.Add(GetOffset(vertex3));
		vertices.Add(GetOffset(vertex4));

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
