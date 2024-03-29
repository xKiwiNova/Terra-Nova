﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    Transform swivel;
    Transform stick;

    float zoom = 1f;

    public float stickMinZoom;
    public float stickMaxZoom;

    public float swivelMinZoom;
    public float swivelMaxZoom;
    
    public float movementSpeedMinZoom;
    public float movementSpeedMaxZoom;

    public HexMap map;

    public float rotationSpeed;
    float rotationAngle;

    void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if(zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if(xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }

        float rotationDelta = Input.GetAxis("Rotation");
        if(rotationDelta != 0f)
        {
            AdjustRotation(rotationDelta);
        }
    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
		stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
		swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    void AdjustPosition(float xDelta, float zDelta)
    {
        float distance = Mathf.Lerp(movementSpeedMinZoom, movementSpeedMaxZoom, zoom) * Time.deltaTime;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;

        Vector3 position = transform.localPosition;
        position += direction * distance  * damping;
        transform.localPosition = ClampPosition(position);
    }

    // This method keeps the camera from moving out of bounds 
    Vector3 ClampPosition(Vector3 position) 
    {
        float xMax = (map.tileCountX - 0.5f) * (1.5f * Hexagon.outerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = (map.tileCountX) * (2f * Hexagon.innerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }

    void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;

		if(rotationAngle < 0f) 
        {
			rotationAngle += 360f;
		}
		else if(rotationAngle >= 360f) 
        {
			rotationAngle -= 360f;
		}

		transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }
}
