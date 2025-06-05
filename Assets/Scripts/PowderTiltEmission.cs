using UnityEngine;

public class PowderTiltEmission : MonoBehaviour
{
    public ParticleSystem powderParticles;
    public Transform bottleTransform;  // 병 게임오브젝트 트랜스폼

    private ParticleSystem.EmissionModule emission;

    public float tiltThreshold = 30f; // 몇 도 이상 기울이면 입자 나옴

    void Start()
    {
        emission = powderParticles.emission;
        emission.rateOverTime = 0f;  // 시작할 때 입자 없음
    }

    void Update()
    {
        // 병의 x축 기울기(앞뒤)
        float xAngle = bottleTransform.eulerAngles.x;

        // eulerAngles는 0~360 사이 값이라 360 넘으면 보정
        if (xAngle > 180f) xAngle -= 360f;

        if (Mathf.Abs(xAngle) >= tiltThreshold)
        {
            // 기울기 크면 입자 발생량 높이기
            emission.rateOverTime = 500f;
        }
        else
        {
            // 기울기 작으면 입자 없음
            emission.rateOverTime = 0f;
        }
    }
}
