using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackHolsterPosition : MonoBehaviour
{
    public Transform headset;
    public Vector3 backOffset = new Vector3(0, -0.2f, -0.3f); // 머리 뒤쪽 오프셋

    void Update()
    {
        if (headset != null)
        {
            // 머리 위치를 기준으로 등 뒤 위치 계산
            transform.position = headset.position +
                headset.forward * backOffset.z +
                headset.up * backOffset.y;

            // 머리 방향을 따라가지만 수직은 유지
            transform.rotation = Quaternion.Euler(0, headset.eulerAngles.y, 0);
        }
    }
}