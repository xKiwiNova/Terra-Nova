using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "Job", menuName = "JobSO/Job")]
public class JobSO : ScriptableObject
{
    public string name; 
    public bool isOccupied = true;

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

}