using UnityEngine;
using TMPro;


public class ExperimentUI : MonoBehaviour
{
    public static ExperimentUI Instance;

    [Header("UI")]
    public TextMeshProUGUI energyText;
    public GameObject successPanel;   
    public GameObject failPanel;      

    [Header("에너지 설정")]
    public int targetEnergy = 100;   

    private int energy = 0;
    private bool isEnded = false;

    void Awake()
    {
        Instance = this;

        if (successPanel != null)
            successPanel.SetActive(false);

        if (failPanel != null)
            failPanel.SetActive(false);

        UpdateEnergyUI();
    }

    // 핵분열 시 호출되는 함수
    public void AddEnergy(int amount)
    {
        if (isEnded) return;

        energy += amount;
        UpdateEnergyUI();

        // 성공 판정
        if (energy >= targetEnergy)
        {
            ShowSuccess();
        }
    }

    private void UpdateEnergyUI()
    {
        if (energyText != null)
            energyText.text = $"Energy: {energy} / {targetEnergy}";
    }

    private void ShowSuccess()
    {
        isEnded = true;

        if (successPanel != null)
            successPanel.SetActive(true);

        Debug.Log("실험 성공!");
    }

    // NeutronManager에서 호출할 실패 함수
    public void ShowFail()
    {
        if (isEnded) return;

        isEnded = true;

        if (failPanel != null)
            failPanel.SetActive(true);

        Debug.Log("실험 실패!");
    }
}