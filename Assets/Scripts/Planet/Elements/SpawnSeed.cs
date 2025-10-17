using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class SpawnSeed : MonoBehaviour
{
    [Header("Spawn Settings")]
    public NetworkObject objectPrefab;      // NetworkObject가 붙은 Prefab
    public Transform defaultPosition;       // 기본 위치

    [Header("Network Runner")]
    public NetworkRunner runner;            // Inspector에서 연결

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
        if (runner == null)
        {
            runner = FindObjectOfType<NetworkRunner>();
            if (runner != null)
                Debug.Log("SpawnSeed: Runner reconnected from DontDestroyOnLoad");
            else
            {
                Debug.LogWarning("SpawnSeed: Runner not found in the scene!");
                return; // runner 없으면 종료
            }
        }

        // runner가 준비되었으면 즉시 Spawn
        SpawnSeedObject();
    }

    private void SpawnSeedObject()
    {
        if (objectPrefab == null) return;

        runner.Spawn(
            objectPrefab,
            defaultPosition.position,
            defaultPosition.rotation,
            runner.LocalPlayer
        );

        Debug.Log("SpawnSeed: Seed object spawned!");
    }

}
