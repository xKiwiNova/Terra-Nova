using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public LayerMask unitsLayer;
    

    private List<GameObject> selectedObjects;
    [HideInInspector]
    public List<GameObject> selectableObjects;
    Vector3 mousePos1;
    Vector3 mousePos2;

    void Awake()
    {
        selectedObjects = new List<GameObject>();
        selectableObjects = new List<GameObject>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
        mousePos1 = Camera.main.ScreenToViewportPoint(Input.mousePosition);

        RaycastHit selectionHit;  
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out selectionHit, Mathf.Infinity, unitsLayer))
            {
                UnitSelect selectScript = selectionHit.collider.GetComponent<UnitSelect>();
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (selectScript.isSelected == false)
                    {
                        selectedObjects.Add(selectionHit.collider.gameObject);
                        selectScript.isSelected = true;
                        selectScript.Select();
                    }
                    else
                    {
                        selectedObjects.Remove(selectionHit.collider.gameObject);
                        selectScript.isSelected = false;
                    }
                }
                else
                {
                   ClearSelection();
                }

                selectedObjects.Add(selectionHit.collider.gameObject);
                selectScript.isSelected = true;
                selectScript.Select();
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                ClearSelection();
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            mousePos2 = Camera.main.ScreenToViewportPoint(Input.mousePosition);

            if (mousePos1 != mousePos2)
            {
                SelectObjects();
            }
        }
    }
    void SelectObjects()
    {
        List<GameObject> remObjects = new List<GameObject>();
       if (Input.GetKey(KeyCode.LeftShift) == false)
       {
           ClearSelection();
       }
       Rect selectRect = new Rect(mousePos1.x, mousePos1.y, mousePos2.x - mousePos1.x, mousePos2.y - mousePos1.y);
        foreach (GameObject selectObject in selectableObjects)
        {
            if (selectObject != null)
            {
                if (selectRect.Contains(Camera.main.WorldToViewportPoint(selectObject.transform.position), true))
                {
                    selectedObjects.Add(selectObject);
                    selectObject.GetComponent<UnitSelect>().isSelected = true;
                    selectObject.GetComponent<UnitSelect>().Select();
                }
            }
            else
            {
                remObjects.Add(selectObject);
            }
            
        }
        if (remObjects.Count > 0)
        {
            foreach (GameObject rem in remObjects)
            {
                selectableObjects.Remove(rem);
            }
            remObjects.Clear();
        }
    }
    void ClearSelection()
    {
       if (selectedObjects.Count > 0)
        {
            foreach (GameObject obj in selectedObjects)
            {
                obj.GetComponent<UnitSelect>().isSelected = false;
                obj.GetComponent<UnitSelect>().Select();
            }
                selectedObjects.Clear();        
        }
    }
}
