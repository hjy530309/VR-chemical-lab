using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject canvasA;  // 현재 보이는 캔버스
    public GameObject canvasB;  // 전환할 캔버스

    public void SwitchCanvas()
    {
        canvasA.SetActive(false); // 현재 캔버스 끄기
        canvasB.SetActive(true);  // 다른 캔버스 켜기
    }
}
