using System;
using System.Collections;
using UnityEngine;


public class TerrainGenerator : MonoBehaviour
{

    [System.Serializable]
    public class MyTerrainData : MonoBehaviour
    {
        public int m_n;
        public int m_heightMapSize;
        public bool m_automaticRange;
        public float m_randomRange;
        public float m_auxRandomRange;

        public float m_earthTriggerRange;
        public float m_grassTriggerRange;
        public float m_waterTriggerRange;
        public Material m_earthMaterial;
        public Material m_waterMaterial;
        public Material m_grassMaterial;
        public bool m_grassEnabled;
        public bool m_earthEnabled;
        public bool m_waterEnabled;
        public int m_chunkSize;
        public float[,] m_heightMap;

        private int m_half;
        private float randomAux;


        //  methods
        public MyTerrainData()
        {
            m_n = UnityEngine.Random.Range(1, 9);
            m_earthTriggerRange = 0.75f;
            m_grassTriggerRange = 0.5f;
            m_waterTriggerRange = 0.2f;
            //  Dimensions of 2^n + 1
            m_heightMapSize = (int)(Mathf.Pow(2, m_n) + 1);
            m_heightMap = new float[m_heightMapSize, m_heightMapSize];

            m_chunkSize = m_heightMapSize - 1;
            m_automaticRange = true;
            m_waterEnabled = false;
            m_earthEnabled = false;
            m_grassEnabled = false;
            m_earthMaterial = Resources.Load<Material>("groundMaterial");
            m_grassMaterial = Resources.Load<Material>("grassMaterial");
            m_waterMaterial = Resources.Load<Material>("waterMaterial");


            if (m_automaticRange)
            {
                m_randomRange = SetAutomaticRange(m_n, m_randomRange);
            }

            SetRandomCorners(m_heightMap, m_randomRange, m_heightMapSize);
            SetMaterialsTriggerRange(m_earthTriggerRange, m_grassTriggerRange, m_waterTriggerRange, m_randomRange);
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

        void SetMaterialsTriggerRange(float earthTriggerRange, float grassTriggerRange, float waterTriggerRange, float randomRange)
        {
            m_earthTriggerRange = earthTriggerRange * randomRange;
            m_grassTriggerRange = grassTriggerRange * randomRange;
            m_waterTriggerRange = waterTriggerRange * randomRange;
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
                                        m_heightMap[x, (y - m_half + m_half) % m_heightMapSize]) / 4f;
                    float randomAdd = UnityEngine.Random.Range(-m_randomRange, m_randomRange);

                    m_heightMap[x, y] = avg + randomAdd;
                }
            }
            return m_heightMap;
        }
        private void SetMaterial(float[,] height_map, int row, int col, GameObject cube)
        {
            //  Water, mountain or grass?
            if ((height_map[row, col] >= m_earthTriggerRange || height_map[row, col] <= -m_earthTriggerRange) && m_earthEnabled)
            {
                cube.GetComponentInChildren<Renderer>().material = m_earthMaterial;
            }
            else if (((height_map[row, col] >= m_grassTriggerRange && height_map[row, col] < m_earthTriggerRange) || (height_map[row, col] <= -m_grassTriggerRange && height_map[row, col] > -m_earthTriggerRange)) && m_grassEnabled)
            {
                cube.GetComponentInChildren<Renderer>().material = m_grassMaterial;
            }
            else if (((height_map[row, col] < m_grassTriggerRange) && height_map[row, col] >= -m_grassTriggerRange) && m_waterEnabled)
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
    }


    //public GameObject parent;
    public GameObject cubePrefab;

    void Start()
    {
        MyTerrainData myTerrain = new();

        Debug.Log(myTerrain.m_n);
        Debug.Log(myTerrain.m_heightMapSize);


        while (myTerrain.m_chunkSize > 1)
        {
            myTerrain.DiamondSquareStep();
            myTerrain.m_chunkSize /= 2;
            myTerrain.m_randomRange /= 2;
        }
        myTerrain.m_randomRange = myTerrain.m_auxRandomRange;

        myTerrain.BuildHeightmap(myTerrain.m_heightMap, cubePrefab);
    }

}


//  Declaracion de la corrutina
/*  void ReloadingTerrain()
  {
      DeleteChilds();


      InstantiateTerrain();

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
}
*/
