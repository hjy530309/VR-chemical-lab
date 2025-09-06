using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ChecklistItem
{
    public int id;
    public string step_name;
    public bool is_done;
}

public class ChecklistManager : MonoBehaviour
{
    public RectTransform checklistParent;   // ScrollView Content
    public GameObject checklistPrefab;  // Toggle+Text 프리팹

    private Dictionary<int, ChecklistUI> uiMap = new Dictionary<int, ChecklistUI>();

    void Start()
    {
        StartCoroutine(LoadChecklist());           // 처음 체크리스트 불러오기
        InvokeRepeating(nameof(RefreshChecklist), 2f, 5f); // 5초마다 갱신
    }

    IEnumerator LoadChecklist()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/checklist"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("API 요청 실패: " + www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                ChecklistItem[] items = JsonHelper.FromJson<ChecklistItem>(json);

                foreach (var item in items)
                {
                    if (!uiMap.ContainsKey(item.id))
                    {
                        // Prefab 생성
                        GameObject obj = Instantiate(checklistPrefab, checklistParent);

                        // RectTransform 자동 조정 (Content 폭에 맞춤)
                        RectTransform rt = obj.GetComponent<RectTransform>();
                        rt.localScale = Vector3.one;
                        rt.sizeDelta = new Vector2(checklistParent.rect.width, rt.sizeDelta.y);
                        rt.anchoredPosition = new Vector2(0, 0);


                        // UI 세팅
                        var ui = obj.GetComponent<ChecklistUI>();
                        ui.Setup(item.step_name, item.is_done);

                        uiMap[item.id] = ui;
                    }
                    else
                    {
                        uiMap[item.id].SetChecked(item.is_done);
                    }
                }
            }
        }
    }

    void RefreshChecklist()
    {
        StartCoroutine(LoadChecklist());
    }

    // 체크 완료 시 API 호출
    public void CompleteStep(int id)
    {
        Debug.Log($"ChecklistManager: 단계 완료 호출됨 (ID: {id})"); // 🔹 로그 추가
        StartCoroutine(CompleteStepAPI(id));
    }

    IEnumerator CompleteStepAPI(int id)
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://localhost:3000/checklist/complete/" + id, ""))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("단계 완료: " + id);
                StartCoroutine(LoadChecklist()); // 완료 후 UI 갱신
            }
            else
            {
                Debug.LogError("단계 완료 실패: " + www.error);
            }
        }
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
