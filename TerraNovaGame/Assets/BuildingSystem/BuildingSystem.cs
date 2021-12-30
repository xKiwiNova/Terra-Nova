using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{   
    [SerializeField]
    private HexMap map;

    public Building building;
    public HexDirection direction = HexDirection.NW;
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, map.layer))
            {
                HexTile tile = map.FromPosition(hit.point);
                PlaceBuilding(tile, building, direction);
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            direction = direction.Next();
        }
    }

    void PlaceBuilding(HexTile tile, Building building, HexDirection direction)
    {
        bool CanBuild = tile.building == null;
        if(CanBuild)
        {
            Vector3 positon = tile.position;
            Quaternion rotation = Quaternion.Euler(0, GetRotation(direction), 0);

            Transform buildingTransform = Instantiate(building.buildingObject.transform, positon, rotation);
            tile.building = buildingTransform;
        }
    }

    public int GetRotation(HexDirection direction)
    {
        return (int)direction * 60;
    }
}
