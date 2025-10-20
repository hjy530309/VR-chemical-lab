using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class XRInputBlocker : MonoBehaviour
{
    public InputField inputField; // Legacy InputField
    //public TMP_InputField inputField;
    public InputActionAsset xrInputActions; // XR Device Simulator에 연결된 액션 에셋

    private bool isBlocked = false;

    void Update()
    {
        if (inputField != null && xrInputActions != null)
        {
            if (inputField.isFocused && !isBlocked)
            {
                xrInputActions.Disable();  // 전체 XR 움직임 차단
                isBlocked = true;
                Debug.Log("XR 움직임 비활성화됨");
            }
            else if (!inputField.isFocused && isBlocked)
            {
                xrInputActions.Enable();   // XR 움직임 다시 허용
                isBlocked = false;
                Debug.Log("XR 움직임 다시 허용됨");
            }
        }
    }
}
