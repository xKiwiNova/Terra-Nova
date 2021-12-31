using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{   
    [SerializeField]
    private HexMap map;

    public Building building;
    public HexDirection direction = HexDirection.NW;
    Transform ghostTransform;

    public Material ghostMaterial;
    
    void Update()
    {
        if(ghostTransform != null)
        {
            Destroy(ghostTransform.gameObject);
        }
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, map.layer))
        {
            HexTile tile = map.FromPosition(hit.point);
            bool canBuild = tile.building == null;

            ghostMaterial.EnableKeyword("_EMISSION");

            if (canBuild == false) 
            {
                ghostMaterial.SetColor("_FresnelColor", Color.red * 2);
                ghostMaterial.SetColor("_MainColor", Color.red * 1);
            }

            else
            {
                ghostMaterial.SetColor("_FresnelColor", Color.green * 10);
                ghostMaterial.SetColor("_MainColor", Color.green * 5);
            }

            Vector3 position = tile.position;
            Quaternion rotation = Quaternion.Euler(0, GetRotation(direction), 0);

            ghostTransform = Instantiate(building.ghostTransform.transform, position, rotation);
            if(Input.GetMouseButtonDown(0))
            {

                if(canBuild)
                {
                    Transform buildingTransform = Instantiate(building.buildingTransform.transform, position, rotation);
                    tile.building = buildingTransform;
                }              
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            direction = direction.Next();
        }
    }

    public int GetRotation(HexDirection direction)
    {
        return (int)direction * 60;
    }
}
