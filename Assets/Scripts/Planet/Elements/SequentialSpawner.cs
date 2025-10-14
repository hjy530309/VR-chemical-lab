using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SequentialSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public NetworkObject objectPrefab;      // NetworkObject가 붙은 Prefab
    public Transform defaultPosition;       // 기본 위치
    public string elementName;              // 원소 이름
    public ElementCounter elementMgr;       // 원소 관리 매니저
    public float maxDistance = 1f;          // 마지막 클론이 벗어나야 새로 스폰

    [Header("Network Runner")]
    public NetworkRunner runner;            // Inspector에서 연결

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private bool isFirst = true;

    public void ShowElement()
    {
        if (!isFirst) return;

        if (runner == null || !runner.IsRunning)
        {
            Debug.LogWarning("Runner가 준비되지 않았습니다.");
            return;
        }

        isFirst = false;
        SpawnNewElement();
    }

    void Update()
    {
        if (spawnedObjects.Count == 0) return;

        // 마지막으로 Spawn한 오브젝트만 체크
        GameObject lastObj = spawnedObjects[spawnedObjects.Count - 1];
        if (lastObj == null) return;

        float distance = Vector3.Distance(lastObj.transform.position, defaultPosition.position);
        if (distance > maxDistance)
        {
            // 남은 원소가 있으면 새로 스폰
            if (elementMgr.GetElementCount(elementName) > 0)
            {
                SpawnNewElement();
            }
        }
    }

    private void SpawnNewElement()
    {
        if (elementMgr.GetElementCount(elementName) <= 0 || runner == null || objectPrefab == null)
            return;

        PlayerRef localPlayer = runner.LocalPlayer;

        // 월드 좌표는 Spawn 위치로 임시 지정 (0,0,0이어도 괜찮음)
        NetworkObject netObj = runner.Spawn(
            objectPrefab,
            defaultPosition.position, // 월드 좌표, 임시
            defaultPosition.rotation,
            localPlayer
        );

        if (netObj != null)
        {
            // 부모에 붙이면서 로컬 좌표를 0,0,0으로 설정
            netObj.transform.SetParent(defaultPosition, false);
            netObj.transform.localPosition = Vector3.zero;
            netObj.transform.localRotation = Quaternion.identity;

            spawnedObjects.Add(netObj.gameObject);
            Debug.Log("Spawned as child at local 0,0,0: " + netObj.name);
        }
        else
        {
            Debug.LogError("Spawn 실패");
        }
    }

}
