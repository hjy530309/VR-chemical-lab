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

    [Header("Gpt highlighter")]
    public ObjectHighlighter highlighter;

    private ChecklistManager cm;
    private List<ChapterItem> chapters = new List<ChapterItem>();

    [Header("Chapter Objects")]
    public List<GameObject> chapterObjects;

    private Dictionary<GameObject, Dictionary<Transform, bool>> chapterChildStates = new Dictionary<GameObject, Dictionary<Transform, bool>>();
    private GameObject currentChapterObject = null;

    void Awake()
    {
        // ChecklistManager�� Canvas �ֻ����� ����
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

        //조수한테 챕터의 물건 찾을수있게함
        highlighter.SetGptChapter(chapterId);

        // 챕터 오브젝트 활성화
        for (int i = 0; i < chapterObjects.Count; i++)
        {
            GameObject chapterParent = chapterObjects[i];

            if (i == chapterId - 1) // 선택한 챕터
            {
                // 현재 챕터 참조 저장
                currentChapterObject = chapterParent;

                // 자식 상태 저장
                Dictionary<Transform, bool> childStates = new Dictionary<Transform, bool>();
                foreach (Transform child in chapterParent.transform)
                    childStates[child] = child.gameObject.activeSelf;

                chapterParent.SetActive(true);

                // 자식 상태 복원
                foreach (var kvp in childStates)
                    kvp.Key.gameObject.SetActive(kvp.Value);
            }
            else
            {
                chapterParent.SetActive(false);
            }
        }
    }

    public void OnBackButtonPressed()
    {
        if (currentChapterObject != null)
        {
            currentChapterObject.SetActive(false);
            currentChapterObject = null;
            Debug.Log("Back 버튼: 현재 챕터 오브젝트 비활성화됨");
        }
    }

    IEnumerator LoadChaptersFromServer()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/api/chapters"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("é�� �ε� ����: " + www.error);
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

            SetupClonedButton(btnObj);

            Button btn = btnObj.GetComponent<Button>();
            TMP_Text btnText = btnObj.GetComponentInChildren<TMP_Text>();
            btnText.text = chapter.name;

            int capturedId = chapter.id;
            btn.onClick.AddListener(() => OnChapterSelected(capturedId));
        }
    }

    void SetupClonedButton(GameObject buttonObj)
    {
        // 1. 레이어 강제 설정
        buttonObj.layer = LayerMask.NameToLayer("UI");

        // 2. Collider 확인 및 추가
        BoxCollider collider = buttonObj.GetComponent<BoxCollider>();
        if (collider == null)
        {
            collider = buttonObj.AddComponent<BoxCollider>();
            RectTransform rect = buttonObj.GetComponent<RectTransform>();
            if (rect != null)
            {
                collider.size = new Vector3(rect.rect.width, rect.rect.height, 10f); // Z 두껍게
                collider.center = Vector3.zero;
            }
            Debug.Log($"🔹 Collider 추가됨: {buttonObj.name}");
        }

        // 3. Button 컴포넌트 활성화 확인
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            button.interactable = true;
        }

        // 4. 모든 자식에도 레이어 설정 (중요!)
        foreach (Transform child in buttonObj.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("UI");
        }
    }

}
