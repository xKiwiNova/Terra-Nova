using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "BuildingSO/Building")]
public class Building : ScriptableObject
{
    public string buildingName;
    public GameObject buildingObject;
    public GameObject ghostTransform;

}