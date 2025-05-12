using UnityEngine;

public class VRModeToggle : MonoBehaviour
{
    public GameObject ovrObjects;
    public GameObject xrObjects;

    private bool isUsingOVR = true;

    void Start()
    {
        // 시작할 때 VR 오브젝트 활성화
        ovrObjects.SetActive(true);
        xrObjects.SetActive(false);
    }

    public void ToggleVR()
    {
        isUsingOVR = !isUsingOVR;

        ovrObjects.SetActive(isUsingOVR);
        xrObjects.SetActive(!isUsingOVR);
    }
}
