using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class test : MonoBehaviour
{
    //  Si dejamos el pivote hacia arriba y hacia abajo, eso nos permite generar m�s profundida, porque as� nos aseguramos
    //  de que las alturas tambi�n van hacia abajo
    //  Graphic representation of 2D matrix
    public GameObject cubePrefab; // Prefab del objeto visual de cada celda
    public float cubeSize = 1f; // Tama�o de cada cubo en el plano
    public float[,] height_map;
    public int n;
    public int randomVal;
    public int height_map_size;
    public int mapLenght;
    public int chunkSize;

    public Material earthMaterial;
    public Material waterMaterial;
    public Material grassMaterial;

    public float subground;
    public float grassTrigger;
    public float earthTrigger;


    int currentSceneIndex;

    void Start()
    {
        earthTrigger = 5f;
        grassTrigger = 2f;
        subground = 0f;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

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
        while (chunkSize > 1)
        {
            DiamondSquareStep(height_map, chunkSize, randomVal);
            chunkSize /= 2;
            randomVal /= 2;
        }

        BuildHeightMap(height_map);
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
                cube.transform.localScale = new Vector3(cubeSize, height_map[row, col], cubeSize);  //  Instancia del tama�o del cubo
                cube.transform.parent = transform;
                cube.name = height_map[row, col].ToString();

                if (height_map[row,col] >= earthTrigger)
                {
                cube.GetComponentInChildren<Renderer>().material = earthMaterial;
                }
                else if (height_map[row, col] > grassTrigger && height_map[row, col] < earthTrigger)
                {
                    cube.GetComponentInChildren<Renderer>().material = grassMaterial;
                }
                else
                {
                    cube.GetComponentInChildren<Renderer>().material = waterMaterial;
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
            //  Testing merge
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(currentSceneIndex);
        }
    }

}

