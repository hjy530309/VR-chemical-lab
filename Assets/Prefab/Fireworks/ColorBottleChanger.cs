using UnityEngine;

public class ColorBottleChanger : MonoBehaviour
{
    public Renderer liquidRenderer;

    // 빨, 주, 노, 초, 파, 남, 보
    private Color[] rainbowColors = new Color[]
    {
        Color.red,
        new Color(1f, 0.5f, 0f), // 주황
        Color.yellow,
        Color.green,
        Color.blue,
        new Color(0f, 0f, 0.5f), // 남색
        new Color(0.5f, 0f, 0.5f) // 보라
    };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeColor(0); // 빨강
        if (Input.GetKeyDown(KeyCode.Alpha5))
            ChangeColor(1); // 주황
        if (Input.GetKeyDown(KeyCode.Alpha6))
            ChangeColor(2); // 노랑
        if (Input.GetKeyDown(KeyCode.Alpha7))
            ChangeColor(3); // 초록
        if (Input.GetKeyDown(KeyCode.Alpha8))
            ChangeColor(4); // 파랑
        if (Input.GetKeyDown(KeyCode.Alpha9))
            ChangeColor(5); // 남색
        if (Input.GetKeyDown(KeyCode.Alpha0))
            ChangeColor(6); // 보라
    }

    void ChangeColor(int index)
    {
        liquidRenderer.material.color = rainbowColors[index];
    }
}
