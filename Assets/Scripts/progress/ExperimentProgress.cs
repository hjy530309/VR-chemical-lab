using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentProgress : MonoBehaviour
{
    public int nowProcess = 1;
    int prevProcess = 1;
    public bool prevSuccess = true;
    public ParticleSystem BubbleParticles;
    public SpoonManager spoonManager;
    public GameObject Progress1_Obj1;
    public GameObject Progress1_Obj2;
    public GameObject Progress2_Obj1;
    public GameObject Progress2_Obj2;
    public GameObject Progress3_Obj1;
    public GameObject Progress3_Obj2;

    // void Update(){
    //     LiquidVolumeAnimator P1O2 = Progress1_Obj2.GetComponentInChildren<LiquidVolumeAnimator>();
    //     if((P1O2.level > 0.2f || P1O2.level < 0.1f) && nowProcess >= 2 && prevSuccess == true)
    //     {
    //         prevProcess = nowProcess;
    //         nowProcess = 1;
    //     }else
    //     {
    //         prevSuccess = false;
    //     }
    //     if(!(P1O2.level > 0.2f || P1O2.level < 0.1f) && (prevSuccess == false))
    //     {
    //         prevSuccess = true;
    //         nowProcess = prevProcess;
    //     }
    // }
    void Start()
    {
        BubbleParticles.Stop();
    }


    public void OnLiquidTouch(GameObject targetObj, GameObject sourceObj)
    {
        // 자식의 자식에 있는 LiquidVolumeAnimator 가져오기
        LiquidVolumeAnimator target = targetObj.GetComponentInChildren<LiquidVolumeAnimator>();
        
        switch (nowProcess)
        {
            case 1:
               if ((sourceObj == Progress1_Obj1 || sourceObj == Progress1_Obj2) &&
                (targetObj == Progress1_Obj1 || targetObj == Progress1_Obj2))
                {
                    if (target != null && target.level >= 0.1f)
                    {
                        nowProcess++;
                    }

                }
                break;

            case 2:
                if (sourceObj == Progress2_Obj1 || targetObj == Progress2_Obj2)
                {
                    SpoonManager spoonMgr = sourceObj.GetComponent<SpoonManager>();

                    if (spoonMgr != null && spoonMgr.liquid.activeSelf)
                    {
                        nowProcess++;
                    }
                }
                break;
        }

        if((sourceObj.name == "Fairy") && (targetObj.name == "Spoon"))
        {
            spoonManager.ActiveLiquid();
        }
    }

    public void OnPowderTouch(GameObject targetObj, GameObject sourceObj)
    {
        switch (nowProcess)
        {
            case 3:
            {
                SpoonManager spoonMgr = sourceObj.GetComponent<SpoonManager>();
                if (spoonMgr != null && spoonMgr.hasPowder)
                {
                    if (sourceObj == Progress3_Obj1 || targetObj == Progress3_Obj2)
                    {
                        nowProcess++;
                        BubbleParticles.Play();
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
            // case 3:
            //     if ((sourceObj == Progress2_Obj1 && targetObj == Progress2_Obj2) || (sourceObj == Progress2_Obj2 && targetObj == Progress2_Obj1))
            //     {
            //         nowProcess++;
            //     }
            //     break;

        }
        if((sourceObj.name == "Spoon") && (targetObj.name == "Powder"))
        {
            spoonManager.ActivePowder();
        }
        // if((sourceObj.name == "Spoon" && targetObj.name == "Fairy") || (sourceObj.name == "Fairy" && targetObj.name == "Spoon"))
        // {
        //     SpoonManager.ActiveLiquid();
        // }
    }

    public void OnPickUp(GameObject item)
    {
    }
}
