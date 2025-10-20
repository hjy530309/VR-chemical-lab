using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Fusion.XR.Host.Rig;

public class LocalControllerRay : MonoBehaviour
{
    public HardwareHand hand; // 연결된 HardwareHand
    public XRRayInteractor rayInteractor; // XR Ray Interactor

    private void Awake()
    {
        if (!hand) hand = GetComponent<HardwareHand>();
        if (!rayInteractor) rayInteractor = GetComponent<XRRayInteractor>();
    }

    private void Update()
    {
        // ray origin을 컨트롤러 위치와 회전으로 맞춤
        rayInteractor.transform.position = hand.transform.position;
        rayInteractor.transform.rotation = hand.transform.rotation;
    }
}
