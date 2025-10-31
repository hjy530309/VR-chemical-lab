using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskMaterialToggle : MonoBehaviour
{
    private MeshRenderer mr;
    private Material[] mats;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mats = mr.materials;

        Color color = new Color(35f/255f, 89f/255f, 116f/255f);
        // 두 번째 Material 색 변경
        mats[1].color = color;
        mr.materials = mats;
    }

    public void ShowHiddenMaterial(Color color)
    {
        // 두 번째 Material 색 변경
        mats[1].color = color;
        mr.materials = mats;
    }

    public void HideHiddenMaterial()
    {
        Color color = new Color(35f/255f, 89f/255f, 116f/255f);
        // 두 번째 Material 색 변경
        mats[1].color = color;
        mr.materials = mats;
    }
}
