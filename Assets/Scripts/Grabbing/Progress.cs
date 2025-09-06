using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public enum LiquidType {
    None,
    A,
    B
}

public class Progress : MonoBehaviour
{
    public int nowProcess = 1;
    public LiquidType requiredLiquid = LiquidType.A; // 이번 단계에 필요한 액체

    public StepManager stepManager;
    public TMP_Text contentField;
    public GameObject nowObj;
    public GameObject Progress1_Obj1;
    public GameObject Progress1_Obj2;
    public GameObject Progress2_Obj1;
    public GameObject Progress2_Obj2;
    public float percent;

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
            case 1000:
                if ((nowObj == Progress1_Obj1 && item == Progress1_Obj2) || 
                    (nowObj == Progress1_Obj2 && item == Progress1_Obj1))
                {
                    // 자식의 자식에 있는 LiquidVolumeAnimator 가져오기
                    LiquidVolumeAnimator target = item.GetComponentInChildren<LiquidVolumeAnimator>();

                    if (target != null && Mathf.Approximately(target.level, 0.1f))
                    {
                        ProgressBar.Instance.AddProgress(percent);
                        nowProcess++;
                        GetContent("1-2");
                    }
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

    public void OnLiquidTouch(LiquidType liquidType, GameObject targetObj)
    {
        // 올바른 액체인지 확인
        if (liquidType != requiredLiquid)
        {
            Debug.Log("잘못된 액체가 들어옴!");
            return; // 무시하거나, 실패 처리
        }

        switch (nowProcess)
        {
            case 1:
                if (targetObj == Progress1_Obj2)
                {
                    ProgressBar.Instance.AddProgress(percent);
                    nowProcess++;
                    requiredLiquid = LiquidType.B; // 다음 단계에 B가 필요하다면 갱신
                    GetContent("1-2");
                }
                break;

            case 2:
                if (targetObj == Progress2_Obj2)
                {
                    ProgressBar.Instance.AddProgress(percent);
                    nowProcess++;
                    requiredLiquid = LiquidType.None; // 필요 없으면 초기화
                    GetContent("1-3");
                }
                break;
        }
    }

}
