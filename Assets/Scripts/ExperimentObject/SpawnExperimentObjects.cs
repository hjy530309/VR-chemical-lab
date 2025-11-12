using System.Collections;
using UnityEngine;
using Fusion;

public class SpawnExperimentObjects : MonoBehaviour
{
    public NetworkPrefabRef experimentGroupPrefab;
    private NetworkObject spawnedGroup;
    public NetworkRunner runner;

    public void StartSpawnCoroutine()
    {
        StartCoroutine(SpawnWhenRunnerReady());
    }

    private IEnumerator SpawnWhenRunnerReady()
    {
        // Runner가 null이면 찾기
        if (runner == null)
            runner = FindObjectOfType<NetworkRunner>();

        if (runner == null)
        {
            Debug.LogError("Runner가 씬에 없습니다!");
            yield break;
        }

        // Runner가 실행될 때까지 기다리기
        while (!runner.IsRunning)
        {
            yield return null; // 한 프레임 대기
        }

        Debug.Log("Runner 실행 완료, 이제 Spawn 시작!");

        // Spawn
        SpawnObjects();
    }

    public void SpawnObjects()
    {
        DespawnObjects();

        Vector3 spawnPos = Vector3.zero;
        spawnedGroup = runner.Spawn(experimentGroupPrefab, spawnPos, Quaternion.identity);

        if (spawnedGroup != null)
            Debug.Log("Spawn 성공!");
        else
            Debug.LogError("Spawn 실패!");
    }

    public void DespawnObjects()
    {
        if (spawnedGroup != null)
        {
            runner.Despawn(spawnedGroup);
            spawnedGroup = null;
        }
    }
}
