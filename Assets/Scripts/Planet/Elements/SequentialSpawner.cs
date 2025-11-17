using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class SequentialSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public NetworkObject objectPrefab;      // NetworkObject가 붙은 Prefab
    public Transform defaultPosition;       // 기본 위치
    public string elementName;              // 원소 이름
    public ElementCounter elementMgr;       // 원소 관리 매니저
    public float maxDistance = 1f;          // 마지막 클론이 벗어나야 새로 스폰
    public AudioSource spawnAudio;

    [Header("Network Runner")]
    public NetworkRunner runner;            // Inspector에서 연결

    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // runner가 아직 연결되지 않았다면 DontDestroyOnLoad 씬에서 찾음
        if (runner == null)
        {
            runner = FindObjectOfType<NetworkRunner>();
            if (runner != null)
                Debug.Log("SequentialSpawner: Runner reconnected from DontDestroyOnLoad");
            else
                Debug.LogWarning("SequentialSpawner: Runner not found in the scene!");
        }

        // elementMgr도 자동 연결
        if (elementMgr == null)
        {
            elementMgr = FindObjectOfType<ElementCounter>();
            if (elementMgr != null)
                Debug.Log("SequentialSpawner: ElementCounter reconnected");
            else
                Debug.LogWarning("SequentialSpawner: ElementCounter not found in the scene!");
        }
    }

    void Update()
    {
        // null인 오브젝트 제거 (이미 삭제된 경우)
        spawnedObjects.RemoveAll(obj => obj == null);

        int remaining = elementMgr.GetElementCount(elementName);

        // 마지막 오브젝트 확인
        GameObject lastObj = spawnedObjects.Count > 0 ? spawnedObjects[spawnedObjects.Count - 1] : null;

        // 마지막 오브젝트가 없으면, 남은 수 > 1 이므로 스폰
        if (lastObj == null)
        {
            SpawnNewElement();
            return;
        }

        // 마지막 오브젝트가 범위를 벗어났으면 감소 후 새 스폰
        float distance = Vector3.Distance(lastObj.transform.position, defaultPosition.position);
        if (distance > maxDistance)
        {
            SpawnNewElement();
        }
    }

    private void SpawnNewElement()
    {
        if (elementMgr.GetElementCount(elementName) <= 0 || runner == null || objectPrefab == null)
            return;

        // 호스트면 직접 Spawn
        if (runner.IsServer)
        {
            DoSpawn(runner.LocalPlayer);
        }
        else
        {
            // 클라이언트면 요청 RPC
            RequestSpawn(runner.LocalPlayer);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RequestSpawn(PlayerRef requester, RpcInfo info = default)
    {
        DoSpawn(requester);
    }

    private void DoSpawn(PlayerRef owner)
    {
        spawnAudio.Play();
        elementMgr.RemoveElement(elementName, 1);

        NetworkObject netObj = runner.Spawn(
            objectPrefab,
            defaultPosition.position,
            defaultPosition.rotation,
            owner
        );

        if (netObj != null)
        {
            spawnedObjects.Add(netObj.gameObject);
            Debug.Log($"Spawned by {owner}: {netObj.name}");
        }
        else
        {
            Debug.LogError("Spawn 실패");
        }
    }

}
