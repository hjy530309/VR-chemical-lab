using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRUIRayController : MonoBehaviour
{
    [Header("XR 레이 인터랙션")]
    public XRRayInteractor rayInteractor;
    public LayerMask uiLayerMask = 1 << 5; // UI 레이어 (5번)

    void Start()
    {
        SetupRayInteractor();
    }

    void SetupRayInteractor()
    {
        if (rayInteractor != null)
        {
            // UI 상호작용을 위한 설정
            rayInteractor.interactionLayers = InteractionLayerMask.GetMask("UI");
            rayInteractor.raycastMask = uiLayerMask;

            // 레이 시각화 설정
            rayInteractor.lineType = XRRayInteractor.LineType.StraightLine;
            rayInteractor.maxRaycastDistance = 10f;

            // 컨트롤러를 레이 오리진으로 설정
            rayInteractor.rayOriginTransform = this.transform;
        }
        else
        {
            // 자동으로 XRRayInteractor 찾기
            rayInteractor = GetComponent<XRRayInteractor>();
            if (rayInteractor == null)
            {
                Debug.LogWarning("XRRayInteractor를 찾을 수 없습니다.");
            }
        }
    }

    void Update()
    {
        // 컨트롤러의 위치와 회전을 레이 인터랙터에 실시간 동기화
        if (rayInteractor != null)
        {
            rayInteractor.transform.position = transform.position;
            rayInteractor.transform.rotation = transform.rotation;
        }
    }
}