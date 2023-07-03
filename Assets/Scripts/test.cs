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
    public int randomVal, randomAux;
    public int height_map_size;
    public int mapLenght;
    public int chunkSize;

    public bool waterEnabled;
    public bool grassEnabled;
    public bool earthEnabled;


    public Material earthMaterial;
    public Material waterMaterial;
    public Material grassMaterial;

    public float waterTrigger;
    public float grassTrigger;
    public float earthTrigger;

    public GameObject parent;

    void Start()
    {

        earthTrigger = 7f;
        grassTrigger = 2f;
        waterTrigger = 1f;
        n = UnityEngine.Random.Range(1, 8);


        //  Dimensions of 2^n + 1
        height_map_size = (int)(Mathf.Pow(2, n) + 1);

        height_map = new float[height_map_size, height_map_size];

        mapLenght = height_map.GetLength(0);

        //  Random values at the corners of the square
        height_map[0, 0] = UnityEngine.Random.Range(1, 10);
        height_map[0, mapLenght - 1] = UnityEngine.Random.Range(1, 10);
        height_map[mapLenght - 1, 0] = UnityEngine.Random.Range(1, 10);
        height_map[mapLenght - 1, mapLenght - 1] = UnityEngine.Random.Range(1, 10);


        chunkSize = height_map_size - 1;
        randomVal = 30;
        randomAux = randomVal;
        while (chunkSize > 1)
        {
            DiamondSquareStep(height_map, chunkSize, randomVal);
            chunkSize /= 2;
            randomVal /= 2;
        }
        randomVal = randomAux;

        BuildHeightMap(height_map);

        InvokeRepeating("ReloadingTerrain", 5f, 5f);
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
                if ((height_map[row, col] >= earthTrigger || height_map[row, col] <= -earthTrigger) && earthEnabled)
                {
                    cube.GetComponentInChildren<Renderer>().material = earthMaterial;
                }
                else if (((height_map[row, col] >= grassTrigger && height_map[row, col] < earthTrigger) || (height_map[row, col] <= -grassTrigger && height_map[row, col] > -earthTrigger)) && grassEnabled)
                {
                    cube.GetComponentInChildren<Renderer>().material = grassMaterial;
                }
                else if (((height_map[row, col] < grassTrigger) && height_map[row, col] >= -grassTrigger) && waterEnabled)
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

        height_map = new float[height_map_size, height_map_size];

        mapLenght = height_map.GetLength(0);

        //  Random values at the corners of the square
        height_map[0, 0] = UnityEngine.Random.Range(-randomVal, randomVal);
        height_map[0, mapLenght - 1] = UnityEngine.Random.Range(-randomVal, randomVal);
        height_map[mapLenght - 1, 0] = UnityEngine.Random.Range(-randomVal, randomVal);
        height_map[mapLenght - 1, mapLenght - 1] = UnityEngine.Random.Range(-randomVal, randomVal);


        chunkSize = height_map_size - 1;
        randomAux = randomVal;
        while (chunkSize > 1)
        {
            DiamondSquareStep(height_map, chunkSize, randomVal);
            chunkSize /= 2;
            randomVal /= 2;
        }

        randomVal = randomAux;
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
}
