using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementCounter : MonoBehaviour
{
    [System.Serializable]
    public class ElementData
    {
        public string name;
        public Text textUI;
        public SequentialSpawner spawner;
    }

    [Header("원소 리스트")]
    public List<ElementData> elements = new List<ElementData>();

    private Dictionary<string, int> elementCounts = new Dictionary<string, int>();
    private Dictionary<string, ElementData> elementMap = new Dictionary<string, ElementData>();

    void Awake()
    {
        // 딕셔너리 초기화
        foreach (var element in elements)
        {
            if (element == null || string.IsNullOrEmpty(element.name)) continue;

            elementMap[element.name] = element;
            elementCounts[element.name] = 0;

            if (element.textUI != null)
                element.textUI.text = "0";
        }
    }

    public void AddElement(string elementName, int amount = 1)
    {
        if (!elementCounts.ContainsKey(elementName)) return;

        elementCounts[elementName] += amount;
        UpdateUI(elementName);
    }

    public void RemoveElement(string elementName, int amount = 1)
    {
        if (!elementCounts.ContainsKey(elementName)) return;

        elementCounts[elementName] = Mathf.Max(0, elementCounts[elementName] - amount);
        UpdateUI(elementName);
    }

    private void UpdateUI(string elementName)
    {
        if (!elementMap.ContainsKey(elementName)) return;

        var data = elementMap[elementName];
        if (data.textUI != null)
            data.textUI.text = elementCounts[elementName].ToString();
    }

    public int GetElementCount(string elementName)
    {
        return elementCounts.ContainsKey(elementName) ? elementCounts[elementName] : 0;
    }
}
