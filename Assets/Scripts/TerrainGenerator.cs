using System;
using System.Collections;
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{
    public int m_n;
    public int m_heightMapSize;
    public bool m_automaticRange;
    public float m_randomRange;
    public float m_auxRandomRange;

    public float m_earthTriggerRange;
    public float m_grassTriggerRange;
    public float m_waterTriggerRange;

    public float m_waterTriggerNorm;
    public float m_grassTriggerNorm;
    public float m_earthTriggerNorm;

    public Material m_earthMaterial;
    public Material m_waterMaterial;
    public Material m_grassMaterial;
    public bool m_grassEnabled;
    public bool m_earthEnabled;
    public bool m_waterEnabled;
    public int m_chunkSize;
    public float[,] m_heightMap;

    private int m_half;

    public GameObject cubePrefab;
    public bool DeleteTerrain = false;

    //  methods
    public void ClearMatrix()
    {
        for (int i = 0; i < m_heightMapSize; i++)
        {
            for (int j = 0; j < m_heightMapSize; j++)
            {
                m_heightMap[i, j] = 0;
            }
        }
    }

    public float SetAutomaticRange(int n, float randomRange)
    {
        //  continuous piecewise function
        switch (n)
        {
            case 1:
                randomRange = 2;
                break;
            case 2:
                randomRange = 4;
                break;
            case 3:
                randomRange = 10;
                break;
            case 4:
                randomRange = 15;
                break;
            case 5:
                randomRange = 20;
                break;
            case 6:
                randomRange = 30;
                break;
            case 7:
                randomRange = 35;
                break;
            case 8:
                randomRange = 40;
                break;
            default:
                randomRange = n / 2;
                break;
        }
        return randomRange;
    }

    void SetRandomCorners(float[,] heightMap, float randomRange, int heightMapSize)
    {
        //  Random values at the corners of the square
        heightMap[0, 0] = UnityEngine.Random.Range(-randomRange, randomRange);
        heightMap[0, heightMapSize - 1] = UnityEngine.Random.Range(-randomRange, randomRange);
        heightMap[heightMapSize - 1, 0] = UnityEngine.Random.Range(-randomRange, randomRange);
        heightMap[heightMapSize - 1, heightMapSize - 1] = UnityEngine.Random.Range(-randomRange, randomRange);
    }


    public void BuildHeightmap(float[,] heightMap, GameObject cubePrefab)
    {
        int heightmap_rows = heightMap.GetLength(0);
        int heightmap_columns = heightMap.GetLength(1);
        for (int row = 0; row < heightmap_rows; row++)
        {
            for (int col = 0; col < heightmap_columns; col++)
            {
                Vector3 position = new Vector3(col * 1, 0f, row * 1);

                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                FixCubeSize(heightMap, row, col, cube);

                //                    cube.transform.parent = transform;
                cube.name = heightMap[row, col].ToString();
                //cube.GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.white, Color.black, heightMap[row, col]); ;

                //  Water, mountain or grass?
                SetMaterial(heightMap, row, col, cube);


            }
        }
    }
    public float[,] DiamondSquareStep()
    {
        m_half = m_chunkSize / 2;
        for (int y = m_half; y < m_heightMapSize - 1; y += m_chunkSize)
        {
            for (int x = m_half; x < m_heightMapSize - 1; x += m_chunkSize)
            {
                float avg = (m_heightMap[x - m_half, y - m_half] +
                 m_heightMap[x + m_half, y - m_half] +
                 m_heightMap[x - m_half, y + m_half] +
                 m_heightMap[x + m_half, y + m_half]
                 ) / 4f;
                float randomAdd = UnityEngine.Random.Range(-m_randomRange, m_randomRange);

                m_heightMap[x, y] = avg + randomAdd;
            }
        }
        //  Square step
        for (int y = 0; y < m_heightMapSize - 1; y += m_half)
        {
            for (int x = (y + m_half) % m_chunkSize; x < m_heightMapSize - 1; x += m_chunkSize)
            {
                float avg = (m_heightMap[(x - m_half + m_heightMapSize) % m_heightMapSize, y] +
                                    m_heightMap[(x + m_half) % m_heightMapSize, y] +
                                    m_heightMap[x, (y + m_half) % m_heightMapSize] +
                                    m_heightMap[x, (y - m_half + m_heightMapSize) % m_heightMapSize]) / 4f;
                float randomAdd = UnityEngine.Random.Range(-m_randomRange, m_randomRange);

                m_heightMap[x, y] = avg + randomAdd;
            }
        }
        return m_heightMap;
    }
    private void SetMaterial(float[,] height_map, int row, int col, GameObject cube)
    {
        //  Water, mountain or grass?
        if ((height_map[row, col] >= m_earthTriggerNorm || height_map[row, col] <= -m_earthTriggerNorm) && m_earthEnabled)
        {
            cube.GetComponentInChildren<Renderer>().material = m_earthMaterial;
        }
        else if (((height_map[row, col] >= m_grassTriggerNorm && height_map[row, col] < m_earthTriggerNorm) || (height_map[row, col] <= -m_grassTriggerRange && height_map[row, col] > -m_earthTriggerNorm)) && m_grassEnabled)
        {
            cube.GetComponentInChildren<Renderer>().material = m_grassMaterial;
        }
        else if (((height_map[row, col] < m_grassTriggerNorm) && height_map[row, col] >= -m_grassTriggerNorm) && m_waterEnabled)
        {
            cube.GetComponentInChildren<Renderer>().material = m_waterMaterial;
        }
        else
        {
            cube.GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.white, Color.black, height_map[row, col] / 10f);
        }
    }
    void FixCubeSize(float[,] heightmap, int row, int col, GameObject cube)
    {
        if (heightmap[row, col] == 0)
        {
            cube.transform.localScale = new Vector3(1, 0.1f, 1);
        }
        else
        {
            cube.transform.localScale = new Vector3(1, heightmap[row, col], 1);  //  Instancia del tamaño del cubo
        }
    }

    public void ReloadTerrain()
    {
        DestroyCubes();
        ClearMatrix();
        //  Dimensions of 2^n + 1
        m_heightMapSize = (int)(Mathf.Pow(2, m_n) + 1);
        m_heightMap = new float[m_heightMapSize, m_heightMapSize];

        m_chunkSize = m_heightMapSize - 1;

        if (m_automaticRange)
        {
            m_randomRange = SetAutomaticRange(m_n, m_randomRange);
        }

        m_earthTriggerNorm = m_earthTriggerRange * m_randomRange;
        m_grassTriggerNorm = m_grassTriggerRange * m_randomRange;
        m_waterTriggerNorm = m_waterTriggerRange * m_randomRange;


        SetRandomCorners(m_heightMap, m_randomRange, m_heightMapSize);
        m_auxRandomRange = m_randomRange;
        while (m_chunkSize > 1)
        {
            DiamondSquareStep();
            m_chunkSize /= 2;
            m_randomRange /= 2;
        }
        m_randomRange = m_auxRandomRange;

        BuildHeightmap(m_heightMap, cubePrefab);
    }

    public void DestroyCubes()
    {
        // Buscar todos los objetos con el tag especificado
        GameObject[] objectsToDelete = GameObject.FindGameObjectsWithTag("TerrainCube");
        // Eliminar cada objeto encontrado
        foreach (GameObject obj in objectsToDelete)
        {
            Destroy(obj);
        }
    }




    private void Awake()
    {
        m_n = UnityEngine.Random.Range(1, 9);
        m_earthTriggerRange = 0.75f;
        m_grassTriggerRange = 0.2f;
        m_waterTriggerRange = 0.1f;
        //  Dimensions of 2^n + 1
        m_heightMapSize = (int)(Mathf.Pow(2, m_n) + 1);
        m_heightMap = new float[m_heightMapSize, m_heightMapSize];

        m_chunkSize = m_heightMapSize - 1;
        m_automaticRange = true;
        m_waterEnabled = true;
        m_earthEnabled = true;
        m_grassEnabled = true;

        if (m_automaticRange)
        {
            m_randomRange = SetAutomaticRange(m_n, m_randomRange);
        }

        m_earthTriggerNorm = m_earthTriggerRange * m_randomRange;
        m_grassTriggerNorm = m_grassTriggerRange * m_randomRange;
        m_waterTriggerNorm = m_waterTriggerRange * m_randomRange;

        SetRandomCorners(m_heightMap, m_randomRange, m_heightMapSize);
    }

    void Start()
    {
        m_auxRandomRange = m_randomRange;
        while (m_chunkSize > 1)
        {
            DiamondSquareStep();
            m_chunkSize /= 2;
            m_randomRange /= 2;
        }
        m_randomRange = m_auxRandomRange;

        BuildHeightmap(m_heightMap, cubePrefab);
    }

    private void Update()
    {
        //  Revisar el algoritmo de Diamante, randomRange y auxRandomRange
        //  Spawnear un terreno al lado del primero para comparar
    }
}

