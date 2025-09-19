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
    public Color newTopColor = Color.green;
    public Renderer targetRenderer;
    private MaterialPropertyBlock mpb;

    public Color shakeColor = new Color(1f, 0.5f, 0f); // 주황색
    public float shakeSensitivity = 1f; // 흔들림 감도
    private Vector3 lastPosition;

    public ParticleSystem smashParticle; // 깨지는 파티클
    public Color smashParticleColor1 = new Color(1f, 0f, 0f, 1f);
    public Color smashParticleColor2 = new Color(1f, 0.5f, 0f);

    void Start()
    {
        BubbleParticles.Stop();
        mpb = new MaterialPropertyBlock();

        // 기존 프로퍼티 가져오기
        targetRenderer.GetPropertyBlock(mpb);
        lastPosition = Progress2_Obj2.transform.position;
    }
    void Update()
    {
        if(nowProcess == 5)
        {
            BubbleParticles.Play();
            nowProcess++;
        }

        // ------------------ 흔들림 감지 ------------------
        if(nowProcess == 3)
        {
            Vector3 currentPosition = Progress2_Obj2.transform.position;

            float movement = (currentPosition - lastPosition).magnitude;
            // 흔들림 정도 계산
            if (movement * shakeSensitivity > 0.5f) // 약간의 움직임 무시
            {
                // 흔들림이 감지되면 주황색으로 변경
                mpb.SetColor("_GradientTopColor", shakeColor);
                mpb.SetColor("_GradientBottomColor", shakeColor);
                targetRenderer.SetPropertyBlock(mpb);
                nowProcess++;
            }

            lastPosition = currentPosition;
        }

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
                        if (smashParticle == null) return;
                        var main = smashParticle.main;
                        main.startColor = smashParticleColor1;

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
                        // 액체의 TopColor만 변경
                        mpb.SetColor("_GradientTopColor", newTopColor);
                        // 적용
                        targetRenderer.SetPropertyBlock(mpb);
                        
                        // 깨지는 색 변경
                        if (smashParticle == null) return;
                        var main = smashParticle.main;
                        main.startColor = smashParticleColor2;

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
            case 4:
            {
                SpoonManager spoonMgr = sourceObj.GetComponent<SpoonManager>();
                if (spoonMgr != null && spoonMgr.hasPowder)
                {
                    if (sourceObj == Progress3_Obj1 || targetObj == Progress3_Obj2)
                    {
                        nowProcess++;
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
