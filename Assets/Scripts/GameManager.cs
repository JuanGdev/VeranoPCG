using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private TerrainGenerator terrainGeneratorScript;
    // Start is called before the first frame update
    void Start()
    {
        terrainGeneratorScript = GameObject.Find("Terrain Data").GetComponent<TerrainGenerator>();
        Debug.Log("Heightmap size: " + terrainGeneratorScript.m_heightMapSize);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
