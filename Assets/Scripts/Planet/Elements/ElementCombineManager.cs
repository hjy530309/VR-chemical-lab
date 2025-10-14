using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

[System.Serializable]
public class Recipe
{
    public string name; // 레시피 이름 (예: "Water")
    public List<Ingredient> ingredients; // 필요 원소와 개수
    public NetworkObject resultPrefab;   // ⚠️ NetworkObject로 변경
}

[System.Serializable]
public class Ingredient
{
    public string elementName; // "H", "O", "C" 등
    public int amount;         // 몇 개 필요한지
}

public class ElementCombineManager : MonoBehaviour
{
    [Header("Recipe Settings")]
    public List<Recipe> recipes;

    [Header("Combine Cooldown")]
    public float combineCooldown = 0.5f;   // 합성 쿨타임 (초 단위)
    private bool isOnCooldown = false;     // 쿨타임 여부

    [Header("Fusion Network Runner")]
    public NetworkRunner runner;           // 🔹 Fusion Runner 연결

    private Dictionary<string, int> playerInventory = new Dictionary<string, int>();

    public bool TryCombine(List<GameObject> selectedObjects)
    {
        if (isOnCooldown)
            return false;

        if (runner == null || !runner.IsRunning)
        {
            Debug.LogWarning("⚠️ NetworkRunner가 준비되지 않았습니다.");
            return false;
        }

        // 선택된 원소를 Dictionary로 변환
        Dictionary<string, int> selectedElements = new Dictionary<string, int>();
        foreach (var obj in selectedObjects)
        {
            ElementObject element = obj.GetComponent<ElementObject>();
            if (element == null) continue;

            if (selectedElements.ContainsKey(element.elementName))
                selectedElements[element.elementName] += element.amount;
            else
                selectedElements[element.elementName] = element.amount;
        }

        // 레시피 확인
        foreach (var recipe in recipes)
        {
            bool canCombine = true;
            foreach (var ingredient in recipe.ingredients)
            {
                if (!selectedElements.ContainsKey(ingredient.elementName) ||
                    selectedElements[ingredient.elementName] < ingredient.amount)
                {
                    canCombine = false;
                    break;
                }
            }

            if (canCombine)
            {
                // 결과 생성 위치: 첫 번째 재료 위치
                Vector3 spawnPos = selectedObjects[0].transform.position;
                Quaternion spawnRot = Quaternion.identity;

                PlayerRef localPlayer = runner.LocalPlayer;

                // 🔹 Fusion 방식으로 네트워크 스폰
                NetworkObject newObj = runner.Spawn(
                    recipe.resultPrefab,
                    spawnPos,
                    spawnRot,
                    localPlayer
                );

                if (newObj != null)
                {
                    Debug.Log($"{recipe.name} 합성 성공! (Fusion Spawn)");
                }
                else
                {
                    Debug.LogError("⚠️ Fusion Spawn 실패!");
                }

                // 재료 오브젝트 제거 (네트워크 오브젝트라면 runner.Despawn 사용)
                foreach (var obj in selectedObjects)
                {
                    var netObj = obj.GetComponent<NetworkObject>();
                    if (netObj != null && runner.IsServer)
                        runner.Despawn(netObj);
                    else
                        Destroy(obj);
                }

                // 쿨타임 시작
                StartCoroutine(CombineCooldownCoroutine());
                return true;
            }
        }

        Debug.Log("합성 실패!");
        StartCoroutine(CombineCooldownCoroutine());
        return false;
    }

    private IEnumerator CombineCooldownCoroutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(combineCooldown);
        isOnCooldown = false;
    }
}
