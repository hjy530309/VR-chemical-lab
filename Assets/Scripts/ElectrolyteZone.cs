using UnityEngine;
using Fusion;

public class ElectrolyteZone : NetworkBehaviour
{
    [SerializeField] private ChecklistManager checklistManager;
    [SerializeField] private int stepId = 2; // DB에서 정의한 도금액 단계 ID
    private bool stepCompleted = false;  // 중복 호출 방지

    private void OnTriggerEnter(Collider other)
    {

        var metal = other.GetComponentInParent<MetalObject>();
        if (metal != null)
        {
            Debug.Log("금속이 용액에 들어감");
            metal.InElectrolyte = true;

            // 처음 들어갔을 때만 체크리스트 완료
            if (!stepCompleted && checklistManager != null)
            {
                checklistManager.CompleteStep(stepId);
                stepCompleted = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        var metal = other.GetComponentInParent<MetalObject>();
        if (metal != null)
        {
            Debug.Log("금속이 용액에서 나옴");
            metal.InElectrolyte = false;
        }
    }
}
