using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public static ProgressBar Instance { get; private set; }

    public Slider progressSlider;
    private float currentProgress = 0f;

    private void Awake()
    {
        Instance = this;
        progressSlider.value = currentProgress;
    }

    public void AddProgress(float percent)
    {
        currentProgress += percent;
        currentProgress = Mathf.Clamp(currentProgress, 0f, 100f); // 최대 100%
        progressSlider.value = currentProgress;
    }

    public void ResetProgress()
    {
        currentProgress = 0f;
        progressSlider.value = 0f;
    }
    
}
