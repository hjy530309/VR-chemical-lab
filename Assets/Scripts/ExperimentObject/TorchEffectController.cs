using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchEffectController : MonoBehaviour
{
    public GameObject fireEffect; // Inspector에서 FX 파티클 연결

    public void DisableEffect()
    {
        if (fireEffect != null)
            fireEffect.SetActive(false);
    }

    public void EnableEffect()
    {
        if (fireEffect != null)
            fireEffect.SetActive(true);
    }
}
