using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Progress : MonoBehaviour
{
    int nowProcess = 1;
    public StepManager stepManager;
    public TMP_Text contentField;
    public GameObject nowObj;
    public GameObject Progress1_Obj1;
    public GameObject Progress1_Obj2;
    public GameObject Progress2_Obj1;
    public GameObject Progress2_Obj2;
    private float percent;

    public void GetContent(string stepId)
    {
        if (stepManager != null && stepManager.stepList != null)
        {
            // 딕셔너리로 변환
            Dictionary<string, StepManager.StepData> stepDict = stepManager.stepList.ToDictionary(s => s.stepId);

            // stepId 단계의 내용 가져오기
            if (stepDict.ContainsKey(stepId))
            {
                string content = stepDict[stepId].content;

                // 텍스트 필드에 출력
                if (contentField != null)
                {
                    contentField.text = $"{content}";
                }
            }
        }
    }
    
    void Start(){
        GetContent("1-1");
        percent = 100/stepManager.stepList.Count;
        Debug.Log(percent + stepManager.stepList.Count);
    }
    
    // Object PickUp 처리
    public void OnPickUp(GameObject item)
    {
        nowObj = item;
        switch (nowProcess)
        {
            case 100:
                if (item == Progress1_Obj1)
                {
                    ProgressBar.Instance.AddProgress(percent);
                    nowProcess++;
                    GetContent("1-2");
                }
                break;

            case 200:
                if (item == Progress2_Obj1)
                {
                    ProgressBar.Instance.AddProgress(percent);
                    nowProcess++;
                    GetContent("1-3");
                }
                break;

        }
    }

    // 충돌 처리 (tag 기반)
    public void OnTouch(GameObject item)
    {
        switch (nowProcess)
        {
            case 1:
                if ((nowObj == Progress1_Obj1 && item == Progress1_Obj2) || (nowObj == Progress1_Obj2 && item == Progress1_Obj1))
                {
                    ProgressBar.Instance.AddProgress(percent);
                    nowProcess++;
                    GetContent("1-2");
                }
                break;

            case 2:
                if ((nowObj == Progress2_Obj1 && item == Progress2_Obj2) || (nowObj == Progress2_Obj2 && item == Progress2_Obj1))
                {
                    ProgressBar.Instance.AddProgress(percent);
                    nowProcess++;
                    GetContent("1-3");
                }
                break;

            // 필요한 만큼 case 계속 추가 가능
        }
    }

}
