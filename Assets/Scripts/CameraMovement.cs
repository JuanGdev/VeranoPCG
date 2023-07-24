using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class CameraMovement : MonoBehaviour
{

    private TerrainGenerator terrainScript;
    public Vector3 targetPosition;
    public int halfPoint;
    public Vector3 newPosition;
    public Vector3 initialPosition;

    private void Start()
    {
        //  La posicion inicial de la camara debe ser tambien la mitad del mapa de altura
        //  despues debo recorrer el mapa hacia la altura maxima del mismo y 1.5 veces en x y z el tamano del mapa        
        terrainScript = GameObject.Find("Terrain Data").GetComponent<TerrainGenerator>();

        halfPoint = terrainScript.m_heightMapSize / 2;
        initialPosition.x = 10 + halfPoint * 2;
        initialPosition.y = 10 + (terrainScript.FindMaxValue(terrainScript.m_heightMap) * 2);
        initialPosition.z = 10 + halfPoint * 2;
        targetPosition = new Vector3(halfPoint, 10f, halfPoint);
        transform.position = initialPosition;
    }

    private void FixedUpdate()
    {
        // Hace que la cámara mire hacia la posición objetivo
        transform.LookAt(targetPosition);
        // Orbita alrededor de la posición objetivo
        transform.RotateAround(targetPosition, Vector3.up, 10f * Time.deltaTime);
    }
}
