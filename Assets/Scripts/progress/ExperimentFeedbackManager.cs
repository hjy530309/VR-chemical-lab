using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class FeedbackTarget
{
    public string objectName;               // 오브젝트 이름
    public TMP_Text feedbackText;           // 메시지 표시 위치
    public float minRange = 0.3f;           // 적정 최소값
    public float maxRange = 0.5f;           // 적정 최대값
}

public class ExperimentFeedbackManager : MonoBehaviour
{
    [Header("피드백 대상 리스트")]
    public List<FeedbackTarget> feedbackTargets = new List<FeedbackTarget>();

    [Header("기본 설정")]
    public float messageDuration = 2f;
    public Color lowColor = Color.red;
    public Color highColor = Color.yellow;
    public Color okColor = Color.green;

    private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();

    /// <summary>
    /// 오브젝트 이름과 값(예: 액체량)을 전달하면 자동으로 메시지를 출력합니다.
    /// </summary>
    public void CheckAmount(string objectName, float value)
    {
        FeedbackTarget target = feedbackTargets.Find(t => t.objectName == objectName);
        if (target == null || target.feedbackText == null) return;

        string message;
        Color color;

        if (value < target.minRange)
        {
            message = "The amount is too low";
            color = lowColor;
        }
        else if (value > target.maxRange)
        {
            message = "The amount is too high";
            color = highColor;
        }
        else
        {
            message = "The amount is just right!";
            color = okColor;
        }

        ShowMessage(target.feedbackText, message, color, objectName);
    }

    private void ShowMessage(TMP_Text textUI, string message, Color color, string key)
    {
        if (textUI == null) return;

        // 기존 코루틴이 실행 중이면 중단
        if (activeCoroutines.ContainsKey(key) && activeCoroutines[key] != null)
            StopCoroutine(activeCoroutines[key]);

        textUI.text = message;
        textUI.color = color;

        // 코루틴 시작
        activeCoroutines[key] = StartCoroutine(HideAfterDelay(textUI, key));
    }

    private IEnumerator HideAfterDelay(TMP_Text textUI, string key)
    {
        yield return new WaitForSeconds(messageDuration);
        activeCoroutines[key] = null;
    }
}
