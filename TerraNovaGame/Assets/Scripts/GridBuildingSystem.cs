using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    public LayerMask terrainLayer;
    private Grid<GridObject> grid; 
    public BuildingSO building;
    private Vector3 origin;
    private BuildingSO.Direction dir = BuildingSO.Direction.Down; 
    public Transform debugTile;
    public Material ghostMaterial;


    public GameObject resourceManager;
    private ResourceManagement resourceManagement;
    public GameObject jobManager;
    private JobSystem jobSystem;
    private Vector3 lastOrigin;
    private Transform ghostTransform;

    public TerrainGenerator terrainGenerator;

    // Start is called before the first frame update
    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int z;
        private Transform placedBuilding;
        public List<Vector2Int> gridPositionList;
        private BuildingSO buildingSO;
        

        //public List<Vector2Int?> gridPositionList

        public GridObject(Grid<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }
        
        public void SetBuildingSO(BuildingSO buildingSO)
        {
            this.buildingSO = buildingSO;
        }

        public BuildingSO GetBuildingSO()
        {
            return this.buildingSO;
        }

        public void SetGridPositionList(List<Vector2Int> gridPositionList)
        {
            this.gridPositionList = new List<Vector2Int>(gridPositionList);
        }

        public List<Vector2Int> GetGridPositionList()
        {
            return gridPositionList;
        }

        public void ClearGridPositionList()
        {
            this.gridPositionList = null;
        }

        public void SetPlacedBuilding(Transform placedBuilding)
        {
            this.placedBuilding = placedBuilding;
            grid.TriggerGridObjectChanged(x, z);
        }
        public Transform GetPlacedBuilding()
        {
            return placedBuilding;
        }

        public void ClearPlacedBuilding()
        {
            this.placedBuilding = null; 
        }

        public bool HasPlacedBuilding()
        {
            return placedBuilding != null;
        }
        public bool CanBuild()
        {
        return placedBuilding == null;
        }
    }
    
    void Start()
    {
        grid = new Grid<GridObject>(100, 100, 10f, Vector3.zero, (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));
        ghostTransform = Instantiate(building.buildingGhost, Vector3.zero, Quaternion.Euler(0, building.GetRotationAngle(dir), 0));
    }

    // Update is called once per frame
    void Update()
    {
        CreateBuilding();
    }


    public Vector3 TerrainGetWorldPoint()
    {
        RaycastHit terrainHit; 
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out terrainHit, Mathf.Infinity, terrainLayer))
        {
            return terrainHit.point; 
        }
        else
        {
            return Vector3.zero;
        }
    }
    public void CreateBuilding()
    {
        grid.GetXZ(TerrainGetWorldPoint(), out int x, out int z);
        building.GetOffset(dir, out int xOffset, out int zOffset);
        origin = new Vector3(grid.GetWorldPosition(x + xOffset, z + zOffset).x, 3, grid.GetWorldPosition(x + xOffset, z + zOffset).z);
        Destroy(ghostTransform.gameObject);

        ghostTransform = Instantiate(building.buildingGhost, origin, Quaternion.Euler(0, building.GetRotationAngle(dir), 0));

        List<Vector2Int> gridPositionList = building.GetGridPositionList(x, z, dir);
        bool canBuild = true;
        foreach(Vector2Int gridPosition in gridPositionList)
        {
            if(!grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                canBuild = false;
                break;
            }
            if(gridPosition.x >= grid.width || gridPosition.y >= grid.height)
            {
                canBuild = false;
                break;
            }
        }
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

        if(Input.GetMouseButtonDown(0))
        {
            if(canBuild)
            {
                Transform placedBuilding = Instantiate(building.buildingPrefab, origin, Quaternion.Euler(0, building.GetRotationAngle(dir), 0));
                

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetGridPositionList(gridPositionList);
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedBuilding(placedBuilding);
                    grid.GetGridObject(gridPosition.x, gridPosition.y).SetBuildingSO(building);

                    terrainGenerator.grid.GetGridObject(gridPosition.x, gridPosition.y).ClearForestElements();

                    // Instantiate(debugTile, grid.GetWorldPosition(gridPosition.x, gridPosition.y) + new Vector3(grid.cellSize * .5f, grid.cellSize * .25f, grid.cellSize * .5f), Quaternion.identity);
                    // Debug.Log((gridPosition.x) + ",  " + (gridPosition.y) + " has a value of " + grid.GetGridObject(gridPosition.x, gridPosition.y).HasPlacedBuilding() + " and an origin of " + grid.GetGridObject(gridPosition.x, gridPosition.y).GetGridPositionList());
                }

                ResourceManagement resourceManagement = resourceManager.GetComponent<ResourceManagement>();
                
                OnPlaced(building);
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            GridObject gridObject = grid.GetGridObject((Vector3)TerrainGetWorldPoint());
            Transform placedBuilding = gridObject.GetPlacedBuilding();
            BuildingSO buildingSO = gridObject.GetBuildingSO();
            if(placedBuilding != null)
            {
                List<Vector2Int> newGridPositionList = grid.GetGridObject(x, z).GetGridPositionList();
                foreach (Vector2Int newGridPosition in newGridPositionList)
                {
                    grid.GetGridObject(newGridPosition.x, newGridPosition.y).ClearGridPositionList();
                    grid.GetGridObject(newGridPosition.x, newGridPosition.y).ClearPlacedBuilding();
                   //  Debug.Log((gridPosition.x) + ",  " + (gridPosition.y) + " has a value of " + grid.GetGridObject(gridPosition.x, gridPosition.y).HasPlacedBuilding() + " and an origin of " + grid.GetGridObject(gridPosition.x, gridPosition.y).GetGridPositionList());
                }
                OnDestroyed(buildingSO);
                Destroy(placedBuilding.gameObject);
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            dir = BuildingSO.RotateDirection(dir);
        }

        lastOrigin = origin;
    }

    public List<BuildingSO> buildingList;

    public void OnDestroyed(BuildingSO building)
    {
        buildingList.Remove(building);
        foreach(JobSO job in building.jobs)
        {
            JobSystem jobSystem = jobManager.GetComponent<JobSystem>();
            jobSystem.allJobList.Remove(job);

        }
    }

    public void OnPlaced(BuildingSO building)
    {
        buildingList.Add(building);
        foreach(JobSO job in building.jobs)
        {
            JobSystem jobSystem = jobManager.GetComponent<JobSystem>();
            jobSystem.allJobList.Add(job);
        }
    }

   
    public void CalculateBuildingIncome()
    {
        ResourceManagement resourceManagement = resourceManager.GetComponent<ResourceManagement>();
        foreach(BuildingSO building in buildingList)
        {
            foreach(BuildingSO.ProducedResource producedResource in building.ProducedResources)
            {
                resourceManagement.CalculateBuildingProduction(producedResource.resource, producedResource.amount, producedResource.modifier);
            }
            foreach(BuildingSO.ConsumedResource consumedResource in building.ConsumedResources)
            {
                resourceManagement.CalculateBuildingConsumption(consumedResource.resource, consumedResource.amount, consumedResource.modifier);
            }
        }
    }
}
