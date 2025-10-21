using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NeutronSpeedController : MonoBehaviour
{
    [Header("속도 조절")]
    [Tooltip("한 번 누를 때 속도 배율 변화")]
    public float speedStep = 0.2f;
    [Tooltip("최소 속도")]
    public float minMultiplier = 0.1f;
    [Tooltip("최대 속도")]
    public float maxMultiplier = 4f;

    [Header("UI")]
    [Tooltip("중성자 속도 표시 텍스트")]
    public TMP_Text speedText;

    [HideInInspector]
    public float globalMultiplier = 1f; // 현재 속도 배율, 인스펙터에서 보여주지 않음

    private List<Neutron> neutrons => NeutronManager.Instance.GetNeutrons();

    void Start()
    {
        UpdateSpeedUI();
    }

    // 버튼 OnClick에서 선택 가능
    public void IncreaseSpeed()
    {
        globalMultiplier = Mathf.Min(globalMultiplier + speedStep, maxMultiplier);
        UpdateNeutronSpeed();
    }

    public void DecreaseSpeed()
    {
        globalMultiplier = Mathf.Max(globalMultiplier - speedStep, minMultiplier);
        UpdateNeutronSpeed();
    }

    private void UpdateNeutronSpeed()
    {
        foreach (var n in neutrons)
        {
            if (n != null)
                n.ModifySpeed(globalMultiplier);
        }

        UpdateSpeedUI();
    }

    private void UpdateSpeedUI()
    {
        if (speedText != null)
            speedText.text = $"Neutron Speed: {globalMultiplier:0.00}x";
    }
}
