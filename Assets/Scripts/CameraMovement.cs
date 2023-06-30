using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    public GameObject surface;
    public float offset;

    // Start is called before the first frame update
    void Start()
    {
        // Obtiene la referencia al Transform del objeto objetivo
        Transform targetTransform = surface.transform;

        // Apunta la c�mara hacia el objeto objetivo
        transform.LookAt(targetTransform);
    }

    // Update is called once per frame
    void Update()
    {
        // Define la velocidad de rotaci�n deseada
        float rotationSpeed = 10.0f;
        // Rota la c�mara alrededor del objeto objetivo
        transform.RotateAround(surface.transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
