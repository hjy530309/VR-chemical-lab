using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermitExperimentProgress : MonoBehaviour
{
    [SerializeField] private ExperimentFeedbackManager feedbackManager;
    [SerializeField] private ChecklistManager checklistManager;
    [SerializeField] private int stepId =1;
    
    public int nowProcess = 1;
    public ThermitSpoonManager spoonMgr;
    public ThermitPlateManager plateMgr;
    public SplitController PowderColor;
    private bool firstPowder = true;
    private bool notFill = true;

    public GameObject Progress1_Obj1;
    public GameObject Progress1_Obj2;
    public GameObject Progress1_Obj3;

    public GameObject Progress2_Obj1;
    public GameObject Progress2_Obj2;
    public GameObject Progress3_Obj1;
    public GameObject Progress3_Obj2;

    public GameObject result1;
    public GameObject result2;
    public GameObject result3;
    public GameObject magnet;

    public float shakeSensitivity = 1f; // 흔들림 감도
    private Vector3 lastPosition;
    private bool notCall = true;
    private int prevProcess = -1;
    private bool checkOne = false;

    void Start()
    {
        result1.SetActive(false);
        result2.SetActive(false);
        result3.SetActive(false);
        
        if (Progress2_Obj2 != null)
            lastPosition = Progress2_Obj2.transform.position;
    }

    void Update()
    {
        if(!plateMgr.powder.activeSelf)
            firstPowder = true;

        Vector2 powderScale = plateMgr.GetScale();
        if(nowProcess == 1 && powderScale.x > 0.1f || powderScale.x < 0.08f || powderScale.y > 0.04f || powderScale.y < 0.02f)
        {
            feedbackManager.CheckAmount("PlateBrown", powderScale.x);
            feedbackManager.CheckAmount("PlateGray", powderScale.y);

            notFill = true;
        }
        else
        {
            feedbackManager.CheckAmount("PlateBrown", powderScale.x);
            feedbackManager.CheckAmount("PlateGray", powderScale.y);

            notFill = false;
        }

        Vector2 ratio = plateMgr.GetRatio();

        if (ratio.x > 0f && ratio.y > 0f && !checkOne)
        {
            float actualRatio = ratio.y / ratio.x; // Obj2 / Obj1
            if (Mathf.Abs(actualRatio - 0.3f) <= 0.1f && !notFill) // 오차 ±0.2 허용
            {
                plateMgr.SetParticleColor(new Color(140f / 255f, 68f / 255f, 32f / 255f));
                nowProcess++;
                checklistManager.CompleteStep(stepId);
                stepId++;
                checkOne = true;
            }
        }

        if(nowProcess == 1)
        {
            result1.SetActive(false);
            result2.SetActive(false);
            result3.SetActive(false);
        }
        // ------------------ 흔들림 감지 ------------------
        if(nowProcess == 2)
        {
            Vector3 currentPosition = Progress2_Obj2.transform.position;
            float movement = (currentPosition - lastPosition).magnitude;

            // Rigidbody velocity 기반 필터
            Rigidbody rb = Progress2_Obj2.GetComponent<Rigidbody>();
            float velocity = (rb != null) ? rb.velocity.magnitude : 0f;

            if (movement * shakeSensitivity > 0.5f && velocity > 0.01f) // 손 움직임 기반
            {
                PowderColor.ChangePowderColor(new Color(140f/255f,68f/255f,32f/255f), Color.white);
                PowderColor.SetSplit(1f);
                nowProcess++;
                checklistManager.CompleteStep(stepId);
                stepId++;
            }

            lastPosition = currentPosition;
        }
        if(nowProcess == 6)
        {
            plateMgr.HidePlateRibbon();
            plateMgr.InactivePowder();
            result1.SetActive(true);
            result2.SetActive(true);
            result3.SetActive(true);
            plateMgr.StopExplosion();
            checklistManager.CompleteStep(stepId);
            stepId++;
        }

        if(notFill)
        {
            // Undo는 한 번만 실행
            if(checkOne)
            {
                prevProcess = nowProcess; // 현재 상태 저장
                nowProcess = 1;           // 임시 1
                checkOne = false;

                while(stepId > 1)
                {
                    checklistManager.UndoStep(stepId - 1);
                    stepId--;
                }
            }
        }
        else
        {
            // notFill 해제 시, 이전 상태로 복원 (한 번만)
            if(!checkOne && prevProcess != -1)
            {
                while(stepId < prevProcess)
                {
                    checklistManager.CompleteStep(stepId);
                    stepId++;
                }

                nowProcess = prevProcess;
                prevProcess = -1;
                checkOne = true;
            }
        }

    }
    public void OnPowderTouch(GameObject targetObj, GameObject sourceObj)
    {
        switch (nowProcess)
        {
            case 1:
            {
                if (targetObj == Progress1_Obj3)
                {
                    if (sourceObj == Progress1_Obj1)
                    {
                        plateMgr.IncreasePowderScale(sourceObj, Progress1_Obj1, Progress1_Obj2);
                        if(firstPowder)
                        {
                            PowderColor.SetSplit(1);
                            plateMgr.SetParticleColor(new Color(64f / 255f, 23f / 255f, 3f / 255f));
                            firstPowder = false;
                        }
                        
                        PowderColor.SetSplitDelta(0.01f);
                    }
                    if (sourceObj == Progress1_Obj2)
                    {
                        plateMgr.IncreasePowderScale(sourceObj, Progress1_Obj1, Progress1_Obj2);
                        if(firstPowder)
                        {
                            PowderColor.SetSplit(0);
                            plateMgr.SetParticleColor(Color.gray);
                            firstPowder = false;
                        }
                        PowderColor.SetSplitDelta(-0.01f);
                    }
                }
                break;
            }

            case 3:
            {
                if((sourceObj == Progress2_Obj1) && (targetObj == Progress2_Obj2))
                {
                    PowderColor.SetSplit(0.8f);
                    nowProcess++;
                    checklistManager.CompleteStep(stepId);
                    stepId++;
                }
                break;
            }
        }

        // if((sourceObj.name == "PowderBottle") && (targetObj.name == "Spoon"))
        // {
        //     spoonMgr.ActivePowder();
        // }
    }
    
    public void OnTouch(GameObject targetObj, GameObject sourceObj)
    {
        switch (nowProcess)
        {
            case 4:
                if ((sourceObj == Progress3_Obj1 && targetObj == Progress3_Obj2) || (sourceObj == Progress3_Obj2 && targetObj == Progress3_Obj1))
                {
                    plateMgr.ShowPlateRibbon();
                    plateMgr.HideRibbon();
                    nowProcess++;
                    checklistManager.CompleteStep(stepId);
                    stepId++;
                }
                break;

        }

        if((sourceObj.name == "Spoon") && (targetObj.name == "Powder") || (sourceObj.name == "Powder") && (targetObj.name == "Spoon"))
        {
            spoonMgr.ActivePowder();
        }
    }

    public void OnFireTouch(GameObject targetObj, GameObject sourceObj)
    {
        switch (nowProcess)
        {
            case 5:
                {
                    if(targetObj.name =="Plate")
                    {
                        StartCoroutine(plateMgr.MoveFireCoroutine(() =>
                        {
                            if(notCall){
                                nowProcess++;
                                notCall = false;
                                checklistManager.CompleteStep(stepId);
                                stepId++;
                            }
                        }));

                    }
                    
                }
                break;
        }
    }

    public void OnPickUp(GameObject item)
    {
    }
}
