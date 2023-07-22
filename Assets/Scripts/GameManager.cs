using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Net.Mime.MediaTypeNames;

public class GameManager : MonoBehaviour
{
    private TerrainGenerator terrainGeneratorScript;
    private bool canvasActive = false;

    private AudioSource mainMenuSound;

    public UnityEngine.UI.Slider randomRangeSlider;
    public UnityEngine.UI.Slider mapSizeSlider;

    public UnityEngine.UI.Slider earthRangeSlider;
    public UnityEngine.UI.Slider waterRangeSlider;
    public UnityEngine.UI.Slider grassRangeSlider;


    public UnityEngine.UI.Toggle automaticRangeToggle;
    public UnityEngine.UI.Toggle earthEnabledToggle;
    public UnityEngine.UI.Toggle waterEnabledToggle;
    public UnityEngine.UI.Toggle grassEnabledToggle;

    public GameObject panelUI;
    // Start is called before the first frame update
    void Start()
    {
        //  Audio
        mainMenuSound = GetComponent<AudioSource>();

        //  Setting UI to zero for hide
        panelUI.transform.localScale = Vector2.zero;
        terrainGeneratorScript = GameObject.Find("Terrain Data").GetComponent<TerrainGenerator>();

        //  sliders
        randomRangeSlider.onValueChanged.AddListener((value) => OnRandomRangeValueChange(value));
        mapSizeSlider.onValueChanged.AddListener((value) => OnMapSizeValueChange((int)value));

        earthRangeSlider.onValueChanged.AddListener((value) => OnEarthRangeValueChange(value));
        grassRangeSlider.onValueChanged.AddListener((value) => OnGrassRangeValueChange(value));
        waterRangeSlider.onValueChanged.AddListener((value) => OnWaterRangeValueChange(value));

        //  toggle buttons
        automaticRangeToggle.onValueChanged.AddListener(OnToggleAutomaticRangeValueChange);

        //  enable materials
        earthEnabledToggle.onValueChanged.AddListener(OnToggleEarthEnabledValueChange);
        grassEnabledToggle.onValueChanged.AddListener(OnToggleGrassEnabledValueChange);
        waterEnabledToggle.onValueChanged.AddListener(OnToggleWaterEnabledValueChange);
    }

    private void Update()
    {
        //  Animations UI
        if (Input.GetKeyDown(KeyCode.M))
        {
            mainMenuSound.Play();
            canvasActive = !canvasActive;
            if (canvasActive)
            {
                panelUI.LeanScale(new Vector2(0.3f,0.3f), 0.5f);

            }
            else
            {
                panelUI.LeanScale(Vector2.zero, 1f).setEaseInBack();
            }
        }

    }

    private void OnToggleEarthEnabledValueChange(bool value)
    {
        terrainGeneratorScript.m_earthEnabled = value;
    }

    private void OnToggleGrassEnabledValueChange(bool value)
    {
        terrainGeneratorScript.m_grassEnabled = value;
    }

    private void OnToggleWaterEnabledValueChange(bool value)
    {
        terrainGeneratorScript.m_waterEnabled = value;
    }


    private void OnToggleAutomaticRangeValueChange(bool value)
    {
        terrainGeneratorScript.m_automaticRange = value;
    }

    private void OnRandomRangeValueChange(float value)
    {
        terrainGeneratorScript.m_randomRange = value;
    }

    private void OnEarthRangeValueChange(float value)
    {
        terrainGeneratorScript.m_earthTriggerRange = value;
    }

    private void OnGrassRangeValueChange(float value)
    {
        terrainGeneratorScript.m_grassTriggerRange = value;
    }

    private void OnWaterRangeValueChange(float value)
    {
        terrainGeneratorScript.m_waterTriggerRange = value;
    }




    private void OnMapSizeValueChange(int value)
    {
        terrainGeneratorScript.m_n = value;
    }



    public void ReloadTerrain()
    {
        terrainGeneratorScript.ReloadTerrain();
    }
}
