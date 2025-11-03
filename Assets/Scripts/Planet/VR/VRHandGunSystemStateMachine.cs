using UnityEngine;

public class VRHandGunSystemStateMachine : MonoBehaviour
{
    public enum WeaponState { Hand, Gun }

    [Header("Gun Prefab")]
    public GameObject gunPrefab;

    [Header("Reference")]
    public Transform referenceTransform;

    [Header("Gun Alignment")]
    public string alignmentPointName = "GripPoint"; // 총 안의 기준점 오브젝트 이름
    public Vector3 positionOffset = Vector3.zero;   // 추가 위치 오프셋
    public Vector3 rotationOffset = Vector3.zero;   // 추가 회전 오프셋

    [Header("Settings")]
    public float behindBackThreshold = -0.3f;
    public float cooldownTime = 1.0f;

    [Header("Debug")]
    public WeaponState currentState = WeaponState.Hand;
    public bool showDebug = true;

    private Transform controllerTransform;
    private bool wasBehindBack = false;
    private float lastActionTime = 0f;
    private GameObject currentGun;
    private Transform gunAlignmentPoint; // 총 안의 기준점

    void Start()
    {
        controllerTransform = transform;

        if (referenceTransform == null)
            referenceTransform = Camera.main.transform;

        SetState(WeaponState.Hand);
    }

    void Update()
    {
        HandleWeaponToggle();
        UpdateWeaponPosition();
    }

    void HandleWeaponToggle()
    {
        if (Time.time - lastActionTime < cooldownTime) return;

        Vector3 localHandPos = referenceTransform.InverseTransformPoint(controllerTransform.position);
        bool isBehindBack = localHandPos.z < behindBackThreshold;

        if (wasBehindBack && !isBehindBack)
        {
            ToggleWeapon();
            lastActionTime = Time.time;
        }

        wasBehindBack = isBehindBack;
    }

    void UpdateWeaponPosition()
    {
        if (currentState == WeaponState.Gun && currentGun != null && gunAlignmentPoint != null)
        {
            // 총의 기준점을 컨트롤러 위치에 맞추기
            Vector3 targetPosition = controllerTransform.position;
            Quaternion targetRotation = controllerTransform.rotation;

            // 총의 기준점이 컨트롤러 위치에 오도록 조정
            Vector3 offset = currentGun.transform.position - gunAlignmentPoint.position;
            currentGun.transform.position = targetPosition + offset;

            // 회전 적용
            currentGun.transform.rotation = targetRotation * Quaternion.Euler(rotationOffset);

            // 추가 위치 오프셋
            currentGun.transform.position += controllerTransform.TransformDirection(positionOffset);
        }
    }

    void ToggleWeapon()
    {
        if (currentState == WeaponState.Hand)
        {
            SetState(WeaponState.Gun);
        }
        else
        {
            SetState(WeaponState.Hand);
        }
    }

    void SetState(WeaponState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (newState)
        {
            case WeaponState.Hand:
                if (currentGun != null)
                {
                    Destroy(currentGun);
                    currentGun = null;
                    gunAlignmentPoint = null;
                }
                if (showDebug) Debug.Log("→ 손 상태");
                break;

            case WeaponState.Gun:
                if (gunPrefab != null && currentGun == null)
                {
                    currentGun = Instantiate(gunPrefab, controllerTransform.position, controllerTransform.rotation);

                    // 총 안의 기준점 찾기
                    FindGunAlignmentPoint();

                    if (showDebug)
                    {
                        if (gunAlignmentPoint != null)
                            Debug.Log($"총 기준점 찾음: {gunAlignmentPoint.name}");
                        else
                            Debug.LogWarning("총 기준점을 찾지 못함");
                    }
                }
                if (showDebug) Debug.Log("→ 총 상태");
                break;
        }
    }

    void FindGunAlignmentPoint()
    {
        if (currentGun == null) return;

        // 이름으로 기준점 찾기
        gunAlignmentPoint = FindDeepChild(currentGun.transform, alignmentPointName);

        // 못 찾으면 총의 자식 중 첫 번째 Transform 사용
        if (gunAlignmentPoint == null && currentGun.transform.childCount > 0)
        {
            gunAlignmentPoint = currentGun.transform.GetChild(0);
            if (showDebug) Debug.Log($"기본 기준점 사용: {gunAlignmentPoint.name}");
        }

        // 그래도 못 찾으면 총 자신을 기준점으로 사용
        if (gunAlignmentPoint == null)
        {
            gunAlignmentPoint = currentGun.transform;
            if (showDebug) Debug.Log("총 자신을 기준점으로 사용");
        }
    }

    // 자식 오브젝트 깊이 찾기
    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }

    [ContextMenu("수동 전환")]
    public void ManualToggle()
    {
        ToggleWeapon();
    }

    [ContextMenu("기준점 다시 찾기")]
    public void RealignGun()
    {
        if (currentGun != null)
        {
            FindGunAlignmentPoint();
            UpdateWeaponPosition();
        }
    }
}