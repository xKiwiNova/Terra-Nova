using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class AddScore : MonoBehaviour {
            public TextMeshProUGUI PointText;
            public static int points;

    // Trigger on Enter
    void OnTriggerEnter(Collider collider)
    {
        // if collider is Player
        if (collider.gameObject.tag == "Player")
        {
            // Add one point, destroy the object
            points++;
            PointText.text = "Points - " + points; 
            Destroy(gameObject);
        }
    }
}