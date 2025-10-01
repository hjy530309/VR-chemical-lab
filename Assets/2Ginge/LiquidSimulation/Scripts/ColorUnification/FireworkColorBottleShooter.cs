using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkColorBottleShooter : MonoBehaviour
{
    public Renderer liquidRenderer;        // 파티클의 액체 색을 가진 렌더러
    public FireworkShellManager shellMgr;      // 저장할 대상 스크립트 (다른 오브젝트에 붙어있음)
    public ParticleSystem powderParticle;


    void OnParticleCollision(GameObject other)
    {
        // 현재 파티클의 색 가져오기
        Color currentColor = liquidRenderer.material.color;

        GameObject targetObj = other;
        while (targetObj != null && targetObj.tag != "ExperimentObj")
        {
            targetObj = targetObj.transform.parent != null ? targetObj.transform.parent.gameObject : null;
        }

        // 다른 스크립트에 저장
        if (shellMgr != null && targetObj != null && targetObj.name == "Shell1")
        {
            shellMgr.starColor = currentColor;
            shellMgr.ChangeStarColor(currentColor);
        }

        powderParticle.GetComponent<ParticleSystemRenderer>().material.color = currentColor;

    }
}
