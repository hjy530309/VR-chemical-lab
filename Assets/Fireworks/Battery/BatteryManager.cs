using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryManager : MonoBehaviour
{
    public ParticleSystem fireParticle1;
    public ParticleSystem fireParticle2;

    private bool isRunning = false;

    void Start()
    {
        StopRocketBattery();
    }

    public void StartRocketBattery()
    {
        // 파티클 시작과 동시에 소리 재생
        PlayParticleWithSound(fireParticle1);
        if(fireParticle2 != null)
            PlayParticleWithSound(fireParticle2);
    }

    public void StopRocketBattery()
    {
        fireParticle1.Stop();
        if(fireParticle2 != null)
            fireParticle2.Stop();
    }

    void PlayParticleWithSound(ParticleSystem ps)
    {
        ps.Play();
    }
}