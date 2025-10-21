using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShowGun : MonoBehaviour
{
    public GameObject gun;

    void Update()
    {
        // 마우스 오른쪽 버튼 (기본: Fire2)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            // 현재 상태 반전 (보이면 숨기고, 숨겨져 있으면 보이게)
            ToggleGun();
        }
    }

    void ToggleGun()
    {
        bool isActive = gun.activeSelf;
        gun.SetActive(!isActive);
    }
}