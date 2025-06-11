using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Y축 고정 회전 (선택적)
        Vector3 direction = transform.position - cam.transform.position;
        direction.y = 0f; // 위아래 회전 제한 (원하면 제거 가능)
        transform.rotation = Quaternion.LookRotation(direction);

        Debug.Log("Canvas 회전: " + transform.rotation.eulerAngles);
    }
}
