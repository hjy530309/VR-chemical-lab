using Fusion.XR.Host.Rig;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RayToggleOnTrigger : MonoBehaviour
{
    [Header("XR Ray Interactor")]
    public XRRayInteractor rayInteractor;           // 항상 활성화
    public XRInteractorLineVisual lineVisual;      // Line Renderer 시각화

    [Header("Hand Command")]
    public HardwareHand hardwareHand;              // Trigger 입력 읽기용

    void Update()
    {
        if (hardwareHand != null && lineVisual != null)
        {
            // Trigger 값 읽기
            bool triggerPressed = hardwareHand.handCommand.triggerCommand > 0.1f;

            // LineRenderer만 켜고 끄기
            lineVisual.enabled = triggerPressed;
        }
    }
}
