using System;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class test : MonoBehaviour
{
    //  Graphic representation of 2D matrix
    public GameObject cubePrefab; // Prefab del objeto visual de cada celda
    public float cellSize = 1f; // Tamaño de cada celda en el plano

    public int n;
    void Start()
    {
        n = UnityEngine.Random.Range(1, 7);
        //  Initial variables


        //  Dimensions of 2^n + 1
        int height_map_size = (int)(Mathf.Pow(2, n) + 1);

        int[,] height_map = new int[height_map_size, height_map_size];

        int mapLenght = height_map.GetLength(0);

        //  Random values at the corners of the square
        height_map[0, 0] = UnityEngine.Random.Range(1, 10);
        height_map[0, mapLenght - 1] = UnityEngine.Random.Range(1, 10);
        height_map[mapLenght - 1, 0] = UnityEngine.Random.Range(1, 10);
        height_map[mapLenght - 1, mapLenght - 1] = UnityEngine.Random.Range(1, 10);

        int chunkSize = height_map_size - 1;
        int[] heightRangeArray = { -2, -1, 0, 1, 2 };
        while (chunkSize > 1)
        {
            DiamondSquareStep(height_map, chunkSize, heightRangeArray);
            chunkSize /= 2;
            for (int i = 0; i < heightRangeArray.Length; i++)
            {
                heightRangeArray[i] /= 2;
            }
        }

        BuildHeightMap(height_map);
    }

    void BuildHeightMap(int[,] height_map)
    {

        int rows = height_map.GetLength(0);
        int columns = height_map.GetLength(1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float value = height_map[row, col];
                Vector3 position = new Vector3(col * cellSize, 0f, row * cellSize);

                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity);
                cube.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                cube.GetComponent<Renderer>().material.color = Color.Lerp(Color.black, Color.white, value / 10);
                cube.transform.parent = transform;
                cube.name = height_map[row, col].ToString();
            }
        }
    }
    void DiamondSquareStep(int[,] heightMap, int chunkSize, int[] heightRangeArray)
    {
        int half = chunkSize / 2;
        int mapSize = heightMap.GetLength(0);
        //  Diamond Step
        for (int y = half; y < heightMap.GetLength(0) - 1; y += chunkSize)
        {
            for (int x = half; x < heightMap.GetLength(1) - 1; x += chunkSize)
            {
                int index = UnityEngine.Random.Range(0, heightRangeArray.Length);

                float avg = (heightMap[x - half, y - half] +
                                    heightMap[x + half, y - half] +
                                    heightMap[x - half, y + half] +
                                    heightMap[x + half, y + half]
                                    ) / 4f;
                int randomAdd = heightRangeArray[index];

                heightMap[x, y] = (int)Mathf.Round(avg + randomAdd);
            }
        }

        //  Square step
        for (int y = 0; y < heightMap.GetLength(0) - 1; y += half)
        {
            for (int x = (y + half) % chunkSize; x < heightMap.GetLength(1) - 1; x += chunkSize)
            {
                int index = UnityEngine.Random.Range(0, heightRangeArray.Length);
                float avg = (heightMap[(x - half + mapSize) % mapSize, y] +
                                    heightMap[(x + half) % mapSize, y] +
                                    heightMap[x, (y + half) % mapSize] +
                                    heightMap[x, (y - half + mapSize) % mapSize]) / 4f;
                float randomAdd = heightRangeArray[index];

                heightMap[x, y] = (int)Mathf.Ceil(avg + randomAdd);
            }
        }
    }
}

