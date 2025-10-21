using UnityEngine;
using Fusion;
using UnityEngine.XR; // XR 장치 접근용 (필요시)
using UnityEngine.XR.Interaction.Toolkit; // XR Toolkit 지원용

public class VRGunAim : NetworkBehaviour
{
    [Header("VR 컨트롤러 Transform (오른손)")]
    public Transform rightHandTransform;

    [Header("조준 거리 (총이 바라볼 지점까지의 거리)")]
    public float aimDistance = 10f;

    [Header("총 회전 부드러움 (0~1)")]
    [Range(0f, 1f)]
    public float smoothness = 0.5f;

    private void Update()
    {
        // Fusion: 내 캐릭터만 처리
        if (!Object || !Object.HasInputAuthority)
            return;

        if (rightHandTransform == null)
            return;

        // 컨트롤러가 향하고 있는 방향
        Vector3 aimDirection = rightHandTransform.forward;

        // 일정 거리 앞의 목표 위치
        Vector3 targetPos = rightHandTransform.position + aimDirection * aimDistance;

        // 현재 총의 방향을 목표로 부드럽게 회전
        Quaternion targetRot = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, smoothness);
    }
}
