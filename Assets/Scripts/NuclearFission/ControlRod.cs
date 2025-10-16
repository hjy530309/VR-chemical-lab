using UnityEngine;

public class ControlRod : MonoBehaviour
{
    public float absorbRange = 0.5f; // 중성자 흡수 범위

    void FixedUpdate()
    {
        foreach (var n in NeutronManager.Instance.GetNeutrons())
        {
            if (n == null) continue;

            float distance = Vector3.Distance(transform.position, n.transform.position);

            // 근접하면 흡수
            if (distance < absorbRange)
            {
                n.DestroySelf();
                Debug.Log("?? ControlRod 흡수!");
            }
        }
    }
}
