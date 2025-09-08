using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChecklistUI : MonoBehaviour
{
    public TMP_Text label;
    public Toggle toggle;

    public void Setup(string stepName, bool isDone, bool inProgress = false)
    {
        label.text = stepName;
        toggle.isOn = isDone;
        toggle.interactable = false; // 유저가 직접 클릭 못하게
        Debug.Log("Setting label: " + stepName);

        UpdateVisual(isDone, inProgress);

        // 토글 값 변경 시 비주얼 갱신
        toggle.onValueChanged.AddListener((val) => UpdateVisual(val, inProgress));
    }

    public void SetChecked(bool value, bool inProgress = false)
    {
        toggle.isOn = value;
        UpdateVisual(value, inProgress);
    }

    // 단계 완료 시 토글 true 표시
    public void UpdateVisual(bool isDone, bool inProgress)
    {
        if (isDone)
        {
            // 완료 단계
            label.color = Color.gray;
            label.fontStyle = FontStyles.Bold | FontStyles.Strikethrough;
        }
        else if (inProgress)
        {
            // 진행 중
            label.color = Color.green;
            label.fontStyle = FontStyles.Bold;
        }
        else
        {
            // 미시작
            label.color = Color.white;
            label.fontStyle = FontStyles.Normal;
        }
    }

    // JSON 배열 파싱 유틸
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
}

