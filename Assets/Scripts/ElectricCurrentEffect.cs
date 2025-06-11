using UnityEngine;

public class ElectricCurrentEffect : MonoBehaviour
{
    public Transform startPoint;         // 전원 장치 위치
    public Transform endPoint;           // 금속 오브젝트 위치
    public LineRenderer lineRenderer;    // 라인 렌더러
    public Light glowLight;              // 빛 효과

    public float pulseSpeed = 2f;        // 라인 알파 애니메이션 속도
    public Gradient pulseGradient;       // 전류 색상

    private float pulseTime = 0f;

    private void Start()
    {
        lineRenderer.positionCount = 2;
        DisableEffect(); // 처음엔 비활성화
    }

    private void Update()
    {
        if (lineRenderer.enabled)
        {
            // 전류 위치 업데이트
            lineRenderer.SetPosition(0, startPoint.position);
            lineRenderer.SetPosition(1, endPoint.position);

            // 알파 값 펄싱 (선이 흐르는 느낌)
            pulseTime += Time.deltaTime * pulseSpeed;
            float t = Mathf.PingPong(pulseTime, 1f);
            lineRenderer.colorGradient = pulseGradient;

            if (glowLight != null)
            {
                glowLight.transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);
                glowLight.intensity = Mathf.Lerp(0.5f, 2f, Mathf.Sin(Time.time * 10f) * 0.5f + 0.5f);
            }
        }
    }

    public void EnableEffect()
    {
        lineRenderer.enabled = true;
        if (glowLight != null) glowLight.enabled = true;
    }

    public void DisableEffect()
    {
        lineRenderer.enabled = false;
        if (glowLight != null) glowLight.enabled = false;
    }
}
