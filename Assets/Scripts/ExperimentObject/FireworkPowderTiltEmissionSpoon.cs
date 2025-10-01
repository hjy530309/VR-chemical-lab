using System.Diagnostics;
using UnityEngine;

public class FireworkPowderTiltEmissionSpoon : MonoBehaviour
{
    public ParticleSystem gunpowderParticles;
    public ParticleSystem sawdustpowderParticles;
    public Transform bottleTransform;  // 병 게임오브젝트 트랜스폼
    public GameObject spoon;

    public float tiltThreshold = 30f; // 몇 도 이상 기울이면 입자 나옴


void Start()
{
    gunpowderParticles.Stop();
    sawdustpowderParticles.Stop();
}

void Update()
{
    if (spoon == null) return;

    FireworkScoopManager scoopMgr = spoon.GetComponent<FireworkScoopManager>();
    if (scoopMgr == null) return;

    Vector3 localUp = bottleTransform.localRotation * Vector3.up;
    float dot = Vector3.Dot(localUp, Vector3.up);

    // gunpowder
    if (scoopMgr.isGunPowder && scoopMgr.gunpowder.activeSelf)
    {
        if(dot < Mathf.Cos(tiltThreshold * Mathf.Deg2Rad)) gunpowderParticles.Play();
    }

    // sawdustpowder
    if (scoopMgr.isSawdustPowder && scoopMgr.sawdustpowder.activeSelf)
    {
        if(dot < Mathf.Cos(tiltThreshold * Mathf.Deg2Rad)) sawdustpowderParticles.Play();
    }
}

}
