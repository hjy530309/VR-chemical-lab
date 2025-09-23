using UnityEngine;

public class BoundingBoxController : MonoBehaviour
{
    private BoxCollider box;

    void Awake()
    {
        box = GetComponent<BoxCollider>();
    }

    public bool IsInside(Vector3 pos)
    {
        // 월드 좌표 → 로컬 좌표로 변환
        Vector3 localPos = transform.InverseTransformPoint(pos);
        return box.bounds.Contains(pos);
    }
}
