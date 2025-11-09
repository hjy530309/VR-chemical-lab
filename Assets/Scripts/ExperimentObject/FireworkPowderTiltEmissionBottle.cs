using System.Diagnostics;
using UnityEngine;

public class FireworkPowderTiltEmissionBottle : MonoBehaviour
{
    public AudioSource Audio;
    public ParticleSystem Particles;
    public Transform bottleTransform;  // 병 게임오브젝트 트랜스폼
    public GameObject spoon;

    public float tiltThreshold = 30f; // 몇 도 이상 기울이면 입자 나옴


    void Start()
    {
        Particles.Stop();
    }

    void Update()
    {
        if (spoon == null) return;

        Vector3 localUp = bottleTransform.localRotation * Vector3.up;
        float dot = Vector3.Dot(localUp, Vector3.up);

        if (dot < Mathf.Cos(tiltThreshold * Mathf.Deg2Rad))
        {
            Particles.Play();
            if (!Audio.isPlaying)
                Audio.Play();
        }
        else
        {
            Particles.Stop();
            if (Audio.isPlaying)
                Audio.Stop();
        }
    }
}
