using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // When you right click
        {
           Ray ray = cam.ScreenPointToRay(Input.mousePosition); // Create a ray from camera to mouseposition
           RaycastHit hit; 

           if (Physics.Raycast(ray, out hit)) // If it hits something
           {
                // Move the agent
                agent.SetDestination(hit.point);
           }
        }
    }
}
