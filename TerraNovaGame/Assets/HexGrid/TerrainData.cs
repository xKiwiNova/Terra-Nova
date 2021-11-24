using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainData
{
    public int elevation;
    public int precipitation;
    public int temperature;

    public TerrainData(int elevation, int precipitation, int temperature)
    {
        this.elevation = elevation;
        this.precipitation = precipitation;
        this.temperature = temperature;
    }
}
