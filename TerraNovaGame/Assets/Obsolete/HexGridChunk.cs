using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HexGridChunk : MonoBehaviour
{
    Cell[] cells;

    HexMesh hexMesh;
    Canvas gridCanvas;

    void Awake() 
    {
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new Cell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
	}

    public void AddCell(int index, Cell cell)
    {
        cells[index] = cell;
		cell.transform.SetParent(transform, false);
        cell.chunk = this;
		cell.debugUIRect.SetParent(gridCanvas.transform, false);
    }

    public void Refresh()
    {
        enabled = true;
    }

    void LateUpdate()
    {
        hexMesh.Triangulate(cells);
        enabled = false;
    }
}
