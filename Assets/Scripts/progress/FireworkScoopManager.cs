using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkScoopManager : MonoBehaviour
{
    public AudioSource spoonAudio;
    public AudioSource powderAudio1;
    public AudioSource powderAudio2;
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
        powderAudio1.Play();
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
        powderAudio2.Play();
        sawdustpowder.SetActive(true);
        hasPowder = true;
        isGunPowder = false;
        isSawdustPowder = true;
    }

    public void InactiveSawdustPowder()
    {
        sawdustpowder.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Floor 레이어에 부딪혔는지 확인
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            if (spoonAudio != null && !spoonAudio.isPlaying)
            {
                spoonAudio.Play();
            }
        }
    }
}
