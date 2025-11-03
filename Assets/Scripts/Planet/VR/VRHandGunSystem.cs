using UnityEngine;
using UnityEngine.XR;

public class VRHandGunSystem : MonoBehaviour
{
    [Header("VR Hand References")]
    public Transform handTransform;  // VR 컨트롤러 트랜스폼
    public GameObject handVisual;    // 손 메시

    [Header("Gun System")]
    public GameObject gunPrefab;

    [Header("Settings")]
    public float behindBackThreshold = -0.3f;
    public float cooldownTime = 1.0f;

    [Header("Reference Point - 중요!")]
    public Transform referenceTransform; // 기준점 (플레이어 몸통이나 카메라)

    [Header("Debug")]
    public bool showDebug = true;

    private bool isBehindBack = false;
    private float lastToggleTime = 0f;
    private GameObject currentGun;

    private void Start()
    {
        // referenceTransform이 설정되지 않았으면 카메라를 기준으로 사용
        if (referenceTransform == null)
        {
            referenceTransform = Camera.main.transform;
            if (showDebug)
            {
                Debug.Log("기준점 자동 설정: " + referenceTransform.name);
            }
        }

        if (showDebug)
        {
            Debug.Log("VRHandGunSystem 시작");
            Debug.Log("손 트랜스폼: " + handTransform.name);
            Debug.Log("기준 트랜스폼: " + referenceTransform.name);
        }
    }

    private void Update()
    {
        CheckHandPosition();
        HandleHandToGunTransition();
    }

    private void CheckHandPosition()
    {
        if (Time.time - lastToggleTime > cooldownTime)
        {
            // 기준점(카메라/몸통)을 기준으로 손의 로컬 위치 계산
            Vector3 localHandPos = referenceTransform.InverseTransformPoint(handTransform.position);
            bool wasBehindBack = isBehindBack;
            isBehindBack = localHandPos.z < behindBackThreshold;

            if (showDebug)
            {
                Debug.Log($"손 위치 - 월드: {handTransform.position}, 로컬: {localHandPos}, Z값: {localHandPos.z:F3}, 등 뒤: {isBehindBack}");
            }

            if (wasBehindBack != isBehindBack)
            {
                if (showDebug)
                {
                    Debug.Log($"등 뒤 상태 변경: {wasBehindBack} -> {isBehindBack}");
                }
                lastToggleTime = Time.time;
            }
        }
    }

    private void HandleHandToGunTransition()
    {
        if (isBehindBack && handVisual.activeSelf)
        {
            if (showDebug)
            {
                Debug.Log("등 뒤 감지 - 손 숨기고 총 생성");
            }
            HideHandAndSpawnGun();
        }
        else if (!isBehindBack && !handVisual.activeSelf && currentGun != null)
        {
            if (showDebug)
            {
                Debug.Log("등 앞 감지 - 총 제거하고 손 보이기");
            }
            ShowHandAndDespawnGun();
        }
    }

    private void HideHandAndSpawnGun()
    {
        // 손 비활성화
        handVisual.SetActive(false);

        if (gunPrefab != null)
        {
            // 총 생성
            currentGun = Instantiate(gunPrefab, handTransform.position, handTransform.rotation);
            currentGun.transform.SetParent(handTransform);

            if (showDebug)
            {
                Debug.Log("총 생성 완료: " + currentGun.name);
            }
        }
        else
        {
            Debug.LogError("총 프리팹이 설정되지 않았습니다!");
        }
    }

    private void ShowHandAndDespawnGun()
    {
        // 손 활성화
        handVisual.SetActive(true);

        // 총 제거
        if (currentGun != null)
        {
            if (showDebug)
            {
                Debug.Log("총 제거: " + currentGun.name);
            }
            Destroy(currentGun);
            currentGun = null;
        }
    }

    // Gizmo로 디버그 시각화
    private void OnDrawGizmos()
    {
        if (showDebug && referenceTransform != null && handTransform != null)
        {
            // 기준점 위치
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(referenceTransform.position, 0.1f);

            // 손 위치
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(handTransform.position, 0.05f);

            // 등 뒤 영역 시각화
            Gizmos.color = Color.red;
            Vector3 behindPoint = referenceTransform.position + referenceTransform.forward * behindBackThreshold;
            Gizmos.DrawWireSphere(behindPoint, 0.08f);

            // 손과 기준점 연결선
            Gizmos.color = Color.green;
            Gizmos.DrawLine(referenceTransform.position, handTransform.position);
        }
    }
}