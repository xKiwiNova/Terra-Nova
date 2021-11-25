using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapEditor : MonoBehaviour
{

    public Color[] colors;
    Color activeColor;
    int activeElevation;

    public HexGrid hexGrid;

    void Awake()
    {
        SelectColor(0);
    }

    void Update()
    {   
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("White");
            SelectColor(0);
            activeElevation = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Red");
            SelectColor(1); 
            activeElevation = 2;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Yellow");
            SelectColor(2);
            activeElevation = 3;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Green");
            SelectColor(3);
            activeElevation = 4;
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Blue");
            SelectColor(4);
            activeElevation = 5;
        }
        if(Input.GetMouseButtonDown(0))
        {
            HandleInput();
        }
        
    }
    void HandleInput()
    {  
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(inputRay, out hit))
        {
            Debug.Log((hexGrid.GetCell(hit.point).terrainData.elevation));
        }
    }

    void EditCell(HexCell cell)
    {
        cell.Color = activeColor;
        cell.elevation = activeElevation;
		cell.Refresh();
    }

	void SelectColor (int index) 
    {
		activeColor = colors[index];
	}
}
