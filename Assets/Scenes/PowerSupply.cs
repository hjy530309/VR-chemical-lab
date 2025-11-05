using UnityEngine;

public class PowerSupply : MonoBehaviour
{
    public bool IsPoweredOn = false;

    [SerializeField] private GameObject switchVisual; // �ð������� ������ ����ġ
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;

    private Renderer _renderer;

    // ChecklistManager ���� (���� �ִ� ��)
    [SerializeField] private ChecklistManager checklistManager;
    [SerializeField] private int stepId =1; // DB �ܰ� ID

    private void Start()
    {
        if (switchVisual != null)
            _renderer = switchVisual.GetComponent<Renderer>();

        UpdateVisual();
    }

    void Awake()
    {
        if (checklistManager == null)
        {
            checklistManager = FindObjectOfType<ChecklistManager>();
            if(checklistManager == null)
                Debug.LogError("ChecklistManager를 찾을 수 없습니다!");
        }
    }

    public void TogglePower()
    {
        IsPoweredOn = !IsPoweredOn;
        Debug.Log("���� ����: " + (IsPoweredOn ? "ON" : "OFF"));
        UpdateVisual();

        // ���� ������ �� üũ����Ʈ �Ϸ�
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
            // ����ġ�� ȸ�����Ѽ� ���� ǥ�� (��: �� = ON, �Ʒ� = OFF)
            //float angle = IsPoweredOn ? -30f : 30f;
            //switchVisual.transform.localRotation = Quaternion.Euler(angle, 0, 0);
        }
    }

    private void OnMouseDown() // PC �׽�Ʈ�� ���콺 Ŭ��
    {
        TogglePower();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPoweredOn) // �̹� �������� ����
        {
            Debug.Log("����ġ�� ���� ����: " + other.name);
            TogglePower();
        }
    }
}
