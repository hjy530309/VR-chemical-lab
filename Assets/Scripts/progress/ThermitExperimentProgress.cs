using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermitExperimentProgress : MonoBehaviour
{
    public int nowProcess = 1;
    public ThermitSpoonManager spoonMgr;
    public ThermitPlateManager plateMgr;
    public SplitController PowderColor;
    private bool firstPowder = true;

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
                            firstPowder = false;
                        }
                        PowderColor.SetSplitDelta(-0.01f);
                    }

                    Vector2 ratio = plateMgr.GetRatio();

                    if (ratio.x > 0f && ratio.y > 0f)
                    {
                        float actualRatio = ratio.y / ratio.x; // Obj2 / Obj1
                        if (Mathf.Abs(actualRatio - 0.3f) <= 0.2f) // 오차 ±0.2 허용
                        {
                            nowProcess++;
                        }
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
