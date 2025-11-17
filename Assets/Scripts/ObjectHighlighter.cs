using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectHighlighter : MonoBehaviour
{
    public static ObjectHighlighter Instance;
    public string chapter;
    public Dictionary<string, string> ItemMap => itemMap;

    private Dictionary<string, string> itemMap = new Dictionary<string, string>()
    {
        //Gold_Plating_Object
        { "power", "PowerSupply" },
        { "supply", "PowerSupply" },
        { "metal", "MetalObject" },
        { "plating", "ElectrolyteZone" },

        //Elephant's Toothpaste
        { "hydrogen", "hydrogen" },
        { "peroxide", "Bottle" },
        { "flask", "Erlenmeyer_flask" },
        { "detergent", "Fairy" },
        { "potassium", "Powder" },
        { "iodide", "Powder" },
        { "spoon", "Spoon" },

        //Thermit
        { "oxide", "PowderBottle(Brown)" },
        { "aluminum", "PowderBottle(Gray)" },
        { "bowl", "Plate" },
        { "chlorate", "Powder" },
        { "ribbon", "Ribbon" },
        { "torch", "Torch" },
        { "magnet", "Magnet" },
        
        //Firework
        { "scoop", "Scoop" },
        { "shell", "Shell1" },
        { "gunpowder", "GunpowderBox" },
        { "star", "Star" },
        { "color", "ColorBottle" },
        { "sawdust", "SawdustBox" },
        { "time", "wick" },
        { "fuse", "wick" },
        { "adhesive", "Glue" },
        { "completed shell", "ShellSphere" },
        { "firework", "rocket" },
        { "casing", "rocket" },
        { "site", "FireworkSetting" },
        { "launch", "FireworkSetting" }
    };

    public Dictionary<string, string> locationMap = new Dictionary<string, string>()
    {
        { "PowerSupply", "On the central table" },
        { "MetalObject", "On the central table" },
        { "ElectrolyteZone", "On the central table" },

        // Elephant's Toothpaste
        { "Bottle", "On the second row of the right cabinet" },
        { "Erlenmeyer_flask", "On the second row of the left cabinet" },
        { "Fairy", "On the first row of the left shelf" },
        { "Powder", "On the first row of the right cabinet" },
        { "Spoon", "On the second row of the left cabinet" },

        // Thermit
        { "PowderBottle(Brown)", "On the second row of the right cabinet" },
        { "PowderBottle(Gray)", "On the second row of the right cabinet" },
        { "Plate", "On the second row of the left shelf" },
        { "Ribbon", "On the second row of the left shelf" },
        { "Torch", "On the table" },
        { "Magnet", "On the table" },

        // Firework
        { "Scoop", "On the second row of the left cabinet" },
        { "Shell1", "On the table" },
        { "GunpowderBox", "On the third row of the left shelf" },
        { "Star", "On the table" },
        { "ColorBottle", "On the second row of the left shelf" },
        { "SawdustBox", "On the third row of the left shelf" },
        { "wick", "On the second row of the left shelf" },
        { "Glue", "On the first row of the left shelf" },
        { "ShellSphere", "On the table" },
        { "rocket", "On the right table" },
        { "FireworkSetting", "On the right side of the table" }
    };


    public IEnumerable<string> ItemKeys => itemMap.Keys;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // 중복 방지
    }
    public void HighlightItem(string itemName)
    {
        itemName = itemName.ToLower();
        if (itemMap.TryGetValue(itemName, out string targetName))
        {
            GameObject parent = GameObject.Find(chapter);
            Transform target = parent.transform.FindDeepChild(targetName);
            if(parent == null) Debug.LogWarning("부모 오브젝트 없음");
            if(target == null) Debug.LogWarning($"타겟 {targetName} 없음");

            if (target != null)
            {
                StartCoroutine(HighlightEffect(target.gameObject));
            }
            else
            {
                Debug.LogWarning($"오브젝트 '{targetName}'를 찾을 수 없음");
            }
        }
        else
        {
            Debug.LogWarning($"매핑에 '{itemName}' 없음");
        }
    }

    private IEnumerator HighlightEffect(GameObject obj, float blinkWidth = 5f, int blinkCount = 10, float interval = 0.5f)
    {
        Outline[] outlines = obj.GetComponentsInChildren<Outline>();
        if (outlines.Length == 0) yield break;

        // 현재 OutlineColor 저장
        Color[] originalColors = new Color[outlines.Length];
        for (int i = 0; i < outlines.Length; i++)
            originalColors[i] = outlines[i].OutlineColor;

        for (int i = 0; i < blinkCount; i++)
        {
            // ON: OutlineWidth + 알파 1
            foreach (var o in outlines)
            {
                o.OutlineWidth = blinkWidth;
                Color c = o.OutlineColor;
                c.a = 1f; // 완전 불투명
                o.OutlineColor = c;
            }

            yield return new WaitForSeconds(interval);

            // OFF: OutlineWidth 0 + 알파 0
            foreach (var o in outlines)
            {
                o.OutlineWidth = 0f;
                Color c = o.OutlineColor;
                c.a = 0f; // 완전히 투명
                o.OutlineColor = c;
            }

            yield return new WaitForSeconds(interval);
        }

        // 마지막에 원래 색으로 복원
        for (int i = 0; i < outlines.Length; i++)
            outlines[i].OutlineColor = originalColors[i];
    }

    public void SetGptChapter(int chapterId)
    {
        switch(chapterId)
        {
            case 1:
                chapter = "Gold_Plating_Object";
                break;
            case 2:
                chapter = "Elephant's Toothpaste";
                break;
            case 3:
                chapter = "Thermit";
                break;
            case 4:
                chapter = "Firework";
                break;
            case 5:
                chapter = "Planet";
                break;
            case 6:
                chapter = "Planet";
                break;
        }
    }

}
