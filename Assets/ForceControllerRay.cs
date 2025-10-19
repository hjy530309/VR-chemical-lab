using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ForceControllerRay : MonoBehaviour
{
    [Header("References")]
    public XRRayInteractor rayInteractor;

    [Header("Settings")]
    public bool forceControllerOrigin = true;
    public LayerMask uiLayerMask = 1 << 5; // UI 레이어

    void Start()
    {
        ForceSetup();
    }

    void ForceSetup()
    {
        // 컴포넌트 찾기
        if (rayInteractor == null)
            rayInteractor = GetComponent<XRRayInteractor>();

        if (rayInteractor == null)
        {
            Debug.LogError("XRRayInteractor를 찾을 수 없습니다!");
            return;
        }

        // 1. 레이 오리진 강제 설정
        if (forceControllerOrigin)
        {
            rayInteractor.rayOriginTransform = this.transform;
        }

        // 2. 인터랙션 레이어 설정
        rayInteractor.interactionLayers = InteractionLayerMask.GetMask("UI");
        rayInteractor.raycastMask = uiLayerMask;

        // 3. 레이 타입 설정
        rayInteractor.lineType = XRRayInteractor.LineType.StraightLine;
        rayInteractor.maxRaycastDistance = 10f;

        // 4. 부모 관계 명확히 설정
        rayInteractor.transform.SetParent(this.transform);
        rayInteractor.transform.localPosition = Vector3.zero;
        rayInteractor.transform.localRotation = Quaternion.identity;

        // 5. 모든 충돌체 감지 모드 (테스트용)
        rayInteractor.raycastTriggerInteraction = QueryTriggerInteraction.Collide;

        Debug.Log($"레이 설정 완료: {gameObject.name}");
        Debug.Log($"레이 오리진: {rayInteractor.rayOriginTransform.name}");
    }

    void Update()
    {
        // 지속적으로 오리진 확인 (다른 시스템이 변경할 수 있음)
        if (forceControllerOrigin && rayInteractor.rayOriginTransform != this.transform)
        {
            Debug.LogWarning($"레이 오리진이 변경됨! 재설정: {rayInteractor.rayOriginTransform.name} -> {this.transform.name}");
            rayInteractor.rayOriginTransform = this.transform;
        }
    }

    // 디버그 정보 표시
    void OnDrawGizmos()
    {
        if (Application.isPlaying && rayInteractor != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 5f);
        }
    }
}