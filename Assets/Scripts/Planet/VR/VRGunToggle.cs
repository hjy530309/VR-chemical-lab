using UnityEngine;
using Fusion.XR.Host.Rig;

public class VRGunToggle : MonoBehaviour
{
    public HardwareHand rightHand;
    public GameObject gun;

    private bool gunVisible = false;
    private bool gripPressedLastFrame = false;

    void Update()
    {
        if (rightHand == null) return;

        float grip = rightHand.handCommand.gripCommand;

        // GripÀÌ ´­·È´Ù°¡ ¶¼Áú ¶§ Åä±Û
        if (grip > 0.8f && !gripPressedLastFrame)
        {
            gunVisible = !gunVisible;
            gun.SetActive(gunVisible);
        }

        gripPressedLastFrame = grip > 0.8f;
    }
}
