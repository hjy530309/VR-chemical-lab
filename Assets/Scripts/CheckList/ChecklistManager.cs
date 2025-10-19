using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // ← UI 전환용

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
    [Header("Checklist UI")]
    public RectTransform checklistParent;   // ScrollView Content
    public GameObject checklistPrefab;      // Toggle+Text 프리팹

    [Header("UI Panels")]
    public GameObject checklistPanel;       // 현재 체크리스트 패널
    public GameObject chapterSelectPanel;   // 챕터 선택 패널
    public Button backButton;               // "돌아가기" 버튼

    private Dictionary<int, ChecklistUI> uiMap = new Dictionary<int, ChecklistUI>();
    private int currentChapterId = 0;

    void Start()
    {
        // 주기적으로 갱신
        InvokeRepeating(nameof(RefreshChecklist), 2f, 5f);

        // 돌아가기 버튼 연결
        if (backButton != null)
        {
            backButton.onClick.AddListener(ReturnToChapterSelect);

            // ExperimentManager의 Back 버튼 기능 호출
            backButton.onClick.AddListener(() =>
            {
                ExperimentManager expMgr = FindObjectOfType<ExperimentManager>();
                if (expMgr != null)
                    expMgr.OnBackButtonPressed();
            });
        }
            
    }

    public void LoadStepsForChapter(int chapterId)
    {
        Debug.Log($"체크리스트: 챕터 {chapterId} 단계 불러오기 요청");
        currentChapterId = chapterId;

        uiMap.Clear();
        foreach (Transform child in checklistParent)
            Destroy(child.gameObject);

        StartCoroutine(ResetChecklist());
        StartCoroutine(LoadChecklist(chapterId));
    }

    IEnumerator LoadChecklist(int chapterId)
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"http://localhost:3000/checklist/{chapterId}"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("API 요청 실패: " + www.error);
                yield break;
            }

            string json = www.downloadHandler.text;
            ChecklistItem[] allItems = JsonHelper.FromJsonArray<ChecklistItem>(json);

            // ✅ 현재 챕터만 필터링
            List<ChecklistItem> items = new List<ChecklistItem>();
            foreach (var item in allItems)
            {
                if (item.chapter_id == chapterId)
                    items.Add(item);
            }

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
                    GameObject obj = Instantiate(checklistPrefab, checklistParent);
                    obj.transform.localScale = Vector3.one;

                    ChecklistUI ui = obj.GetComponent<ChecklistUI>();
                    ui.Setup(item.step_name, item.is_done, inProgress);
                    uiMap[item.id] = ui;
                }
                else
                {
                    var ui = uiMap[item.id];
                    ui.Setup(item.step_name, item.is_done, inProgress);
                }
            }
        }
    }

    void RefreshChecklist() => StartCoroutine(LoadChecklist(currentChapterId));

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
                StartCoroutine(LoadChecklist(currentChapterId));
            else
                Debug.LogError("단계 완료 실패: " + www.error);
        }
    }

    public void UndoStep(int id)
    {
        Debug.Log($"ChecklistManager: 단계 되돌리기 호출됨 (ID: {id})");
        StartCoroutine(UndoStepAPI(id));
    }

    IEnumerator UndoStepAPI(int id)
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://localhost:3000/checklist/undo/" + id, ""))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                StartCoroutine(LoadChecklist(currentChapterId));
            else
                Debug.LogError("단계 되돌리기 실패: " + www.error);
        }
    }

    // "돌아가기" 기능 추가
    void ReturnToChapterSelect()
    {
        Debug.Log("챕터 선택 화면으로 돌아갑니다.");
        if (checklistPanel != null) checklistPanel.SetActive(false);
        if (chapterSelectPanel != null) chapterSelectPanel.SetActive(true);
    }

    // 진행 초기화
    IEnumerator ResetChecklist()
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://localhost:3000/checklist/reset", ""))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("체크리스트 초기화 성공");
                // 필요 시 체크리스트 다시 로드
                StartCoroutine(LoadChecklist(currentChapterId));
            }
            else
            {
                Debug.LogError("체크리스트 초기화 실패: " + www.error);
            }
        }
    }
}
