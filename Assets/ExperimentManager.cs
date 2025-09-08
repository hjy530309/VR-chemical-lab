using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


[System.Serializable]
public class ChapterItem
{
    public int id;
    public string name;
}

public class ExperimentManager : MonoBehaviour
{

    [Header("Panels")]
    public GameObject startPanel;
    public GameObject chapterPanel;
    public GameObject checklistPanel;
    public GameObject checklistManager;


    [Header("Chapter Buttons Parent")]
    public RectTransform chapterButtonParent;
    public GameObject chapterButtonPrefab;

    private ChecklistManager cm;
    private List<ChapterItem> chapters = new List<ChapterItem>();


    void Awake()
    {
        // ChecklistManager는 Canvas 최상위에 있음
        cm = checklistManager.GetComponent<ChecklistManager>();
        checklistPanel.SetActive(false);
    }

    void Start()
    {
        ShowStartPanel();
        StartCoroutine(LoadChaptersFromServer());
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        chapterPanel.SetActive(false);
        checklistPanel.SetActive(false);
    }

    public void OnStartExperiment()
    {
        startPanel.SetActive(false);
        chapterPanel.SetActive(true);
        checklistPanel.SetActive(false);
    }

    void OnChapterSelected(int chapterId)
    {
        Debug.Log("선택한 챕터 ID: " + chapterId);
        chapterPanel.SetActive(false);
        checklistPanel.SetActive(true);

        cm.LoadStepsForChapter(chapterId);
    }

    IEnumerator LoadChaptersFromServer()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/api/chapters"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("챕터 로드 실패: " + www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                ChapterItem[] items = JsonHelper.FromJsonArray<ChapterItem>(json);
                chapters = new List<ChapterItem>(items);

                SetupChapterButtons();
            }
        }
    }

    void SetupChapterButtons()
    {
        foreach (Transform child in chapterButtonParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var chapter in chapters)
        {
            GameObject btnObj = Instantiate(chapterButtonPrefab, chapterButtonParent);
            btnObj.transform.localScale = Vector3.one;

            Button btn = btnObj.GetComponent<Button>();
            TMP_Text btnText = btnObj.GetComponentInChildren<TMP_Text>();
            btnText.text = chapter.name;

            int capturedId = chapter.id; // 람다 캡처 문제 방지
            btn.onClick.AddListener(() => OnChapterSelected(capturedId));
        }
    }


}
