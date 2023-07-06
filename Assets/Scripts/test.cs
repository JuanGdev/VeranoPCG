using System;
using System.Collections;
using UnityEngine;


public class test : MonoBehaviour
{
    //  Si dejamos el pivote hacia arriba y hacia abajo, eso nos permite generar más profundida, porque así nos aseguramos
    //  de que las alturas también van hacia abajo
    //  Graphic representation of 2D matrix
    public GameObject cubePrefab; // Prefab del objeto visual de cada celda
    public float cubeSize = 1f; // Tamaño de cada cubo en el plano
    public float[,] height_map;
    public int n;
    public int randomRangeValue, randomAux;
    public int height_map_size;
    public int mapLenght;
    public int chunkSize;

    public bool waterEnabled = true;
    public bool grassEnabled = true;
    public bool earthEnabled = true;
    public bool automaticRange = true;

    public Material earthMaterial;
    public Material waterMaterial;
    public Material grassMaterial;

    public float waterTriggerNorm;
    public float grassTriggerNorm;
    public float earthTriggerNorm;

    public float waterTriggerRange = 0.1f;
    public float grassTriggerRange = 0.2f;
    public float earthTriggerRange = 0.75f;

    public GameObject parent;

    void Start()
    {

        n = UnityEngine.Random.Range(1, 9);


        //  Dimensions of 2^n + 1
        height_map_size = (int)(Mathf.Pow(2, n) + 1);

        height_map = new float[height_map_size, height_map_size];

        mapLenght = height_map.GetLength(0);

        if (automaticRange)
        {
            SetAutomaticRange();
        }
        randomRangeValue = 10;

        //  Random values at the corners of the square
        height_map[0, 0] = UnityEngine.Random.Range(-randomRangeValue, randomRangeValue);
        height_map[0, mapLenght - 1] = UnityEngine.Random.Range(-randomRangeValue, randomRangeValue);
        height_map[mapLenght - 1, 0] = UnityEngine.Random.Range(-randomRangeValue, randomRangeValue);
        height_map[mapLenght - 1, mapLenght - 1] = UnityEngine.Random.Range(-randomRangeValue, randomRangeValue);


        chunkSize = height_map_size - 1;
        earthTriggerNorm = earthTriggerRange * randomRangeValue;
        grassTriggerNorm = grassTriggerRange * randomRangeValue;
        waterTriggerNorm = waterTriggerRange * randomRangeValue;
        randomAux = randomRangeValue;
        while (chunkSize > 1)
        {
            DiamondSquareStep(height_map, chunkSize, randomRangeValue);
            chunkSize /= 2;
            randomRangeValue /= 2;
        }
        randomRangeValue = randomAux;

        BuildHeightMap(height_map);

        InvokeRepeating("ReloadingTerrain", 5f, 2f);
    }

    void BuildHeightMap(float[,] height_map)
    {

        int rows = height_map.GetLength(0);
        int columns = height_map.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = new Vector3(col * cubeSize, 0f, row * cubeSize);

                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);

                //  Para no generar objetos negros con escala de 0
                if (height_map[row, col] == 0)
                {
                    cube.transform.localScale = new Vector3(cubeSize, 0.1f, cubeSize);
                }
                else
                {
                    cube.transform.localScale = new Vector3(cubeSize, height_map[row, col], cubeSize);  //  Instancia del tamaño del cubo
                }


                cube.transform.parent = transform;
                cube.name = height_map[row, col].ToString();

