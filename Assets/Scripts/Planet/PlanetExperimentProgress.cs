using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetExperimentProgress : MonoBehaviour
{
    public ElementCombineManager combineMgr; // Inspector에서 드래그 연결
    [SerializeField] private ChecklistManager checklistManager;
    [SerializeField] private int stepId =1;

    public int nowProcess = 1;
    public GameObject seedDesk;
    public GameObject dirt;
    public GameObject plant;
    public PlantGrow plantGrow;

    void Start()
    {
        dirt.SetActive(false);
        plant.SetActive(true);
    }
    
    public void OnTouch(GameObject targetObj, GameObject sourceObj)
    {
        Debug.Log("object touch" + targetObj + "and" + sourceObj);
        if (targetObj == null || sourceObj == null)
        return;

        // GameObject 리스트 생성 (null 제거)
        List<GameObject> selectedObjects = new List<GameObject> { targetObj, sourceObj };

        // TryCombine 호출
        combineMgr.TryCombine(selectedObjects);
        
        if(targetObj == seedDesk && sourceObj.name.Contains("SiO2"))
        {
            DeskMaterialToggle deskMaterial = FindObjectOfType<DeskMaterialToggle>();
            if(sourceObj.name.Contains("Fe"))
            {
                deskMaterial.ShowHiddenMaterial(new Color(205f / 255f, 91f / 255f, 51f / 255f));
            }
            else
            {
                deskMaterial.ShowHiddenMaterial(new Color(239f / 255f, 155f / 255f, 76f / 255f));
            }
            sourceObj.SetActive(false);
            plantGrow.hasSand = true;
        }
        if(targetObj == seedDesk && sourceObj.name.Contains("Soil"))
        {
            DeskMaterialToggle deskMaterial = FindObjectOfType<DeskMaterialToggle>();
            deskMaterial.ShowHiddenMaterial(new Color(82f/255f, 33f/255f, 7f/255f));
            sourceObj.SetActive(false);
            plantGrow.hasSoil = true;
            plantGrow.hasSand = false;
            nowProcess++;
        }
        if(targetObj == seedDesk && sourceObj.name.Contains("Seed"))
        {
            sourceObj.SetActive(false);
            dirt.SetActive(true);
            plantGrow.hasSeed = true;
            nowProcess++;
        }
        if(targetObj == seedDesk && sourceObj.name.Contains("H2O"))
        {
            sourceObj.SetActive(false);
            plant.SetActive(true);
            plantGrow.hasWater = true;
            nowProcess++;
        }
    }

    void AddToDictionary(Dictionary<string,int> dict, string key, int amount)
    {
        if(dict.ContainsKey(key)) dict[key] += amount;
        else dict[key] = amount;
    }

    // Start is called before the first frame update
    public void OnPickUp(GameObject item)
    {
    }
}
