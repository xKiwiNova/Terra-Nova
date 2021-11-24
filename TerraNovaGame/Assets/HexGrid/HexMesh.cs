using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;
    MeshCollider meshCollider;
 
    void Awake()
    {
        meshCollider = gameObject.AddComponent<MeshCollider>();
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "HexMesh";

        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }

    public void Triangulate(HexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        for(int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }

        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();

        hexMesh.RecalculateNormals();

        meshCollider.sharedMesh = hexMesh;
    }



	void Triangulate (HexCell cell) 
    {
		for (HexDirection direction = HexDirection.NE; direction <= HexDirection.NW; direction++) {
			Triangulate(direction, cell);
		}
	}

	void Triangulate (HexDirection direction, HexCell cell) 
    {
		Vector3 center = cell.Position;
        Vector3 vertex1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 vertex2 = center + HexMetrics.GetSecondSolidCorner(direction);

        // Adds the "Solid" (Not Blended) part  of the triangle.
		AddTriangle(center, vertex1, vertex2);
        AddTriangleColor(cell.color, cell.color, cell.color);

        // Creates a quad with the edges blending. The outermost vertices are the average of the cell and neighbor.

        if(direction <= HexDirection.SE)
        {
            TriangulateConnection(direction, cell, vertex1, vertex2);
        }
        
	}

    // Generates the edges of each cell, where color blending takes place
    void TriangulateConnection(HexDirection direction, HexCell cell, Vector3 vertex1, Vector3 vertex2)
    {
        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 vertex3 = vertex1 + bridge;
        Vector3 vertex4 = vertex2 + bridge;

        HexCell neighbor = cell.GetNeighbor(direction);
        
        if(neighbor == null)
        {
            return;
        }

        // vertex3.y = vertex4.y = neighbor.elevation * HexMetrics.elevationStep; 
        vertex3.y = vertex4.y = neighbor.Position.y; 

        HexCell prevNeighbor = cell.GetNeighbor(direction.Previous()) ?? cell;
        HexCell nextNeighbor = cell.GetNeighbor(direction.Next());

        if(direction <= HexDirection.E && nextNeighbor != null)
        {
            Vector3 vertex5 = vertex2 + HexMetrics.GetBridge(direction.Next());
            // vertex5.y = nextNeighbor.elevation * HexMetrics.elevationStep;
            vertex5.y = nextNeighbor.Position.y;

            AddTriangle(vertex2, vertex4, vertex5);
            AddTriangleColor(cell.color, neighbor.color, nextNeighbor.color);
        }

        AddQuad(vertex1, vertex2, vertex3, vertex4);
        AddQuadColor(cell.color, neighbor.color);

        // The final edge triangles on either side of the quad to make up the rest of the trapezoid and finish the triangle.
        /*
        AddTriangle(vertex1, center + HexMetrics.GetFirstCorner(direction), vertex3);
        AddTriangleColor(cell.color, (cell.color + prevNeighbor.color + neighbor.color) / 3f, bridgeColor);

        AddTriangle(vertex2, vertex4, center + HexMetrics.GetSecondCorner(direction));
		AddTriangleColor(cell.color, bridgeColor, (cell.color + neighbor.color + nextNeighbor.color) / 3f);*/
    }

    // Given 3 vertex positions, adds the vertices in order to form a triangle.
    void AddTriangle(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(Perturb(vertex1));
        vertices.Add(Perturb(vertex2));
        vertices.Add(Perturb(vertex3));

        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    void AddTriangleColor(Color color1, Color color2, Color color3)
    {
        colors.Add(color1);
        colors.Add(color2);
        colors.Add(color3);
    }

    // This method is used the add the trapezoids which make up the outer part of the triangle or "blend regions"
    // It is really just making 2 triangles given for corners or vertices.
    void AddQuad(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 vertex4)
    {
        int vertexIndex = vertices.Count;
		vertices.Add(Perturb(vertex1));
		vertices.Add(Perturb(vertex2));
		vertices.Add(Perturb(vertex3));
		vertices.Add(Perturb(vertex4));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1); // We reuse indexes/vertices so we only need 4 points instead of 6.
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
    }

    void AddQuadColor (Color color1, Color color2, Color color3, Color color4) {
		colors.Add(color1);
		colors.Add(color2);
		colors.Add(color3);
		colors.Add(color4);
	}

    void AddQuadColor(Color color1, Color color2)
    {
        colors.Add(color1);
		colors.Add(color1);
		colors.Add(color2);
		colors.Add(color2);
    }

    Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = HexMetrics.SampleNoise(position);
        position.x += sample.x * 2f - 1f * HexMetrics.cellPerturbStrength;
        // position.y += sample.y * 2f - 1f * HexMetrics.cellPerturbStrength;;
        position.z += sample.z * 2f - 1f * HexMetrics.cellPerturbStrength;;
        return position;
    }
}
