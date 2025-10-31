using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetPlant : MonoBehaviour
{
    public GameObject dirt_pile;
    public PlantGrow plantGrow;
    public SpawnSeed seed;
    public Slider growSlider;
    public Slider lightSlider;

    public void PlantProgressReset()
    {
        dirt_pile.SetActive(false);
        
        plantGrow.hasLight = false;
        plantGrow.hasSand = false;
        plantGrow.hasSeed = false;
        plantGrow.hasSoil = false;
        plantGrow.hasWater = false;

        DeskMaterialToggle deskMaterial = FindObjectOfType<DeskMaterialToggle>();
        deskMaterial.HideHiddenMaterial();

        seed.SpawnSeedObject();

        if (growSlider != null)
            growSlider.value = 0f;

        if (lightSlider != null)
            lightSlider.value = 0f;
    }
}
