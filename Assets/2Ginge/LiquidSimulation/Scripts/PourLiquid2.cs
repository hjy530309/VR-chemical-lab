using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PourLiquid2 : MonoBehaviour
{
    public float rateOfFlow = 1.0f;
    public ParticleSystem pouringParticleSystem;
    public float volumeOfParticles = 70.0f;

    // 파티클 강도를 제어하는 값 (0 = 꺼짐, 1 = 최대로 흘러나옴)
    private float particleVal = 0.0f;

    void Update()
    {
        if (pouringParticleSystem != null)
        {
            // ⚡️ 예시: 오브젝트가 기울어진 정도에 따라 particleVal 변경
            float d = Vector3.Dot(transform.up, Vector3.up); 
            // d = 1(세워짐), 0(옆으로), -1(거꾸로)
            particleVal = Mathf.Pow(Mathf.Clamp01(1 - d), 2); // 제곱해서 초반에는 약하게 시작


            // 파티클 강도 조정
            ParticleSystem.EmissionModule emod = pouringParticleSystem.emission;
            emod.rateOverTime = volumeOfParticles * particleVal;

            // 실행/정지
            if (particleVal > 0)
            {
                if (!pouringParticleSystem.isEmitting)
                {
                    pouringParticleSystem.Play();
                }
            }
            else
            {
                pouringParticleSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}
