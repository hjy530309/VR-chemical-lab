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
    bool hasPickedUp = false;
    public GameObject nowObj;

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
        DebugLog($"{gameObject.name} ungrabbed");
        hasPickedUp = false;
    }

    void OnDidGrab(NetworkGrabber newGrabber)
    {
        DebugLog($"{gameObject.name} grabbed by {newGrabber.Object.InputAuthority} {newGrabber.hand.side} hand");
        Debug.Log($"{transform.root.tag} tag from root");
        Debug.Log($"{gameObject.tag} tag name");
        if (progressManager != null && hasPickedUp==false)
        {
            // Grab 된 이 게임오브젝트를 Progress에 전달해서 처리하게 함
            progressManager.OnPickUp(this.gameObject);
            nowObj = this.gameObject;
            hasPickedUp = true;
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
