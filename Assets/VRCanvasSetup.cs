using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class VRCanvasSetup : MonoBehaviour
{
    void Start()
    {
        // Canvas를 World Space로 설정
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        // UI 요소들에 Collider 자동 추가
        AddCollidersToUI();
    }

    void AddCollidersToUI()
    {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.GetComponent<Collider>() == null)
            {
                BoxCollider collider = button.gameObject.AddComponent<BoxCollider>();
                RectTransform rect = button.GetComponent<RectTransform>();
                collider.size = new Vector3(rect.rect.width, rect.rect.height, 1f);
            }
        }
    }
}