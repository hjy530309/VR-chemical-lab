using UnityEngine;
using UnityEngine.XR;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Canvas))]
public class AutoEventCamera : MonoBehaviour
{
    [Header("XR 카메라 설정")]
    [Tooltip("런타임에서 자동으로 카메라를 찾아 Event Camera로 설정")]
    public bool autoFindCamera = true;

    [Tooltip("XR 카메라를 우선적으로 찾도록 설정")]
    public bool prioritizeXRCamera = true;

    [Tooltip("특정 카메라를 지정하면 그 카메라를 Event Camera로 사용")]
    [HideInInspector] public Camera specificCamera;

    [Header("XR 설정")]
    [Tooltip("UI 상호작용 거리")]
    public float uiInteractionDistance = 100f;

    private Canvas worldCanvas;
    private Camera xrCamera;

    void Awake()
    {
        worldCanvas = GetComponent<Canvas>();
        SetupEventCamera();
        SetupXRCanvas();
    }

    void Update()
    {
        // XR 카메라가 이동할 수 있으므로 주기적으로 업데이트
        if (xrCamera != null && worldCanvas.worldCamera == xrCamera)
        {
            // XR 카메라의 위치에 따라 UI 거리 조정
            float distance = Vector3.Distance(transform.position, xrCamera.transform.position);
            if (distance > uiInteractionDistance)
            {
                // 필요시 UI 위치 조정 로직 추가
            }
        }
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
            xrCamera = specificCamera;
            return;
        }

        // 자동 찾기
        if (autoFindCamera)
        {
            Camera camToUse = FindBestCamera();

            if (camToUse != null)
            {
                worldCanvas.worldCamera = camToUse;
                xrCamera = camToUse;
                Debug.Log($"AutoEventCamera: {camToUse.name}을 Event Camera로 설정");
            }
            else
            {
                Debug.LogWarning("AutoEventCamera: 사용 가능한 카메라를 찾을 수 없습니다.");
            }
        }
    }

    /// <summary>
    /// 가장 적합한 카메라 찾기
    /// </summary>
    private Camera FindBestCamera()
    {
        // XR 카메라 우선 탐색
        if (prioritizeXRCamera)
        {
            Camera xrCam = FindXRCamera();
            if (xrCam != null) return xrCam;
        }

        // 메인 카메라
        Camera mainCam = Camera.main;
        if (mainCam != null) return mainCam;

        // 활성화된 카메라 중 찾기
        Camera[] allCams = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in allCams)
        {
            if (cam.enabled && cam.gameObject.activeInHierarchy)
                return cam;
        }

        return null;
    }

    /// <summary>
    /// XR 카메라 찾기
    /// </summary>
    private Camera FindXRCamera()
    {
        // XR Rig 내부의 카메라 찾기
        GameObject xrRig = GameObject.Find("XR Origin");
        if (xrRig == null) xrRig = GameObject.Find("XR Rig");
        if (xrRig == null) xrRig = GameObject.Find("Camera Offset");

        if (xrRig != null)
        {
            Camera cam = xrRig.GetComponentInChildren<Camera>();
            if (cam != null) return cam;
        }

        // "MainCamera" 태그로 XR 카메라 찾기
        GameObject mainCameraObj = GameObject.FindWithTag("MainCamera");
        if (mainCameraObj != null)
        {
            Camera cam = mainCameraObj.GetComponent<Camera>();
            if (cam != null) return cam;
        }

        // XR 카메라 컴포넌트로 찾기
        Camera[] allCams = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in allCams)
        {
            if (cam.enabled && cam.gameObject.activeInHierarchy)
            {
                // XR 관련 컴포넌트가 있는 카메라 우선
                if (cam.GetComponent<UnityEngine.XR.XRInputSubsystem>() != null ||
                    cam.gameObject.name.Contains("XR") ||
                    cam.gameObject.name.Contains("VR"))
                {
                    return cam;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// XR 환경에 맞는 Canvas 설정
    /// </summary>
    private void SetupXRCanvas()
    {
        if (worldCanvas == null) return;

        // XR 환경에서 더 나은 성능을 위한 설정
        worldCanvas.planeDistance = uiInteractionDistance;

        // Graphic Raycaster가 없으면 추가
        var raycaster = GetComponent<UnityEngine.UI.GraphicRaycaster>();
        if (raycaster == null)
        {
            raycaster = gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            raycaster.blockingObjects = UnityEngine.UI.GraphicRaycaster.BlockingObjects.None;
        }

        // XR 상호작용을 위한 콜라이더 자동 추가
        var collider = GetComponent<Collider>();
        if (collider == null)
        {
            // Canvas 크기에 맞는 BoxCollider 추가
            var rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.1f);
                boxCollider.isTrigger = true;
            }
        }
    }

    /// <summary>
    /// 카메라 강제 재설정 (런타임 중 카메라 변경 시 호출)
    /// </summary>
    public void ForceCameraUpdate()
    {
        xrCamera = null;
        SetupEventCamera();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            worldCanvas = GetComponent<Canvas>();
            // 에디터에서는 기본 카메라로 설정
            if (worldCanvas.renderMode == RenderMode.WorldSpace)
            {
                worldCanvas.worldCamera = null;
            }
        }
    }

    void OnDisable()
    {
        if (!Application.isPlaying)
        {
            specificCamera = null;
            if (worldCanvas != null)
                worldCanvas.worldCamera = null;
        }
    }
#endif
}
/*using UnityEngine;

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
*/