using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class CameraMovement : MonoBehaviour
{

    private TerrainGenerator terrainScript;
    public Vector3 targetPosition;
    public int halfPoint;

    private void Start()
    {
        terrainScript = GameObject.Find("Terrain Data").GetComponent<TerrainGenerator>();
        halfPoint = terrainScript.m_heightMapSize / 2;
        targetPosition = new Vector3(halfPoint, 10f, halfPoint);
    }

    private void FixedUpdate()
    {
        // Hace que la c�mara mire hacia la posici�n objetivo
        transform.LookAt(targetPosition);

        // Orbita alrededor de la posici�n objetivo
        transform.RotateAround(targetPosition, Vector3.up, 10f * Time.deltaTime);

    }
}
