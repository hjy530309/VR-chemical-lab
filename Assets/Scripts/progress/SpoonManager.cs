using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpoonManager : MonoBehaviour
{
    public GameObject liquid;
    public GameObject powder;
    public bool hasPowder = false;

    void Start()
    {
        powder.SetActive(false);
        liquid.SetActive(false);
    }

    public void ActiveLiquid()
    {
        liquid.SetActive(true);
        LiquidVolumeAnimator target = liquid.GetComponentInChildren<LiquidVolumeAnimator>();
        target.level = Mathf.MoveTowards(target.level, 1f, 0.5f * Time.deltaTime);
    }

    public void ActivePowder()
    {
        powder.SetActive(true);
        hasPowder = true;
    }

    public void InactivePowder()
    {
        powder.SetActive(false);
    }
}
