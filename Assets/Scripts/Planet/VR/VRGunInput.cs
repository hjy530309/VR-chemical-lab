using UnityEngine;
using UnityEngine.XR;

public class VRGunInput : MonoBehaviour
{
    [Header("VR Controller")]
    public XRNode controllerNode = XRNode.RightHand;

    [Header("Gun Reference")]
    public GunShoot gunShoot;

    private InputDevice controllerDevice;
    private bool wasTriggerPressed = false;

    private void Start()
    {
        controllerDevice = InputDevices.GetDeviceAtXRNode(controllerNode);

        if (gunShoot == null)
        {
            gunShoot = GetComponent<GunShoot>();
        }

        // VR 모드로 설정
        if (gunShoot != null)
        {
            gunShoot.SetVRMode(true);
        }
    }

    private void Update()
    {
        HandleVRInput();
    }

    private void HandleVRInput()
    {
        if (controllerDevice.isValid && gunShoot != null)
        {
            // 트리거 입력 처리
            if (controllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed))
            {
                if (triggerPressed && !wasTriggerPressed)
                {
                    gunShoot.VRShoot();
                }
                wasTriggerPressed = triggerPressed;
            }

            // 그립 버튼으로 다른 기능 추가 가능
            if (controllerDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed))
            {
                // 그립 버튼 기능 여기서 구현
            }
        }
    }

    // 컨트롤러 진동 기능
    public void TriggerHapticFeedback(float amplitude = 0.5f, float duration = 0.1f)
    {
        if (controllerDevice.isValid && controllerDevice.TryGetHapticCapabilities(out var capabilities) && capabilities.supportsImpulse)
        {
            controllerDevice.SendHapticImpulse(0, amplitude, duration);
        }
    }
}