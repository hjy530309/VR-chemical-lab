using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Transform targetCamera1;  // 첫 번째 카메라
    public Transform targetCamera2;  // 두 번째 카메라
    public GameObject objectA;       // A 오브젝트

    private Transform currentCamera; // 현재 활성화된 카메라
    private bool isObjectAActive;    // objectA의 활성화 상태를 추적

    void Start()
    {
        // 초기 카메라는 objectA의 활성화 상태에 따라 결정
        isObjectAActive = objectA.activeSelf;
        SwitchCameraBasedOnObject();
    }

    void Update()
    {
        // objectA의 활성화 상태가 변경되었을 때만 카메라를 전환
        if (objectA.activeSelf != isObjectAActive)
        {
            isObjectAActive = objectA.activeSelf;
            SwitchCameraBasedOnObject();
        }

        // 카메라 방향 업데이트
        if (currentCamera != null)
        {
            Vector3 direction = currentCamera.position - transform.position;
            direction.y = 0f; // y축 회전만 유지 (기울지 않게)
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void SwitchCameraBasedOnObject()
    {
        if (objectA.activeSelf)  // objectA가 활성화된 상태라면
        {
            currentCamera = targetCamera1;
        }
        else  // objectA가 비활성화된 상태라면
        {
            currentCamera = targetCamera2;
        }
    }
}