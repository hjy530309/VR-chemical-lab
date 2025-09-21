using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermitSpoonManager : MonoBehaviour
{
    public GameObject powder;
    public bool hasPowder = false;

    void Start()
    {
        powder.SetActive(false);
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
