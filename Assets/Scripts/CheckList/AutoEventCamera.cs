using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Canvas))]
public class AutoEventCamera : MonoBehaviour
{
    [Header("카메라 설정")]
    [Tooltip("런타임에서 자동으로 카메라를 찾아 Event Camera로 설정")]
    public bool autoFindCamera = true;

    [Tooltip("특정 카메라를 지정하면 그 카메라를 Event Camera로 사용")]
    [HideInInspector] public Camera specificCamera; // 런타임 전용, 에디터 저장 X

    private Canvas worldCanvas;

    void Awake()
    {
        worldCanvas = GetComponent<Canvas>();
        SetupEventCamera();
    }

    /// <summary>
    /// Event Camera 설정
    /// </summary>
    public void SetupEventCamera()
    {
        if (worldCanvas == null || worldCanvas.renderMode != RenderMode.WorldSpace) return;

        // specificCamera가 있으면 우선 사용
        if (specificCamera != null)
        {
            worldCanvas.worldCamera = specificCamera;
            return;
        }

        // 자동 찾기
        if (autoFindCamera)
        {
            Camera camToUse = Camera.main;

            if (camToUse == null)
            {
                Camera[] allCams = GameObject.FindObjectsOfType<Camera>();
                if (allCams.Length > 0)
                    camToUse = allCams[0];
            }

            if (camToUse != null)
                worldCanvas.worldCamera = camToUse;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            worldCanvas = GetComponent<Canvas>();
            worldCanvas.worldCamera = null; // 에디터에서 초기화
        }
    }

    void OnDisable()
    {
        if (!Application.isPlaying)
        {
            specificCamera = null;
            if (worldCanvas != null)
                worldCanvas.worldCamera = null; // Play 종료 후 초기화
        }
    }
#endif
}
