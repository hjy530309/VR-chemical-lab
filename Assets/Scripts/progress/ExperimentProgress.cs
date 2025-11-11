using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentProgress : MonoBehaviour
{
    [SerializeField] private ExperimentFeedbackManager feedbackManager;

    [SerializeField] private ChecklistManager checklistManager;
    [SerializeField] private int stepId =1;

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

    public Color newTopColor = Color.green;
    public Renderer targetRenderer;
    private MaterialPropertyBlock mpb;

    public Color shakeColor = new Color(1f, 0.5f, 0f); // 주황색
    public float shakeSensitivity = 1f; // 흔들림 감도
    private Vector3 lastPosition;

    public ParticleSystem smashParticle; // 깨지는 파티클
    public Color smashParticleColor1 = new Color(1f, 0f, 0f, 1f);
    public Color smashParticleColor2 = new Color(1f, 0.5f, 0f);

    public AudioSource resultAudio;
    private ParticleSystemRenderer psRenderer;
    private Material instantiatedMaterial;

    void Start()
    {
        BubbleParticles.Stop();
        mpb = new MaterialPropertyBlock();

        // 기존 프로퍼티 가져오기
        targetRenderer.GetPropertyBlock(mpb);
        lastPosition = Progress2_Obj2.transform.position;

        instantiatedMaterial = new Material(psRenderer.material);
        psRenderer.material = instantiatedMaterial;
    }

    void Awake()
    {
        if (checklistManager == null)
        {
            checklistManager = FindObjectOfType<ChecklistManager>();
            if(checklistManager == null)
                Debug.LogError("ChecklistManager를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        if(nowProcess == 5)
        {
            LiquidVolumeAnimator target = Progress1_Obj2.GetComponentInChildren<LiquidVolumeAnimator>();
        
            float gravity = Mathf.Lerp(3f, 1f, target.level); 
            var main = BubbleParticles.main;
            main.gravityModifier = gravity;

            resultAudio.Play();
            BubbleParticles.Play();
            nowProcess++;
            checklistManager.CompleteStep(stepId);
            stepId++;
        }

        // ------------------ 흔들림 감지 ------------------
        if(nowProcess == 3)
        {
            Vector3 currentPosition = Progress2_Obj2.transform.position;

            float movement = (currentPosition - lastPosition).magnitude;
            
            Rigidbody rb = Progress2_Obj2.GetComponent<Rigidbody>();
            float velocity = (rb != null) ? rb.velocity.magnitude : 0f;

            // 흔들림 정도 계산
            if (movement * shakeSensitivity > 0.5f && velocity > 0.01f) // 약간의 움직임 무시
            {
                // 흔들림이 감지되면 주황색으로 변경
                mpb.SetColor("_GradientTopColor", shakeColor);
                mpb.SetColor("_GradientBottomColor", shakeColor);
                targetRenderer.SetPropertyBlock(mpb);
                nowProcess++;
                checklistManager.CompleteStep(stepId);
                stepId++;
            }

            lastPosition = currentPosition;
        }

        LiquidVolumeAnimator flask = Progress2_Obj2.GetComponentInChildren<LiquidVolumeAnimator>();
        switch(nowProcess)
        {
            case 1:
                if(flask != null && flask.level >= 0.3 && flask.level <= 0.6)
                {
                    nowProcess = 2;
                    checklistManager.CompleteStep(stepId);
                    stepId++;
                    feedbackManager.CheckAmount(Progress2_Obj2.name, flask.level);
                }
                break;
            case 2:
                if(flask != null && flask.level <0.3 || flask.level > 0.6)
                {
                    nowProcess = 1;
                    checklistManager.UndoStep(stepId-1);
                    stepId--;
                    feedbackManager.CheckAmount(Progress2_Obj2.name, flask.level);
                }
                break;
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
                    if (target != null && target.level != 0f)
                    {
                        if (smashParticle == null) return;
                        var main = smashParticle.main;
                        main.startColor = smashParticleColor1;
                    }

                }
                break;

            case 2:
                if ((sourceObj == Progress2_Obj1 || sourceObj.name == "Fairy") && targetObj == Progress2_Obj2)
                {
                    SpoonManager spoonMgr = sourceObj.GetComponent<SpoonManager>();

                    if ((spoonMgr != null && spoonMgr.liquid.activeSelf) || sourceObj.name == "Fairy")
                    {
                        // 액체의 TopColor만 변경
                        mpb.SetColor("_GradientTopColor", newTopColor);
                        // 적용
                        targetRenderer.SetPropertyBlock(mpb);
                        
                        // 깨지는 색 변경
                        if (smashParticle == null) return;
                        var main = smashParticle.main;
                        main.startColor = smashParticleColor2;

                        if (instantiatedMaterial != null)
                        {
                            instantiatedMaterial.color = smashParticleColor2;  // Material 색상 변경
                        }

                        nowProcess++;
                        checklistManager.CompleteStep(stepId);
                        stepId++;
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

    //양이 적절한지 확인
    void CheckLiquid(GameObject obj, float amount)
    {
        feedbackManager.CheckAmount(obj.name, amount);
    }
}
