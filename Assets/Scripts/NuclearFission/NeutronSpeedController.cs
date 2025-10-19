using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeutronSpeedController : MonoBehaviour
{
    public Slider speedSlider; // 인스펙터에서 연결
    private List<Neutron> neutrons => NeutronManager.Instance.GetNeutrons();

    void Update()
    {
                float multiplier = speedSlider.value; // 0~1 범위
                foreach (var n in neutrons)
                {
                    if (n != null)
                        n.ModifySpeed(multiplier); // 속도 조절
                }
    }
}
