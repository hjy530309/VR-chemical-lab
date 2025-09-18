using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowderShooter : MonoBehaviour
{
    void OnParticleCollision(GameObject other)
    {
        ExperimentProgress progress = FindObjectOfType<ExperimentProgress>();
        if (progress == null) return;

        // 닿은 오브젝트 탐색
        GameObject targetObj = other;
        while (targetObj != null && targetObj.tag != "ExperimentObj")
        {
            targetObj = targetObj.transform.parent != null ? targetObj.transform.parent.gameObject : null;
        }

        // 파티클을 쏜 오브젝트 탐색
        GameObject sourceObj = this.gameObject;
        while (sourceObj != null && sourceObj.tag != "ExperimentObj")
        {
            sourceObj = sourceObj.transform.parent != null ? sourceObj.transform.parent.gameObject : null;
        }

        // null 체크 후 전달
        if (targetObj != null && sourceObj != null && targetObj!=sourceObj)
        {
            progress.OnLiquidTouch(targetObj, sourceObj);
        }
    }
}
