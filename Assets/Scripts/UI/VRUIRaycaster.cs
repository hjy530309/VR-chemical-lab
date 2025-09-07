using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class VRUIRaycaster : MonoBehaviour
{
    [SerializeField]
    public Camera rayOriginCamera; // 마우스용이면 메인 카메라, VR이면 HMD 또는 컨트롤러 카메라
    public LayerMask uiLayerMask;  // UI Canvas가 있는 Layer

    private EventSystem eventSystem;

    [Header("Ray Visualization")]
    public bool showRay = true;
    public Color rayColor = Color.green;
    public float rayLength = 10f;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        eventSystem = EventSystem.current;

        // LineRenderer 추가
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = rayColor;
        lineRenderer.positionCount = 2;
    }


    private void Update()
{
    if (rayOriginCamera == null) return;

    Ray ray = new Ray(rayOriginCamera.transform.position, rayOriginCamera.transform.forward);

    // Ray 시각화
    if (showRay && lineRenderer != null)
    {
        lineRenderer.SetPosition(0, ray.origin);
        lineRenderer.SetPosition(1, ray.origin + ray.direction * rayLength);
    }

    if (Input.GetMouseButtonDown(0)) // VR 컨트롤러 버튼으로 바꿀 수도 있음
    {
        CheckUIHit(ray);
    }
}


    public void CheckUIHit(Ray ray)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);

        // 레이 끝점으로 화면 좌표 변환
        Vector3 screenPoint = rayOriginCamera.WorldToScreenPoint(ray.origin + ray.direction * 10f);
        pointerData.position = screenPoint;

        // 모든 Canvas에서 Raycast
        GraphicRaycaster[] canvases = FindObjectsOfType<GraphicRaycaster>();
        foreach (var raycaster in canvases)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            foreach (var result in results)
            {
                if (((1 << result.gameObject.layer) & uiLayerMask) == 0) continue;

                Button button = result.gameObject.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                    Debug.Log("VR UI Clicked: " + result.gameObject.name);
                    return;
                }
            }
        }
    }

}