                cube.GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.white, Color.black, height_map[row, col] / 10f);

                //  Water, mountain or grass?
                if ((height_map[row, col] >= earthTriggerNorm || height_map[row, col] <= -earthTriggerNorm) && earthEnabled)
                {
                    cube.GetComponentInChildren<Renderer>().material = earthMaterial;
                }
                else if (((height_map[row, col] >= grassTriggerNorm && height_map[row, col] < earthTriggerNorm) || (height_map[row, col] <= -grassTriggerNorm && height_map[row, col] > -earthTriggerNorm)) && grassEnabled)
                {
                    cube.GetComponentInChildren<Renderer>().material = grassMaterial;
                }
                else if (((height_map[row, col] < grassTriggerNorm) && height_map[row, col] >= -grassTriggerNorm) && waterEnabled)
                {
                    cube.GetComponentInChildren<Renderer>().material = waterMaterial;
                }
                else
                {
                    cube.GetComponentInChildren<Renderer>().material.color = Color.Lerp(Color.white, Color.black, height_map[row, col] / 10f);
                }
            }
        }
    }



    void DiamondSquareStep(float[,] heightMap, int chunkSize, float randomRange)
    {
        int half = chunkSize / 2;
        int mapSize = heightMap.GetLength(0);
        //  Diamond Step
        for (int y = half; y < heightMap.GetLength(0) - 1; y += chunkSize)
        {
            for (int x = half; x < heightMap.GetLength(1) - 1; x += chunkSize)
            {
                float avg = (heightMap[x - half, y - half] +
                                    heightMap[x + half, y - half] +
                                    heightMap[x - half, y + half] +
                                    heightMap[x + half, y + half]
                                    ) / 4f;
                float randomAdd = UnityEngine.Random.Range(-randomRange, randomRange);

                heightMap[x, y] = avg + randomAdd;
            }
        }

        //  Square step
        for (int y = 0; y < heightMap.GetLength(0) - 1; y += half)
        {
            for (int x = (y + half) % chunkSize; x < heightMap.GetLength(1) - 1; x += chunkSize)
            {
                float avg = (heightMap[(x - half + mapSize) % mapSize, y] +
                                    heightMap[(x + half) % mapSize, y] +
                                    heightMap[x, (y + half) % mapSize] +
                                    heightMap[x, (y - half + mapSize) % mapSize]) / 4f;
                float randomAdd = UnityEngine.Random.Range(-randomRange, randomRange);

                heightMap[x, y] = avg + randomAdd;
            }
        }
    }


    //  Declaracion de la corrutina
    void ReloadingTerrain()
    {
        DeleteChilds();
        //  Dimensions of 2^n + 1
        height_map_size = (int)(Mathf.Pow(2, n) + 1);

        if (automaticRange)
        {
            SetAutomaticRange();
        }
        //  Free use for randomRangeValue

        height_map = new float[height_map_size, height_map_size];

        mapLenght = height_map.GetLength(0);

        //  Random values at the corners of the square
        height_map[0, 0] = UnityEngine.Random.Range(-randomRangeValue, randomRangeValue);
        height_map[0, mapLenght - 1] = UnityEngine.Random.Range(-randomRangeValue, randomRangeValue);
        height_map[mapLenght - 1, 0] = UnityEngine.Random.Range(-randomRangeValue, randomRangeValue);
        height_map[mapLenght - 1, mapLenght - 1] = UnityEngine.Random.Range(-randomRangeValue, randomRangeValue);


        chunkSize = height_map_size - 1;
        earthTriggerNorm = earthTriggerRange * randomRangeValue;
        grassTriggerNorm = grassTriggerRange * randomRangeValue;
        waterTriggerNorm = waterTriggerRange * randomRangeValue;
        randomAux = randomRangeValue;
        while (chunkSize > 1)
        {
            DiamondSquareStep(height_map, chunkSize, randomRangeValue);
            chunkSize /= 2;
            randomRangeValue /= 2;
        }

        randomRangeValue = randomAux;
        BuildHeightMap(height_map);
    }

    void DeleteChilds()
    {
        parent = gameObject;
        int totalChilds = parent.transform.childCount;
        // Recorre todos los hijos adjuntos y destrúyelos
        for (int i = 0; i < totalChilds; i++)
        {
            // Obtén el hijo actual
            Transform child = parent.transform.GetChild(i);

            // Destruye el hijo
            Destroy(child.gameObject);
        }
    }

    void SetAutomaticRange()
    {
        //  continuous piecewise function
        switch (n)
        {
            case 1:
                randomRangeValue = 2;
                break;
            case 2:
                randomRangeValue = 4;
                break;
            case 3:
                randomRangeValue = 10;
                break;
            case 4:
                randomRangeValue = 20;
                break;
            case 5:
                randomRangeValue = 25;
                break;
            case 6:
                randomRangeValue = 30;
                break;
            case 7:
                randomRangeValue = 35;
                break;
            case 8:
                randomRangeValue = 40;
                break;
            default:
                randomRangeValue = n / 2;
                break;
        }
    }
}
