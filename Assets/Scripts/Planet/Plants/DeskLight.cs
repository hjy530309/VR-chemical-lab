using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DeskLight : MonoBehaviour
{
    public GameObject light;
    public Image backgroundImage;
    public Color backgroundColor = Color.blue;
    public PlantGrow plant;

    void Start()
    {
        if(backgroundImage != null)
        {
            backgroundImage.color = new Color(0.85f, 0.34f, 0.24f, 1f);
        }
        light.SetActive(false);
    }

    // Update is called once per frame
    public void showLight()
    {
        if(backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }
        light.SetActive(true);
        plant.hasLight = true;
    }
    public void hideLight()
    {
        if(backgroundImage != null)
        {
            backgroundImage.color = new Color(0.85f, 0.34f, 0.24f, 1f); // 색상 변경
        }
        light.SetActive(false);
        plant.hasLight = false;
    }
}
