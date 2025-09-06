using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChecklistUI : MonoBehaviour
{
    public TMP_Text label;
    public Toggle toggle;

    public void Setup(string stepName, bool isDone)
    {
        label.text = stepName;
        toggle.isOn = isDone;
        toggle.interactable = false; // 유저가 직접 클릭 못하게
        Debug.Log("Setting label: " + stepName);
    }

    public void SetChecked(bool value)
    {
        toggle.isOn = value;
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

