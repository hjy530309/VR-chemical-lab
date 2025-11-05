using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermitSpoonManager : MonoBehaviour
{
    public AudioSource spoonAudio;
    public AudioSource powderAudio;
    public GameObject powder;
    public bool hasPowder = false;

    void Start()
    {
        powder.SetActive(false);
    }

    public void ActivePowder()
    {
        powderAudio.Play();
        powder.SetActive(true);
        hasPowder = true;
    }

    public void InactivePowder()
    {
        powder.SetActive(false);
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
