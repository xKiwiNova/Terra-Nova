using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Stores and holds information about each Cell
public class HexCell : MonoBehaviour
{

    [SerializeField]
    HexCell[] neighbors = new HexCell[6];
    public HexCoordinates coordinates;
    private Color color;
    public RectTransform debugUIRect;
    public TerrainData terrainData;
    public HexGridChunk chunk;

    public int elevation
    {
        get
        {
            return this.terrainData.elevation;
        }
        set
        {
            terrainData.elevation = value;
            Vector3 position = transform.localPosition;
			position.y = value * HexMetrics.elevationStep;
            position.y +=
                (HexMetrics.SampleNoise(position).y * 2f - 1f) *
				HexMetrics.elevationPerturbStrength;
			transform.localPosition = position;

            Vector3 uiPosition = debugUIRect.localPosition;
			uiPosition.z = -position.y;
			debugUIRect.localPosition = uiPosition;
        }
    }

    public Vector3 Position 
    {
        get
        {
            return transform.localPosition;
        }
    }

    public Color Color
    {
        get 
        {
			return color;
		}
		set {
			if (color == value) 
            {
				return;
			}
			color = value;
			Refresh();
		}
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public void GenerateTerrainData(int elevation, int percipitation, int temperature)
    {
        terrainData = new TerrainData(elevation, percipitation, temperature);
        this.elevation = elevation;

        // color = Color.HSVToRGB((float)(elevation / 20.0f - .04), 0.8f, 0.8f);
        color = Color.HSVToRGB((float)(elevation / 18.0f - .04), 0.8f, 0.8f);
    }

    public void Refresh()
    {
        if(chunk)
        {
            chunk.Refresh();
            for(int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if(neighbor != null && neighbor.chunk != chunk) 
                {
					neighbor.chunk.Refresh();
				}
            }
        }
    }
}
