using UnityEngine;
using UnityEngine.UI; // Slider 사용

public class ColorBottleChanger : MonoBehaviour
{
    public Renderer liquidRenderer;
    public Slider colorSlider; // UI Slider 연결

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

    void Start()
    {
        // 슬라이더 최대/최소 설정
        colorSlider.minValue = 0f;
        colorSlider.maxValue = rainbowColors.Length - 1;
        colorSlider.wholeNumbers = true; // 정수값만
        colorSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        int index = Mathf.RoundToInt(value); // 슬라이더 값 → 색 인덱스
        ChangeColor(index);
    }

    void ChangeColor(int index)
    {
        liquidRenderer.material.color = rainbowColors[index];
    }
}
