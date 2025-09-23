using UnityEngine;
using TMPro;

public class ExperimentUI : MonoBehaviour
{
    public static ExperimentUI Instance;
    public TextMeshProUGUI energyText;
    private int energy = 0;

    void Awake() => Instance = this;

    public void AddEnergy(int amount)
    {
        energy += amount;
        energyText.text = "Energy: " + energy;
    }
}
