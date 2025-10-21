using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICanvasSetup : MonoBehaviour
{
    void Start()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            // World Space로 설정
            canvas.renderMode = RenderMode.WorldSpace;

            // XR 카메라 할당 (AutoEventCamera 스크립트가 있다면 자동)
            canvas.worldCamera = Camera.main;

            // Graphic Raycaster 추가 (UI 입력 처리)
            var raycaster = GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = gameObject.AddComponent<GraphicRaycaster>();
            }

            // Physics Raycaster도 필요할 수 있음
            var physicsRaycaster = FindObjectOfType<PhysicsRaycaster>();
            if (physicsRaycaster == null)
            {
                var xrCamera = Camera.main;
                if (xrCamera != null)
                {
                    physicsRaycaster = xrCamera.gameObject.AddComponent<PhysicsRaycaster>();
                }
            }
        }
    }
}