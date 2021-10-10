using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HexGrid : MonoBehaviour
{
    public bool debugGridCoords = true;
    public TextMeshProUGUI cellLabelPrefab;
    Canvas gridCanvas;

    public int width = 10;
	public int height = 6;

	public HexCell cellPrefab;
    
    HexCell[] cells;
    HexMesh hexMesh;

    public Color defaultColor = Color.white;
	public Color touchedColor = Color.magenta;


    void Awake() 
    {
		cells = new HexCell[height * width];
        gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

        for (int z = 0, i = 0; z < height; z++) 
            {
            for (int x = 0; x < width; x++) 
            {
		    
				CreateCell(x, z, i++);
			}
		}
	}
    
    // Creates the cell given an x and z coordinate, i is the cell number
    void CreateCell (int x, int z, int i) 
    {
        // Creates the Position of each Hexagon by inputing coordinates.
        // Hexagon grids are offset on every row, so add z * .5f to the input to get position.
        // To maintain the "cubiness" of the grid, the hexagons are shifted back every other row.

		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

        // Actually instantiations the cell and sets the parent to the object this is attached to.
		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.color = defaultColor;
        
        // A debug tool to see the coordinates of each cell. Turn debugGridCoords to see it.
        TextMeshProUGUI label = Instantiate<TextMeshProUGUI>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x - 20, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();

        if(debugGridCoords){ label.enabled = true; }
        else{label.enabled = false;}
	}

    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    void Update() 
    {
		if (Input.GetMouseButton(0)) 
        {
			HandleInput();
		}
	}

    // Deals with raycast being sent out
	void HandleInput() 
    {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) 
        {
			TouchCell(hit.point);
		}
	}
	
	void TouchCell(Vector3 position) 
    {
		position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		Debug.Log("touched at " + coordinates.ToString());

        int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		HexCell cell = cells[index];
        Debug.Log(cell.coordinates.ToString());
		cell.color = touchedColor;
		hexMesh.Triangulate(cells);
	}
}
