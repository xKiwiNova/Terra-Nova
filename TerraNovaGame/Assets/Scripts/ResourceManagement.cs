using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManagement : MonoBehaviour
{
    public List<Resource> resourceList;
    public Resource food;
    public Resource energy;
    public Resource credits;
    public Resource minerals;
    public Resource nullResource;

    public TextMeshProUGUI resourceText;
    private string resourceString;

    [System.Serializable]
    public class Resource
    {
        public string name;
        public int amount;

        public int jobProduction = 0;
        public float jobProductionModifier = 1.0f;
        public int jobConsumption = 0;
        public float jobConsumptionModifier = 1.0f;

        public int popProduction = 0;
        public float popProductionModifier = 1.0f;
        public int popConsumption = 0;
        public float popConsumptionModifier = 1.0f;

        public int buildingProduction = 0;
        public float buildingProductionModifier = 1.0f;
        public int buildingConsumption = 0;
        public float buildingConsumptionModifier = 1.0f;

        public int baseProduction;
        public int otherProduction = 0;

        public int baseConsumption;
        public int otherConsumption = 0;

        public int monthlyIncome;
        public int storageCapacity;

        public Resource(string name, int amount, int baseProduction, int baseConsumption, int storageCapacity)
        {
            this.name = name;
            this.amount = amount;
            this.baseProduction = baseProduction;
            this.baseConsumption = baseConsumption;
            this.storageCapacity = storageCapacity;

            /*this.jobProduction = jobProduction;
            this.jobProductionModifier = jobProductionModifier;
            this.jobConsumption = jobConsumption;
            this.jobConsumptionModifier = jobConsumptionModifier;

            this.popProduction = popProduction;
            this.popProductionModifier = popProductionModifier;
            this.popConsumption = popConsumption;
            this.popConsumptionModifier = popConsumptionModifier;

            this.buildingProduction = buildingProduction;
            this.buildingProductionModifier = buildingProductionModifier;
            this.buildingConsumption = buildingConsumption;
            this.buildingConsumptionModifier = buildingConsumptionModifier;*/

            
        }

        public void AddJobProduction(int addedProduction)
        {
            this.jobProduction += addedProduction;
        }

        public void AddJobProductionModifier(int addedModifier)
        {
            this.jobProductionModifier += addedModifier;
        }

        public void AddPopProduction(int addedProduction)
        {
            this.popProduction += addedProduction;
        }

        public void AddPopProductionModifier(int addedModifier)
        {
            this.popProductionModifier += addedModifier;
        }

        public void AddBuildingProduction(int addedProduction)
        {
            this.buildingProduction += addedProduction;
        }

        public void AddBuildingProductionModifier(int addedModifier)
        {
            this.buildingProductionModifier += addedModifier;
        }

        public void AddJobConsumption(int addedConsumption)
        {
            this.jobConsumption += addedConsumption;
        }

        public void AddJobConsumptionModifier(int addedModifier)
        {
            this.jobConsumptionModifier += addedModifier;
        }

        public void AddPopConsumption(int addedConsumption)
        {
            this.popConsumption += addedConsumption;
        }

        public void AddPopConsumptionModifier(int addedModifier)
        {
            this.popConsumptionModifier += addedModifier;
        }

        public void AddBuildingConsumption(int addedConsumption)
        {
            this.buildingConsumption += addedConsumption;
        }

        public void AddBuildingConsumptionModifier(int addedModifier)
        {
            this.buildingConsumptionModifier += addedModifier;
        }

        public void CalculateMonthlyIncome()
        {
            this.monthlyIncome = (int)(
                (jobProduction * jobProductionModifier) + (popProduction * popProductionModifier) + (buildingProduction * buildingProductionModifier) + baseProduction + otherProduction - 
                (jobConsumption * jobConsumptionModifier) - (popConsumption * popConsumptionModifier) - (buildingConsumption * buildingConsumptionModifier) - baseConsumption - otherConsumption
            );
        }

        public void AddIncome()
        {
            if(this.amount + this.monthlyIncome <= this.storageCapacity)
            {
                this.amount += this.monthlyIncome;
            }
            else
            {
                this.amount += this.storageCapacity - this.amount;
            }
        }

    }


    // Start is called before the first frame update
    void Awake()
    {
        // Resource(string name, int amount, int baseProduction, int baseConsumption, int storageCapacity)
        nullResource = new Resource("null", 0, 0, 0, 0);
        
        food = new Resource("food", 0, 100, 0, 10000);
        energy = new Resource("energy", 0, 100, 0, 10000);
        credits = new Resource("credits", 0, 100, 0, 10000);
        minerals = new Resource("minerals", 0, 100, 0, 10000);

        Resource[] resources = {food, energy, credits, minerals};
        resourceList = new List<Resource>(resources);
        UpdateResourceUI();
    }

    public Resource GetResourceFromName(string name)
    {
        switch(name)
        {
            default: return nullResource;
            case "food": return food; break;
            case "energy": return energy; break;
            case "credits": return credits; break;
            case "minerals": return minerals; break;
        }
        
    }

    public void ClearProduction()
    {
        foreach(Resource resource in resourceList)
        {
            resource.jobProduction = 0;
            resource.popProduction = 0;
            resource.buildingProduction = 0;
        }
    }

    public void CalculateJobProduction(string resourceString, int amount, float modifier)
    {
        Resource resource = GetResourceFromName(resourceString);
        resource.AddJobProduction((int)(amount * modifier));
    }

    public void CalculateJobConsumption(string resourceString, int amount, float modifier)
    {
        Resource resource = GetResourceFromName(resourceString);
        resource.AddJobConsumption((int)(amount * modifier));
    }

    public void CalculateBuildingProduction(string resourceString, int amount, float modifier)
    {
        Resource resource = GetResourceFromName(resourceString);
        resource.AddBuildingProduction((int)(amount * modifier));
    }

    public void CalculateBuildingConsumption(string resourceString, int amount, float modifier)
    {
        Resource resource = GetResourceFromName(resourceString);
        resource.AddBuildingConsumption((int)(amount * modifier));
    }

    public void CalculateAllIncome()
    {
        foreach(Resource resource in resourceList)
        {
            resource.CalculateMonthlyIncome();
            resource.AddIncome();
            UpdateResourceUI();
        }
    }

    void UpdateResourceUI()
    {
        resourceString = string.Empty;
        foreach(Resource resource in resourceList)
        {
            resourceString += ($"<sprite={resourceList.IndexOf(resource)}> {resource.amount} + {resource.monthlyIncome} \n");
        }
        resourceText.text = resourceString;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
