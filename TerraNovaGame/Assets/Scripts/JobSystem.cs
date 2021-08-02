using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSystem : MonoBehaviour
{
    public GameObject resourceManager;
    private ResourceManagement resourceManagement;
    public List<JobSO> allJobList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CalculateJobIncome()
    {
        ResourceManagement resourceManagement = resourceManager.GetComponent<ResourceManagement>();
        
        foreach(JobSO job in allJobList)
        {
            if(job.isOccupied == true)
            {
                foreach(JobSO.ProducedResource producedResource in job.ProducedResources)
                {
                    resourceManagement.CalculateJobProduction(producedResource.resource, producedResource.amount, producedResource.modifier);
                }
                foreach(JobSO.ConsumedResource consumedResource in job.ConsumedResources)
                {
                    resourceManagement.CalculateJobConsumption(consumedResource.resource, consumedResource.amount, consumedResource.modifier);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
