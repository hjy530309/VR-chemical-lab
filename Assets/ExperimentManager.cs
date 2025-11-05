using Fusion;
using Fusion.Sockets;
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

    [Header("Chapter Prefabs")]
    public List<NetworkPrefabRef> chapterPrefabs;

    private NetworkObject currentChapterInstance;
    public NetworkRunner runner;

    private int lastSelectedChapterId = 0;

    private Dictionary<GameObject, Dictionary<Transform, bool>> chapterChildStates = new Dictionary<GameObject, Dictionary<Transform, bool>>();

    void Awake()
    {
        // ChecklistManager�� Canvas �ֻ����� ����
        cm = checklistManager.GetComponent<ChecklistManager>();
        checklistPanel.SetActive(false);
    }

    IEnumerator Start()
    {
        if (runner == null)
        {
            Debug.LogError("❌ NetworkRunner not found in scene!");
            yield break;
        }

        ShowStartPanel();
        yield return LoadChaptersFromServer();
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
        lastSelectedChapterId = chapterId;
        chapterPanel.SetActive(false);
        checklistPanel.SetActive(true);

        cm.LoadStepsForChapter(chapterId);

        //조수한테 챕터의 물건 찾을수있게함
        highlighter.SetGptChapter(chapterId);

        // 🔄 기존 인스턴스 제거
        if (currentChapterInstance != null)
        {
            runner.Despawn(currentChapterInstance);
            currentChapterInstance = null;
        }

        // ✅ Fusion으로 프리팹 스폰
        if (chapterId - 1 < chapterPrefabs.Count)
        {
            var prefabRef = chapterPrefabs[chapterId - 1];
            Vector3 spawnPosition = new Vector3(18.4266682f, -3.19088411f, 4.13165569f);
            currentChapterInstance = runner.Spawn(prefabRef, spawnPosition, Quaternion.identity);
            Debug.Log($"📦 Chapter {chapterId} spawned over network!");
        }
        else
        {
            Debug.LogWarning($"⚠️ Chapter prefab not assigned for ID {chapterId}");
        }

    }

    public void OnBackButtonPressed()
    {
        if (currentChapterInstance != null)
        {
            runner.Despawn(currentChapterInstance);
            currentChapterInstance = null;
        }
    }

    public void OnResetButtonPressed()
    {
        if (currentChapterInstance != null)
        {
            // 기존 오브젝트 Despawn
            runner.Despawn(currentChapterInstance);
            currentChapterInstance = null;
        }

        // 마지막으로 선택된 챕터 ID가 필요함
        // 선택한 챕터 ID를 저장하는 변수를 추가해야 함
        if(lastSelectedChapterId <= 0)
        {
            Debug.LogWarning("⚠️ Reset할 챕터 ID가 없습니다!");
            return;
        }

        // 같은 위치에서 다시 Spawn
        if(lastSelectedChapterId - 1 < chapterPrefabs.Count)
        {
            var prefabRef = chapterPrefabs[lastSelectedChapterId - 1];
            Vector3 spawnPosition = new Vector3(18.4266682f, -3.19088411f, 4.13165569f);
            currentChapterInstance = runner.Spawn(prefabRef, spawnPosition, Quaternion.identity);
            Debug.Log($"🔄 Chapter {lastSelectedChapterId} respawned at {spawnPosition}");
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
