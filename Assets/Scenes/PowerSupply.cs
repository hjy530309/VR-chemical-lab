using UnityEngine;

public class PowerSupply : MonoBehaviour
{
    public bool IsPoweredOn = false;

    [SerializeField] private GameObject switchVisual; // 시각적으로 움직일 스위치
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;

    private Renderer _renderer;

    // ChecklistManager 참조 (씬에 있는 것)
    [SerializeField] private ChecklistManager checklistManager;
    [SerializeField] private int stepId =1; // DB 단계 ID

    private void Start()
    {
        if (switchVisual != null)
            _renderer = switchVisual.GetComponent<Renderer>();

        UpdateVisual();
    }

    public void TogglePower()
    {
        IsPoweredOn = !IsPoweredOn;
        Debug.Log("전원 상태: " + (IsPoweredOn ? "ON" : "OFF"));
        UpdateVisual();

        // 전원 켜졌을 때 체크리스트 완료
        if (IsPoweredOn && checklistManager != null)
        {
            checklistManager.CompleteStep(stepId);
        }
    }

    private void UpdateVisual()
    {
        if (_renderer != null)
        {
            _renderer.material = IsPoweredOn ? onMaterial : offMaterial;
        }

        if (switchVisual != null)
        {
            // 스위치를 회전시켜서 상태 표시 (예: 위 = ON, 아래 = OFF)
            //float angle = IsPoweredOn ? -30f : 30f;
            //switchVisual.transform.localRotation = Quaternion.Euler(angle, 0, 0);
        }
    }

    private void OnMouseDown() // PC 테스트용 마우스 클릭
    {
        TogglePower();
    }
}
