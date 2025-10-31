using UnityEngine;
using UnityEngine.UI;

public class PlantGrow : MonoBehaviour
{
    [Header("성장 설정")]
    [Range(0f, 1f)] public float growthLevel = 0f; // 0~1 성장 정도
    public Vector3 startScale = Vector3.zero;
    public Vector3 targetScale = Vector3.one;

    [Header("열매 설정")]
    public Transform[] fruits;              // 하위 열매들
    private Vector3[] fruitTargetScales;    // 각 열매 최대 크기 저장
    public Vector3 fruitStartScale = Vector3.zero; // 시작 크기

    [Header("환경 설정")]
    public bool hasSand = false;
    public bool hasSoil = false; // 토양이 있는지 여부
    public bool hasSeed = false; // 토양이 있는지 여부
    public bool hasWater = false; // 토양이 있는지 여부
    public bool hasLight = false;

    [Header("UI 연결 (선택사항)")]
    public Slider growthSlider; // Unity UI 슬라이더 연결용

    private void Start()
    {
        // 슬라이더 연결되어 있다면 값 변경시 이벤트 등록
        if (growthSlider != null)
        {
            growthSlider.onValueChanged.AddListener(UpdateGrowthFromSlider);
            UpdateGrowthFromSlider(growthSlider.value); // 초기 반영
        }
        else
        {
            UpdateGrowth(growthLevel); // 슬라이더 없을 때 초기 크기 설정
        }
    }

    private void Awake()
    {
        if (fruits != null && fruits.Length > 0)
        {
            fruitTargetScales = new Vector3[fruits.Length];
            for (int i = 0; i < fruits.Length; i++)
            {
                fruitTargetScales[i] = fruits[i].localScale; // Inspector에 설정된 현재 크기를 최대 크기로 저장
                fruits[i].localScale = fruitStartScale;     // 시작 크기 0으로 초기화
            }
        }
    }

    // 슬라이더에서 호출되는 메서드
    public void UpdateGrowthFromSlider(float value)
    {
        growthLevel = value;
        UpdateGrowth(growthLevel);
    }

    // 직접 크기 변경 (0~1)
    public void UpdateGrowth(float level)
    {
        level = Mathf.Clamp01(level);

        float limit = 0.6f;

        if (!hasSeed || !hasWater)
        {
            limit = 0.0f; // 예: 0.0f
        }
        else if (hasWater && (!hasSoil || hasSand || (!hasSoil && !hasSand)))
        {
            limit = 0.1f; // 흙 없음 + 물 있음 → 0.1까지만
        }
        else if (hasWater && hasSoil && !hasLight)
        {
            limit = 0.4f;
        }
        else if (hasLight)
        {
            limit = 0.6f;
        }

        level = Mathf.Min(level, limit);

        transform.localScale = Vector3.Lerp(startScale, targetScale, level);

        // 하위 열매 성장 (0.3~0.6 범위에서만)
        if (fruits != null && fruits.Length > 0)
        {
            float fruitGrowth = 0f;

            if (level >= 0.3f)
            {
                // 0.3 ~ 0.6 범위를 0 ~ 1로 매핑
                fruitGrowth = Mathf.InverseLerp(0.3f, 0.6f, level);
                fruitGrowth = Mathf.Clamp01(fruitGrowth);
            }

            for (int i = 0; i < fruits.Length; i++)
            {
                fruits[i].localScale = Vector3.Lerp(fruitStartScale, fruitTargetScales[i], fruitGrowth);
            }
        }

    }
}
