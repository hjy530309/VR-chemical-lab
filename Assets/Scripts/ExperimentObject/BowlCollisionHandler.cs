using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BowlCollisionHandler : MonoBehaviour
{
    public ParticleSystem bowlChildParticle;
    public GameObject bowlRibbon;

    void OnParticleCollision(GameObject other)
    {
        // 불꽃 파티클이 이 오브젝트(Bowl)에 충돌했을 경우
        if (other.CompareTag("FireParticle"))
        {
            if (bowlChildParticle != null && !bowlChildParticle.isPlaying && bowlRibbon.activeSelf)
            {
                bowlChildParticle.gameObject.SetActive(true);
            }
        }
    }
}
