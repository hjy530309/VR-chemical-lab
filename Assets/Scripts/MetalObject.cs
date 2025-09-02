using Fusion;
using TMPro;
using UnityEngine;
using TMPro;


public class MetalObject : NetworkBehaviour
{
    public bool InElectrolyte = false;        // 전해질 안에 있는지 여부
    public float platingProgress = 0f;        // 도금 진행도 (0 ~ 1)
    public float platingSpeed = 0.2f;         // 초당 도금 속도
    public PowerSupply powerSupply;          // 전원 장치 참조

    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private ParticleSystem platingParticles;

    [SerializeField] private Color baseColor = Color.gray;
    [SerializeField] private Color goldColor = new Color(1.0f, 0.84f, 0.0f); // 금색
    [SerializeField] private Color emissionColor = new Color(1.0f, 0.84f, 0.0f); // Emission용 색상

    [SerializeField] private ElectricCurrentEffect electricEffect;  // 전원장치 연결 이팩트

    [SerializeField] private TMP_Text platingPercentageText;    // 도금률 표시

    private Material _materialInstance;


    [SerializeField] private ChecklistManager checklistManager; // 씬에 있는 ChecklistManager
    [SerializeField] private int stepId = 3; // DB에서 정의한 금속 도금 단계 ID
    private bool stepCompleted = false; // 중복 호출 방지

    private void Start()
    {
        // 렌더러 자동 연결
        if (objectRenderer == null)
            objectRenderer = GetComponentInChildren<Renderer>();

        // 머티리얼 인스턴스화 (공유 머티리얼 오염 방지)
        _materialInstance = objectRenderer.material;
        _materialInstance.color = baseColor;

        // Emission 기능 활성화
        _materialInstance.EnableKeyword("_EMISSION");
        _materialInstance.SetColor("_EmissionColor", Color.black);

        // 파티클 초기화
        if (platingParticles != null)
            platingParticles.Stop();
    }

    private void Update()
    {
        bool shouldPlating = powerSupply != null && powerSupply.IsPoweredOn && InElectrolyte && platingProgress < 1f;

        if (shouldPlating)
        {
            // 도금 진행
            platingProgress += platingSpeed * Time.deltaTime;
            platingProgress = Mathf.Clamp01(platingProgress);

            // 색상 변화
            Color currentColor = Color.Lerp(baseColor, goldColor, platingProgress);
            _materialInstance.SetColor("_Color", currentColor);
            _materialInstance.SetColor("_EmissionColor", emissionColor * platingProgress * 2f);

            // 파티클 재생
            if (platingParticles != null && !platingParticles.isPlaying)
            {
                platingParticles.Play();
                Debug.Log("파티클 재생 시작");
            }

            // 전류 이펙트 ON
            if (electricEffect != null)
                electricEffect.EnableEffect();
        }
        else
        {
            // 전해질 밖이거나 완료된 경우 파티클/이펙트 정지
            if (platingParticles != null && platingParticles.isPlaying)
                platingParticles.Stop();

            if (electricEffect != null)
                electricEffect.DisableEffect();
        }

        // 도금 완료 시 효과 고정
        if (platingProgress >= 1f)
        {
            platingProgress = 1f;
            _materialInstance.SetColor("_EmissionColor", emissionColor * 5f);

            if (platingParticles != null && platingParticles.isPlaying)
                platingParticles.Stop();

            if (electricEffect != null)
                electricEffect.DisableEffect();

            // 체크리스트 완료 호출
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
