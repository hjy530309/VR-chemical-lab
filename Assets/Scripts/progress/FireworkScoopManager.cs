using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkScoopManager : MonoBehaviour
{
    public GameObject gunpowder;
    public GameObject sawdustpowder;
    public bool hasPowder = false;
    public bool isGunPowder = false;
    public bool isSawdustPowder = false;

    void Start()
    {
        InactiveGunPowder();
        InactiveSawdustPowder();
    }

    public void ActiveGunPowder()
    {
        InactiveSawdustPowder();
        gunpowder.SetActive(true);
        hasPowder = true;
        isGunPowder = true;
        isSawdustPowder = false;
    }

    public void InactiveGunPowder()
    {
        gunpowder.SetActive(false);
    }

    public void ActiveSawdustPowder()
    {
        InactiveGunPowder();
        sawdustpowder.SetActive(true);
        hasPowder = true;
        isGunPowder = false;
        isSawdustPowder = true;
    }

    public void InactiveSawdustPowder()
    {
        sawdustpowder.SetActive(false);
    }
}
