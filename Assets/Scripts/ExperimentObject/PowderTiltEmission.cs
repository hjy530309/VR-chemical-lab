using UnityEngine;

public class PowderTiltEmission : MonoBehaviour
{
    public ParticleSystem powderParticles;
    public Transform bottleTransform;  // 병 게임오브젝트 트랜스폼
    public GameObject spoon;
    private ParticleSystem.EmissionModule emission;

    public float tiltThreshold = 30f; // 몇 도 이상 기울이면 입자 나옴

    void Start()
    {
        emission = powderParticles.emission;
        emission.rateOverTime = 0f;  // 시작할 때 입자 없음
        powderParticles.Stop();
    }

    void Update()
    {
        if (spoon == null) return;
            SpoonManager spoonMgr = spoon.GetComponent<SpoonManager>();
        if (spoonMgr != null && spoonMgr.powder.activeSelf){
            powderParticles.Play();
            Vector3 localUp = bottleTransform.localRotation * Vector3.up;

            float dot = Vector3.Dot(localUp, Vector3.up);  // 1이면 위, -1이면 아래

            if (dot < Mathf.Cos(tiltThreshold * Mathf.Deg2Rad))
            {
                emission.rateOverTime = 500f;
            }
            else
            {
                emission.rateOverTime = 0f;
            }
        }
    }
}
