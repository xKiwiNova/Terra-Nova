using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {

	public Color[] colors;

	public HexGrid hexGrid;
    // public Slider slider;

	private Color activeColor;
    private int activeElevation;

	void Awake() 
    {
		SelectColor(0);
	}

    void Update() 
    {
        if (Input.GetMouseButtonDown(0)) // Makes sure that it doesn't due anything if the mouse is over the UI.
        {
			HandleInput();

		}
        // SetElevation(slider.value);
    }

    void HandleInput() 
    {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) 
        {
			EditCell(hexGrid.GetCell(hit.point));
		}
	}

    void EditCell (HexCell cell) {
		cell.color = activeColor;
        cell.Elevation  = activeElevation;
		hexGrid.Refresh();
	}

    public void SelectColor (int index) {
		activeColor = colors[index];
        activeElevation = index;
	}

    public void SetElevation (float elevation) {
		activeElevation = (int)elevation;
	}
}