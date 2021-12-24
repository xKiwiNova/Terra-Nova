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

    public Color[] triangleColors = new Color[6];

    public List<GameObject> forestElements = new List<GameObject>();

    public int elevation
    {
        get
        {
            return terrainData.elevation;
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

    public int precipitation
    {
        get
        {
            return terrainData.precipitation;
        }
        set
        {
            terrainData.precipitation = value;
        }
    }

    public int temperature
    {
        get
        {
            return terrainData.temperature;
        }
        set
        {
            terrainData.temperature = value;
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

            for(int i = 0; i < 6; i++)
            {
                Color tColor = color * Random.Range(.9f, 1.1f);
                triangleColors[i] = color * Random.Range(.9f, 1.1f);
                HexCell neighbor = GetNeighbor((HexDirection)i);
                if(neighbor != null && EqualsInRange(color, neighbor.Color, .05f))
                {
                    neighbor.SetColor(((HexDirection)i).Opposite(), color);
                }
            }
			Refresh();
		}
    }

    public Color GetColor(HexDirection direction)
    {
        return triangleColors[(int)direction];
    }
    public void SetColor(HexDirection direction, Color color)
    {
        triangleColors[(int)direction] = color;
    }


    public bool EqualsInRange(Color color1, Color color2, float range)
    {
        return(
            Mathf.Abs(color1.r - color2.r) <= range &&
            Mathf.Abs(color1.g - color2.g) <= range &&
            Mathf.Abs(color1.b - color2.b) <= range
            );
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

    public void GenerateTerrainData()
    {
        terrainData = new TerrainData();

        // color = Color.HSVToRGB((float)(elevation / 20.0f - .04), 0.8f, 0.8f);
    }

    public void UpdateTerrainData(int elevation, int precipitation, int temperature)
    {
        this.elevation = elevation;
        this.precipitation = precipitation;
        this.temperature = temperature;

        Color = Color.HSVToRGB((float)(elevation / 18.0f - .04), 0.8f, 0.8f);
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

    public void RefreshSelfOnly()
    {
        chunk.Refresh();
    }
}
