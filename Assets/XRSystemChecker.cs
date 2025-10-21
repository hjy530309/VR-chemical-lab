using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSystemChecker : MonoBehaviour
{
    void Start()
    {
        CheckXRSetup();
    }

    void CheckXRSetup()
    {
        Debug.Log("=== XR 시스템 진단 시작 ===");

        // 1. Interaction Manager 확인
        var managers = FindObjectsOfType<XRInteractionManager>();
        Debug.Log($"씬 내 Interaction Manager 개수: {managers.Length}");

        foreach (var manager in managers)
        {
            Debug.Log($"Manager: {manager.name}", manager);
        }

        // 2. 컨트롤러들 확인
        var controllers = FindObjectsOfType<XRController>();
        Debug.Log($"컨트롤러 개수: {controllers.Length}");

        foreach (var controller in controllers)
        {
            Debug.Log($"컨트롤러: {controller.name}", controller);
            Debug.Log($"  부모: {controller.transform.parent?.name}");
            Debug.Log($"  최종 부모: {controller.transform.root.name}");

            // 레이 인터랙터 확인
            var rayInteractor = controller.GetComponent<XRRayInteractor>();
            if (rayInteractor != null)
            {
                Debug.Log($"  레이 인터랙터: {rayInteractor.name}");
                Debug.Log($"  레이 오리진: {rayInteractor.rayOriginTransform?.name}");
                Debug.Log($"  레이 활성화: {rayInteractor.isActiveAndEnabled}");
            }
        }

        // 3. 카메라 확인
        var xrCamera = Camera.main;
        if (xrCamera != null)
        {
            Debug.Log($"메인 카메라: {xrCamera.name}", xrCamera);
            Debug.Log($"카메라 부모: {xrCamera.transform.parent?.name}");
        }

        Debug.Log("=== XR 시스템 진단 완료 ===");
    }

    [ContextMenu("실시간 상태 확인")]
    void CheckRealtimeStatus()
    {
        var controllers = FindObjectsOfType<XRController>();
        foreach (var controller in controllers)
        {
            var rayInteractor = controller.GetComponent<XRRayInteractor>();
            if (rayInteractor != null)
            {
                Debug.Log($"[실시간] {controller.name} 레이 오리진: {rayInteractor.rayOriginTransform?.name}");

                // 현재 레이 방향 확인
                Debug.DrawRay(controller.transform.position, controller.transform.forward * 5f, Color.red, 2f);
            }
        }
    }
}