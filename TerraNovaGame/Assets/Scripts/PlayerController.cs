using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    public Transform player;
    Vector3 lastPosition;


    // Update is called once per frame
    void Start()
    {

    }
    void Update()
    {
        UnitSelect selectScript = GetComponent<UnitSelect>();
        if (selectScript.isSelected == true)
        {
            if (Input.GetMouseButtonDown(1)) // When you right click
            {
           Ray moveRay = cam.ScreenPointToRay(Input.mousePosition); // Create a ray from camera to mouseposition
           RaycastHit hit; 

                if (Physics.Raycast(moveRay, out hit)) // If it hits something
                {
                // Move the agent
                agent.SetDestination(hit.point);
               
                }
        
            }
        }
    }
}
