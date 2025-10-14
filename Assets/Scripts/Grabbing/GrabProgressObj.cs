using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using Fusion.XR.Host.Grabbing;

[RequireComponent(typeof(NetworkGrabbable))]
public class GrabProgressObj : NetworkBehaviour
{
    public TextMeshProUGUI authorityText;
    public TextMeshProUGUI debugText;
    public ExperimentProgress progressManager;
    private ThermitExperimentProgress thermitProgress;
    private FireworkProgress fireworkProgress;
    public PlanetExperimentProgress planetProgress;
    bool hasPickedUp = false;
    public GameObject nowObj;

    private TorchEffectController torchEffect;

    private void Awake()
    {
        debugText.text = "";
        var grabbable = GetComponent<NetworkGrabbable>();
        grabbable.onDidGrab.AddListener(OnDidGrab);
        grabbable.onDidUngrab.AddListener(OnDidUngrab);
    }

    private void DebugLog(string debug)
    {
        debugText.text = debug;
        Debug.Log(debug);
    }

    private void UpdateStatusCanvas()
    {
        if (Object.HasInputAuthority)
            authorityText.text = "You have the input authority on this object";
        else
            authorityText.text = "You have NOT the input authority on this object";

        if (Object.HasStateAuthority)
            authorityText.text += "\nYou have the state authority on this object";
        else
            authorityText.text += "\nYou have NOT the state authority on this object";
    }

    public override void FixedUpdateNetwork()
    {
        UpdateStatusCanvas();
    }

    void OnDidUngrab()
    {
        if(this.gameObject.name == "Torch")
        {
            torchEffect.DisableEffect();
        }

        fireworkProgress = FindObjectOfType<FireworkProgress>();
        if (fireworkProgress != null)
        {
            fireworkProgress.DropOff(this.gameObject);
        }

        DebugLog($"{gameObject.name} ungrabbed");
        hasPickedUp = false;
    }

    void OnDidGrab(NetworkGrabber newGrabber)
    {
        if(this.gameObject.name == "Torch")
        {
            torchEffect = this.gameObject.GetComponent<TorchEffectController>();
            if(torchEffect != null)
            {
                torchEffect.EnableEffect(); // 파티클 실행
            }
        }
        DebugLog($"{gameObject.name} grabbed by {newGrabber.Object.InputAuthority} {newGrabber.hand.side} hand");
        Debug.Log($"{transform.root.tag} tag from root");
        Debug.Log($"{gameObject.tag} tag name");
        progressManager = FindObjectOfType<ExperimentProgress>();
        thermitProgress = FindObjectOfType<ThermitExperimentProgress>();
        fireworkProgress = FindObjectOfType<FireworkProgress>();
        planetProgress = FindObjectOfType<PlanetExperimentProgress>();
        
        if (progressManager != null && hasPickedUp==false)
        {
            GameObject obj1 = progressManager.gameObject;
            if(obj1.activeInHierarchy)
            {
                // Grab 된 이 게임오브젝트를 Progress에 전달해서 처리하게 함
                progressManager.OnPickUp(this.gameObject);
                nowObj = this.gameObject;
                hasPickedUp = true;
            }
        }
        if (thermitProgress != null && hasPickedUp==false)
        {
            GameObject obj2 = thermitProgress.gameObject;
            if(obj2.activeInHierarchy)
            {
                // Grab 된 이 게임오브젝트를 Progress에 전달해서 처리하게 함
                thermitProgress.OnPickUp(this.gameObject);
                nowObj = this.gameObject;
                hasPickedUp = true;
            }
        }
        if (fireworkProgress != null && hasPickedUp==false)
        {
            GameObject obj3 = fireworkProgress.gameObject;
            if(obj3.activeInHierarchy)
            {
                // Grab 된 이 게임오브젝트를 Progress에 전달해서 처리하게 함
                fireworkProgress.OnPickUp(this.gameObject);
                nowObj = this.gameObject;
                hasPickedUp = true;
            }
        }
        if (planetProgress != null && hasPickedUp==false)
        {
            GameObject obj3 = planetProgress.gameObject;
            if(obj3.activeInHierarchy)
            {
                // Grab 된 이 게임오브젝트를 Progress에 전달해서 처리하게 함
                planetProgress.OnPickUp(this.gameObject);
                nowObj = this.gameObject;
                hasPickedUp = true;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject taggedParent = FindTaggedParent(collision.gameObject, "ExperimentObj");
        if (taggedParent != null)
        {
            if (taggedParent.CompareTag("ExperimentObj"))
            {
                if (progressManager != null)
                {
                    progressManager.OnTouch(taggedParent.gameObject, nowObj);
                }
                if (thermitProgress != null)
                {
                    thermitProgress.OnTouch(taggedParent.gameObject, nowObj);
                }
                if (fireworkProgress != null)
                {
                    fireworkProgress.OnTouch(taggedParent.gameObject, nowObj);
                }
                if (planetProgress != null)
                {
                    planetProgress.OnTouch(taggedParent.gameObject, nowObj);
                }
            }
        }
    }


    GameObject FindTaggedParent(GameObject obj, string targetTag)
{
    Transform current = obj.transform;

    while (current != null)
    {
        if (current.CompareTag(targetTag))
        {
            return current.gameObject;
        }
        current = current.parent;
    }

    return null; // 찾지 못했을 경우
}

}
