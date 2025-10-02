using UnityEngine;

public class PhysicalButton : MonoBehaviour
{
    public GameObject targetPanel; // 버튼 누르면 켤 UI 패널

    private void OnMouseDown()  // PC 마우스용
    {
        Debug.Log(name + " 클릭됨");
        if (targetPanel != null)
            targetPanel.SetActive(true);
    }

    // VR 컨트롤러용: 충돌 감지
    public void Press()
    {
        Debug.Log(name + " VR로 클릭됨");
        if (targetPanel != null)
            targetPanel.SetActive(true);
    }
}
