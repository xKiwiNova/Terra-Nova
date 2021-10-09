using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

    // This script generates the hexagonal meshes which make the grid.

	Mesh hexMesh;
	List<Vector3> vertices;
	List<int> triangles;
    MeshCollider meshCollider;

    List<Color> colors;

	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();

		hexMesh.name = "Hex Mesh";
		vertices = new List<Vector3>();
        colors = new List<Color>();
		triangles = new List<int>();
	}

    public void Triangulate (HexCell[] cells) {

        // Clears, just in case there is already data for the mesh.
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
        colors.Clear();

        // Goes through each individual cell and calls a new method that generates the triangles for each individual cell.
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}

        // Adds to the mesh all the generated vertices and triangles
		hexMesh.vertices = vertices.ToArray();
        hexMesh.colors = colors.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.RecalculateNormals();

        meshCollider.sharedMesh = hexMesh;
	}

    void Triangulate (HexCell cell) {
        Vector3 center = cell.transform.localPosition;
        for (int i = 0; i < 6; i++) {
            AddTriangle(
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[i + 1]
            );
            AddTriangleColor(cell.color);
        }
	}

    void AddTriangleColor (Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

    // Adds a triangle, given three vertices
    void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}
}
