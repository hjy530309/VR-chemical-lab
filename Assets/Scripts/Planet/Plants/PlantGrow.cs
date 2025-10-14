using UnityEngine;
using System.Collections;

public class PlantGrow : MonoBehaviour
{
    [Header("성장 속도 설정")]
    public float growDuration = 2f; // 몇 초 동안 자랄지

    private Vector3 startScale = Vector3.zero;
    private Vector3 targetScale = Vector3.one;
    private Coroutine growRoutine;

    void Awake()
    {
        transform.localScale = Vector3.zero; // 시작 크기 0
    }

    public void StartGrowing()
    {
        // 이미 성장 중이면 중복 실행 방지
        if (growRoutine != null)
            StopCoroutine(growRoutine);

        growRoutine = StartCoroutine(GrowOverTime());
    }

    private IEnumerator GrowOverTime()
    {
        float elapsed = 0f;
        transform.localScale = startScale;

        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / growDuration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
        growRoutine = null; // 완료 후 초기화
    }
}
