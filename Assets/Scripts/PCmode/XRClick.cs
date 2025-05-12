using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class XRClick : MonoBehaviour
{
    void Update()
    {
        // 좌클릭 입력 감지
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("좌클릭 감지됨 (XR Device Simulator 포함)");
        }
    }
}
