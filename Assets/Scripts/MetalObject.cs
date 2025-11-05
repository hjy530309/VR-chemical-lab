using Fusion;
using TMPro;
using UnityEngine;
using TMPro;


public class MetalObject : NetworkBehaviour
{
    public bool InElectrolyte = false;        // ������ �ȿ� �ִ��� ����
    public float platingProgress = 0f;        // ���� ���൵ (0 ~ 1)
    public float platingSpeed = 0.2f;         // �ʴ� ���� �ӵ�
    public PowerSupply powerSupply;          // ���� ��ġ ����

    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private ParticleSystem platingParticles;

    [SerializeField] private Color baseColor = Color.gray;
    [SerializeField] private Color goldColor = new Color(1.0f, 0.84f, 0.0f); // �ݻ�
    [SerializeField] private Color emissionColor = new Color(1.0f, 0.84f, 0.0f); // Emission�� ����

    [SerializeField] private ElectricCurrentEffect electricEffect;  // ������ġ ���� ����Ʈ

    [SerializeField] private TMP_Text platingPercentageText;    // ���ݷ� ǥ��

    private Material _materialInstance;


    [SerializeField] private ChecklistManager checklistManager; // ���� �ִ� ChecklistManager
    [SerializeField] private int stepId = 3; // DB���� ������ �ݼ� ���� �ܰ� ID
    private bool stepCompleted = false; // �ߺ� ȣ�� ����

    private void Start()
    {
        // ������ �ڵ� ����
        if (objectRenderer == null)
            objectRenderer = GetComponentInChildren<Renderer>();

        // ��Ƽ���� �ν��Ͻ�ȭ (���� ��Ƽ���� ���� ����)
        _materialInstance = objectRenderer.material;
        _materialInstance.color = baseColor;

        // Emission ��� Ȱ��ȭ
        _materialInstance.EnableKeyword("_EMISSION");
        _materialInstance.SetColor("_EmissionColor", Color.black);

        // ��ƼŬ �ʱ�ȭ
        if (platingParticles != null)
            platingParticles.Stop();
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

    private void Update()
    {
        bool shouldPlating = powerSupply != null && powerSupply.IsPoweredOn && InElectrolyte && platingProgress < 1f;

        if (shouldPlating)
        {
            // ���� ����
            platingProgress += platingSpeed * Time.deltaTime;
            platingProgress = Mathf.Clamp01(platingProgress);

            // ���� ��ȭ
            Color currentColor = Color.Lerp(baseColor, goldColor, platingProgress);
            _materialInstance.SetColor("_Color", currentColor);
            _materialInstance.SetColor("_EmissionColor", emissionColor * platingProgress * 2f);

            // ��ƼŬ ���
            if (platingParticles != null && !platingParticles.isPlaying)
            {
                platingParticles.Play();
                Debug.Log("��ƼŬ ��� ����");
            }

            // ���� ����Ʈ ON
            if (electricEffect != null)
                electricEffect.EnableEffect();
        }
        else
        {
            // ������ ���̰ų� �Ϸ�� ��� ��ƼŬ/����Ʈ ����
            if (platingParticles != null && platingParticles.isPlaying)
                platingParticles.Stop();

            if (electricEffect != null)
                electricEffect.DisableEffect();
        }

        // ���� �Ϸ� �� ȿ�� ����
        if (platingProgress >= 1f)
        {
            platingProgress = 1f;
            _materialInstance.SetColor("_EmissionColor", emissionColor * 5f);

            if (platingParticles != null && platingParticles.isPlaying)
                platingParticles.Stop();

            if (electricEffect != null)
                electricEffect.DisableEffect();

            // üũ����Ʈ �Ϸ� ȣ��
            if (!stepCompleted && checklistManager != null)
            {
                checklistManager.CompleteStep(stepId);
                stepCompleted = true;
            }
        }

        if (platingPercentageText != null)
        {
            int percent = Mathf.RoundToInt(platingProgress * 100f);
            platingPercentageText.text = percent + " %";
        }
    }

}
