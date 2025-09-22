using UnityEngine;

public class MagnetEffect : MonoBehaviour
{
    public Transform magnet;            // 자석 오브젝트
    public Transform target1;         // 자석에 끌릴 오브젝트들
    public Transform target2;
    public Transform target3;
    public float magnetRange = 2f;      // 끌리는 거리
    public float magnetSpeed = 5f;      // 끌리는 속도

    void Update()
    {
        if (magnet == null) return;

        float distance = Vector3.Distance(target1.position, magnet.position);
        if (distance <= magnetRange)
        {
            target1.position = Vector3.Lerp(target1.position, magnet.position, Time.deltaTime * magnetSpeed);
        }

       distance = Vector3.Distance(target2.position, magnet.position);
        if (distance <= magnetRange)
        {
            target2.position = Vector3.Lerp(target2.position, magnet.position, Time.deltaTime * magnetSpeed);
        }

        distance = Vector3.Distance(target3.position, magnet.position);
        if (distance <= magnetRange)
        {
            target3.position = Vector3.Lerp(target3.position, magnet.position, Time.deltaTime * magnetSpeed);
        }

    }
}
