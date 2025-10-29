using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DualFeedbackTarget
{
    public string objectName;           // 오브젝트 이름
    public TMP_Text brownText;          // 브라운 메시지 표시
    public TMP_Text grayText;           // 그레이 메시지 표시

    [Header("Brown Range")]
    public float brownMin = 0.3f;
    public float brownMax = 0.5f;

    [Header("Gray Range")]
    public float grayMin = 0.1f;
    public float grayMax = 0.3f;
}

public class DualExperimentFeedbackManager : MonoBehaviour
{
    [Header("피드백 대상 리스트")]
    public List<DualFeedbackTarget> feedbackTargets = new List<DualFeedbackTarget>();

    [Header("기본 설정")]
    public float messageDuration = 2f;
    public Color lowColor = Color.red;
    public Color highColor = Color.yellow;
    public Color okColor = Color.green;

    private Dictionary<string, Coroutine> activeCoroutines = new Dictionary<string, Coroutine>();

    /// <summary>
    /// 오브젝트 이름과 Brown/Gray 값을 전달하면 각각 메시지를 출력합니다.
    /// </summary>
    public void CheckAmounts(string objectName, float brownValue, float grayValue)
    {
        DualFeedbackTarget target = feedbackTargets.Find(t => t.objectName == objectName);
        if (target == null) return;

        if (target.brownText != null)
        {
            ShowMessage(target.brownText, brownValue, target.brownMin, target.brownMax, objectName + "_Brown");
        }

        if (target.grayText != null)
        {
            ShowMessage(target.grayText, grayValue, target.grayMin, target.grayMax, objectName + "_Gray");
        }
    }

    private void ShowMessage(TMP_Text textUI, float value, float min, float max, string key)
    {
        string message;
        Color color;

        if (value < min)
        {
            message = "The amount is too low";
            color = lowColor;
        }
        else if (value > max)
        {
            message = "The amount is too high";
            color = highColor;
        }
        else
        {
            message = "The amount is just right!";
            color = okColor;
        }

        if (activeCoroutines.ContainsKey(key) && activeCoroutines[key] != null)
            StopCoroutine(activeCoroutines[key]);

        textUI.text = message;
        textUI.color = color;

        activeCoroutines[key] = StartCoroutine(HideAfterDelay(textUI, key));
    }


    private IEnumerator HideAfterDelay(TMP_Text textUI, string key)
    {
        yield return new WaitForSeconds(messageDuration);
        activeCoroutines[key] = null;
    }
}
