using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {

	public Color[] colors;

	public HexGrid hexGrid;

	private Color activeColor;

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
    }

    void HandleInput() 
    {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) 
        {
			hexGrid.ColorCell(hit.point, activeColor);
		}
	}

    public void SelectColor (int index) {
		activeColor = colors[index];
	}
}