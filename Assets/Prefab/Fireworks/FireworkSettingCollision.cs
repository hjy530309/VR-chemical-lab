using UnityEngine;

public class FireworkSettingCollision : MonoBehaviour
{
    public FireworkProgress progress;
    void OnTriggerEnter(Collider other)
    {
        GameObject taggedParent = FindTaggedParent(other.gameObject, "ExperimentObj");

        if(taggedParent.gameObject.name == "rocket")
        {
            progress.settingReady = true;
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
