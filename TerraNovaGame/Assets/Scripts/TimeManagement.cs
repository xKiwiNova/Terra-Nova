using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeManagement : MonoBehaviour
{
    private float time;
    public float gameSpeed = 0.002f;

    
    public int day = 1;
    private int dayOfMonth = 1;

    private int month = 1;
    private int monthOfYear = 1;

    private int year = 1;



    public GameObject resourceManager;
    private ResourceManagement resourceManagement;

    public GameObject jobManager;
    private JobSystem jobSystem;
    public GridBuildingSystem gridBuildingSystem;

    public TextMeshProUGUI timeText;
    

    // Start is called before the first frame update
    void Start()
    {
        time = gameSpeed;
    }

    

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        if(time <= 0)
        {
            day += 1;
            time = gameSpeed;
            dayOfMonth += 1;
            UpdateTimeUI();
        }

        if(dayOfMonth == 30)
        {
            month += 1;
            dayOfMonth = 0;
            monthOfYear += 1;

            ResourceManagement resourceManagement = resourceManager.GetComponent<ResourceManagement>();
            JobSystem jobSystem = jobManager.GetComponent<JobSystem>();
            /// GridBuildingSystem gridBuildingSystem = gridBuilder.GetComponent<GridBuildingSystem>()

            resourceManagement.ClearProduction();
            jobSystem.CalculateJobIncome();
            resourceManagement.CalculateAllIncome();
            gridBuildingSystem.CalculateBuildingIncome();
        }

        if(monthOfYear == 11)
        {
            year += 1;
            monthOfYear = 0;
        }
    

    }
    void UpdateTimeUI()
    {
        timeText.text = (monthOfYear + "/" + dayOfMonth + "/" + year);
    }
}
