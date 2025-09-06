using UnityEngine;

public class AutoEventCamera : MonoBehaviour
{
    [Header("카메라 설정")]
    public bool autoFindCamera = true;
    public Camera specificCamera;

    void Start()
    {
        SetupEventCamera();
    }

    void SetupEventCamera()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null || canvas.renderMode != RenderMode.WorldSpace) return;

        // 1. 특정 카메라가 지정되어 있으면 사용
        if (specificCamera != null)
        {
            canvas.worldCamera = specificCamera;
            return;
        }

        // 2. 자동으로 카메라 찾기
        if (autoFindCamera)
        {
            FindAndSetCamera(canvas);
        }
    }

    void FindAndSetCamera(Canvas canvas)
    {
        // Main Camera 먼저 시도
        if (Camera.main != null)
        {
            canvas.worldCamera = Camera.main;
            return;
        }

        // 모든 카메라 중에서 태그가 MainCamera인 것 찾기
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in cameras)
        {
            if (cam.CompareTag("MainCamera"))
            {
                canvas.worldCamera = cam;
                return;
            }
        }

        // 아무 카메라나 사용
        if (cameras.Length > 0)
        {
            canvas.worldCamera = cameras[0];
        }
    }

    // 에디터에서 실시간으로 확인
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            SetupEventCamera();
        }
    }
}