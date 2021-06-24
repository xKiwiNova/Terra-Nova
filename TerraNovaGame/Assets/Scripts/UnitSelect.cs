using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelect : MonoBehaviour
{   
    Shader selectedShader;
    Shader defaultShader;

    private MeshRenderer myRend;
    public bool isSelected = false; 

    // Start is called before the first frame update
    void Start()
    {
        myRend = GetComponent<MeshRenderer>();
        Camera.main.gameObject.GetComponent<UnitSelection>().selectableObjects.Add(this.gameObject);
        selectedShader = Shader.Find("Shader Graphs/Selected");
        defaultShader = Shader.Find("Universal Render Pipeline/Lit");
        Select();
    }

    // Update is called once per frame
    public void Select()
    {
        if (isSelected == false)
        {
            myRend.material.shader = defaultShader;
        }
        else
        {
            myRend.material.shader = selectedShader;
        }

    }
}
