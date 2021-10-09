using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestBuilding", menuName = "BuildingSO/TestBuilding")]

public class BuildingSO : ScriptableObject
{

    public JobSO[] jobs;

    public static Direction RotateDirection(Direction direction)
    {
        switch(direction)
        {
            default:
            case Direction.Down: return Direction.Left;
            case Direction.Left: return Direction.Up;
            case Direction.Up: return Direction.Right;
            case Direction.Right: return Direction.Down;
        }
    }

    public enum Direction
    {
        Down, // Default direction, pivot in bottom left corner.
        Left, // Pivot in the top left.
        Up, // Pivot in the top right
        Right, // Pivot in the bottom right
    }

    public string buildingName;
    public Transform buildingPrefab;
    public Transform buildingVisual;
    public Transform buildingGhost;
    public int width;
    public int height;
    public string resourceString;

    [System.Serializable]
    public class ProducedResource
    {
        public string resource;
        public int amount;
        public float modifier = 1.0f;
    }
    public ProducedResource[] ProducedResources;
    
    [System.Serializable]
    public class ConsumedResource
    {
        public string resource;
        public int amount;
        public float modifier = 1.0f;
    }
    public ConsumedResource[] ConsumedResources;

    public void UpdateBuildingResourceString(ResourceManagement resourceManagement)
    {
        resourceString = string.Empty;
        foreach(ProducedResource producedResource in ProducedResources)
        {
            ResourceManagement.Resource resource = resourceManagement.GetResourceFromName(producedResource.resource);
            resourceString += ($"{resource.icon} {resource.name} production + {producedResource.amount * producedResource.modifier}");
        }
    }

    public void OnPlaced(List<BuildingSO> buildingList, List<JobSO> allJobList)
    {
        buildingList.Add(this);
        foreach(JobSO job in jobs)
        {
            allJobList.Add(job);
        }
    }

    public void OnDestroyed(List<BuildingSO> buildingList, List<JobSO> allJobList)
    {
        buildingList.Remove(this);
        foreach(JobSO job in jobs)
        {
            allJobList.Remove(job);
        }
    }
    

    public int GetRotationAngle(Direction dir)
    {
        switch(dir)
        {
            default:
            case Direction.Down: return 0;
            case Direction.Left: return 90;
            case Direction.Up: return 180;
            case Direction.Right: return 270;
        }
    }


    public List<Vector2Int> GetGridPositionList(int originX, int originZ, Direction dir)
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch(dir)
        {
            default:
            case Direction.Down:
            for(int x = 0; x < width; x++)
            {
                for(int z = 0; z < height; z++)
                {   
                    gridPositionList.Add(new Vector2Int(x + originX, z + originZ));
                }
            }
            break;

            case Direction.Left:
            for(int x = 0; x < height; x++)
            {
                for(int z = 0; -z < width; z--)
                {   
                    gridPositionList.Add(new Vector2Int(x + originX, z + originZ));
                }
            }
            break;

            case Direction.Up:
            for(int x = 0; -x < width; x--)
            {
                for(int z = 0; -z < height; z--)
                {   
                    gridPositionList.Add(new Vector2Int(x + originX, z + originZ));
                }
            }
            break;

            case Direction.Right:
            for(int x = 0; -x < height; x--)
            {
                for(int z = 0; z < width; z++)
                {   
                    gridPositionList.Add(new Vector2Int(x + originX, z + originZ));
                }
            }
            break;
        }
    return gridPositionList;
    }

    public void GetOffset(Direction dir, out int xOffset, out int zOffset)
    {
        switch(dir)
        {
            default:
            case Direction.Down:
            xOffset = 0;
            zOffset = 0;
            break;

            case Direction.Left:
            xOffset = 0;
            zOffset = 1;
            break;

            case Direction.Up:
            xOffset = 1;
            zOffset = 1;
            break;

            case Direction.Right:
            xOffset = 1;
            zOffset = 0;
            break;
        }
    }
}