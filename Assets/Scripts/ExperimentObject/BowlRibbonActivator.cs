using UnityEngine;

public class BowlRibbonActivator : MonoBehaviour
{
    public GameObject bowlRibbon;     // 그릇 자식 리본 (보여줄 리본)
    public GameObject targetRibbon;   // 닿았을 때 사라질 대상 리본 (특정 리본 오브젝트)

    private void Start()
    {
        if (bowlRibbon != null)
            bowlRibbon.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetRibbon)
        {
            if (bowlRibbon != null)
                bowlRibbon.SetActive(true);

            targetRibbon.SetActive(false);
        }
    }
}
