using UnityEngine;
using TMPro;

public class ControlRod : MonoBehaviour
{
    [Header("근접 제어 설정")]
    public Transform targetFuelPellet; // 제어할 FuelPellet
    public float maxDistance = 2f;     // 이 거리 안에서 속도 감소 적용
    [HideInInspector] public float controlLevel = 1f;    // 0 = 완전 감속, 1 = 정상 속도

    [Header("UI")]
    public TMP_Text speedText;          // 속도 % 표시 UI

    void Update()
    {
        if (targetFuelPellet == null) return;

        float distance = Vector3.Distance(transform.position, targetFuelPellet.position);

        // 가까울수록 1에 가까워지도록 보정
        controlLevel = Mathf.Clamp01(1f - (distance / maxDistance));

        UpdateUI();
    }

    void UpdateUI()
    {
        if (speedText != null)
        {
            int percent = Mathf.RoundToInt((1f - controlLevel) * 100f);
            speedText.text = $"Neutron Slowdown: {percent}%";
        }
    }
}
