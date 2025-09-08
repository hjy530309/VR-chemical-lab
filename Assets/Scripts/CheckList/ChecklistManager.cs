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
    public int chapter_id;
}

public class ChecklistManager : MonoBehaviour
{
    public RectTransform checklistParent;   // ScrollView Content
    public GameObject checklistPrefab;      // Toggle+Text 프리팹

    private Dictionary<int, ChecklistUI> uiMap = new Dictionary<int, ChecklistUI>();
    private int currentChapterId = 0;

    void Start()
    {
        InvokeRepeating(nameof(RefreshChecklist), 2f, 5f);
    }

    public void LoadStepsForChapter(int chapterId)
    {
        Debug.Log($"체크리스트: 챕터 {chapterId} 단계 불러오기 요청");
        currentChapterId = chapterId;

        // 이전 UI 초기화
        uiMap.Clear();
        foreach (Transform child in checklistParent)
        {
            Destroy(child.gameObject);
        }

        StartCoroutine(LoadChecklist(chapterId));
    }

    IEnumerator LoadChecklist(int chapterId)
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
                ChecklistItem[] items = JsonHelper.FromJsonArray<ChecklistItem>(json);

                // 진행 중 단계 계산 (이전 단계가 완료되면 현재 단계 진행 중)
                int maxDoneId = 0;
                foreach (var doneItem in items)
                {
                    if (doneItem.is_done && doneItem.id > maxDoneId)
                        maxDoneId = doneItem.id;
                }

                foreach (var item in items)
                {
                    bool inProgress = (item.id == maxDoneId + 1);

                    if (!uiMap.ContainsKey(item.id))
                    {
                        // Prefab 생성
                        GameObject obj = Instantiate(checklistPrefab, checklistParent);
                        obj.transform.localScale = Vector3.one;

                        //RectTransform rt = obj.GetComponent<RectTransform>();
                        //rt.localScale = Vector3.one;
                        //rt.sizeDelta = new Vector2(checklistParent.rect.width, rt.sizeDelta.y);
                        //rt.anchoredPosition = new Vector2(0, 0);

                        ChecklistUI ui = obj.GetComponent<ChecklistUI>();
                        ui.Setup(item.step_name, item.is_done, inProgress);

                        uiMap[item.id] = ui;
                    }
                    else
                    {
                        var ui = uiMap[item.id];
                        // 글씨, 토글, 색상 모두 갱신
                        ui.Setup(item.step_name, item.is_done, inProgress);
                    }
                }
            }
        }
    }

    void RefreshChecklist()
    {
        StartCoroutine(LoadChecklist(currentChapterId));
    }

    public void CompleteStep(int id)
    {
        Debug.Log($"ChecklistManager: 단계 완료 호출됨 (ID: {id})");
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
                StartCoroutine(LoadChecklist(currentChapterId)); // 현재 챕터 갱신
            }
            else
            {
                Debug.LogError("단계 완료 실패: " + www.error);
            }
        }
    }
}

