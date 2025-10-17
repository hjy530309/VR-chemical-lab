using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class FireworkProgress : MonoBehaviour
{
    [SerializeField] private ChecklistManager checklistManager;
    [SerializeField] private int stepId =1;

    public int nowProcess = 1;
    public GameObject roof;
    public FireworkScoopManager scoopMgr;
    public FireworkShellManager shellMgr;
    public BatteryManager batteryMgr1;
    public BatteryManager batteryMgr2;
    public BatteryManager batteryMgr3;
    public GameObject glue1;
    public GameObject glue2;

    public GameObject Progress1_Obj1;
    public GameObject Progress1_Obj2;

    public GameObject Progress2_Obj1;
    public GameObject StarGem;
    public GameObject HeartGem;
    public GameObject CircleGem;

    public GameObject Progress3_Obj1;
    public GameObject Progress3_Obj2;
    public GameObject Progress4_Obj1;
    public GameObject Progress4_Obj2;
    public GameObject Progress5_Obj1;
    public GameObject Progress5_Obj2;
    public GameObject Progress5_Obj3;
    public GameObject Progress6_Obj1;
    public GameObject Progress6_Obj2;
    public GameObject Progress9_Obj1;
    public GameObject Progress9_Obj2;
    public GameObject Progress10_Obj1;
    public GameObject Progress10_Obj2;
    public GameObject FireworkSphere;
    public GameObject FireworkRod;
    public GameObject FireworkProduct;

    public ParticleSystem TorchParticle;
    public RocketManager rocket;

    public float shakeSensitivity = 1f; // 흔들림 감도
    private Vector3 lastPosition;
    private bool isFirst = true;

    void Start()
    {
        lastPosition = Progress2_Obj1.transform.position;
        glue1.SetActive(false);
        glue2.SetActive(false);
        FireworkSphere.SetActive(false);
        Progress5_Obj3.SetActive(false);
        FireworkProduct.SetActive(false);
        TorchParticle.Stop();
        roof.SetActive(false);
    }

    void Update()
    {
        // ------------------ 흔들림 감지 ------------------
        if(nowProcess == 3)
        {
            Vector3 currentPosition = Progress2_Obj1.transform.position;

            float movement = (currentPosition - lastPosition).magnitude;

            // 흔들림 정도 계산
            if (movement * shakeSensitivity > 0.5f) // 약간의 움직임 무시
            {
                // 흔들림이 감지되면 스타를 떨어뜨림
                switch(shellMgr.nowstar)
                {
                    case 0: break;
                    case 1: shellMgr.InactiveStar(); StarGem.SetActive(true); break;
                    case 2: shellMgr.InactiveHeart(); HeartGem.SetActive(true); break;
                    case 3: shellMgr.InactiveCircle(); CircleGem.SetActive(true); break;
                }
                nowProcess = 2;
                checklistManager.UndoStep(stepId-1);
                stepId--;
            }

            lastPosition = currentPosition;
        }
    }

    public void OnPowderTouch(GameObject targetObj, GameObject sourceObj)
    {
        switch (nowProcess)
        {
            case 1:
            {
                if (targetObj == Progress1_Obj2 && sourceObj == Progress1_Obj1 && scoopMgr.isGunPowder == true)
                {
                    shellMgr.IncreasePowderScale1();
                    if(shellMgr.totalScale >= 0.065f)
                    {
                        nowProcess++;
                        checklistManager.CompleteStep(stepId);
                        stepId++;
                    }
                }
                break;
            }

            case 4:
            {
                if((sourceObj == Progress3_Obj1) && (targetObj == Progress3_Obj2) && scoopMgr.isSawdustPowder == true)
                {
                    shellMgr.IncreasePowderScale2();
                    if(shellMgr.totalScale >= 0.16f)
                    {
                        nowProcess++;
                        checklistManager.CompleteStep(stepId);
                        stepId++;
                    }
                }
                break;
            }
        }
    }
    
    public void OnTouch(GameObject targetObj, GameObject sourceObj)
    {
        switch (nowProcess)
        {
            case 2:
                if (sourceObj.layer == LayerMask.NameToLayer("FireworkGem") && targetObj == Progress2_Obj1)
                {
                    if(sourceObj.name == "StarGem")
                    {
                        StarGem.SetActive(true);
                        shellMgr.ActiveStar();
                    }
                    if(sourceObj.name == "HeartGem")
                    {
                        HeartGem.SetActive(true);
                        shellMgr.ActiveHeart();
                    }
                    if(sourceObj.name == "CircleGem")
                    {
                        CircleGem.SetActive(true);
                        shellMgr.ActiveCircle();
                    }

                    nowProcess++;
                    checklistManager.CompleteStep(stepId);
                    stepId++;
                }
                break;
            case 5:
                if(sourceObj == Progress4_Obj1 && targetObj == Progress4_Obj2)
                {
                    sourceObj.SetActive(false);
                    shellMgr.ActiveWick();
                    Progress5_Obj3.SetActive(true);
                    nowProcess++;
                    checklistManager.CompleteStep(stepId);
                    stepId++;
                }
                break;
            case 7:
                if(sourceObj == Progress6_Obj1 && targetObj == Progress6_Obj2)
                {
                    Progress6_Obj1.SetActive(false);
                    Progress6_Obj2.SetActive(false);
                    FireworkSphere.SetActive(true);
                    nowProcess++;
                    checklistManager.CompleteStep(stepId);
                    stepId++;
                }
                break;
            case 8:
                if((sourceObj == FireworkSphere && targetObj == FireworkRod) || sourceObj == FireworkRod && targetObj == FireworkSphere)
                {
                    FireworkSphere.SetActive(false);
                    FireworkProduct.SetActive(true);
                    nowProcess++;
                    checklistManager.CompleteStep(stepId);
                    stepId++;
                }
                break;
        }

        if((sourceObj.name == "Scoop") && (targetObj.name == "GunpowderBox") || (sourceObj.name == "GunpowderBox") && (targetObj.name == "Scoop"))
        {
            scoopMgr.ActiveGunPowder();
        }
        if((sourceObj.name == "Scoop") && (targetObj.name == "SawdustBox") || (sourceObj.name == "SawdustBox") && (targetObj.name == "Scoop"))
        {
            scoopMgr.ActiveSawdustPowder();
        }
    }

    public void OnLiquidTouch(GameObject targetObj, GameObject sourceObj)
    {
        switch (nowProcess)
        {
            case 6:
                if(sourceObj == Progress5_Obj1 && (targetObj == Progress5_Obj2 || targetObj == Progress5_Obj3))
                {
                    if(targetObj == Progress5_Obj2)
                    {
                        glue1.SetActive(true);
                    }
                    if(targetObj == Progress5_Obj3)
                    {
                        glue2.SetActive(true);
                    }
                    nowProcess++;
                    checklistManager.CompleteStep(stepId);
                    stepId++;
                }
                break;
        }
    }

    public void OnFireTouch(GameObject targetObj, GameObject sourceObj)
    {
        switch(nowProcess)
        {
            case 10:
                if(sourceObj == Progress10_Obj1 && targetObj == Progress10_Obj2)
                {
                    float distance = Vector3.Distance(sourceObj.transform.position, targetObj.transform.position);
                    if(distance < 1f)
                    {
                        StartCoroutine(rocket.StartRocket());
                        checklistManager.CompleteStep(stepId);
                        stepId++;
                    }
                }
                break;
        }

        if(sourceObj == Progress10_Obj1 && (targetObj.name == "Battery1" || targetObj.name == "Battery2" || targetObj.name == "Battery3"))
        {
            float distance = Vector3.Distance(sourceObj.transform.position, targetObj.transform.position);
            if(distance < 1f)
            {   
                switch(targetObj.name)
                {
                    case "Battery1": batteryMgr1.StartRocketBattery(); break;
                    case "Battery2": batteryMgr2.StartRocketBattery(); break;
                    case "Battery3": batteryMgr3.StartRocketBattery(); break;
                }
            }
        }
    }

    public void rocketReady()
    {
        if ((nowProcess == 9))
        {
            if(isFirst)
            {
                rocket.ShowRocket();
                isFirst = false;
                nowProcess++;
                checklistManager.CompleteStep(stepId);
                stepId++;
            }
        }
    }

    public void OnPickUp(GameObject item)
    {
        if(item.name == "Torch")
        {
            TorchParticle.Play();
        }
        switch(item.name)
        {
            case "Battery1": batteryMgr1.StopRocketBattery(); break;
            case "Battery2": batteryMgr2.StopRocketBattery(); break;
            case "Battery3": batteryMgr3.StopRocketBattery(); break;
        }
    }

    public void DropOff(GameObject item)
    {
        if(item.name == "Torch")
        {
            TorchParticle.Stop();
        }
    }

    public void CheckChangeColor()
    {
        if(nowProcess == 3 && shellMgr.starColor != Color.white)
        {
            nowProcess++;
            checklistManager.CompleteStep(stepId);
            stepId++;
        }
    }
}
