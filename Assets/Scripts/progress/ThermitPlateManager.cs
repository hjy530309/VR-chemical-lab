using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermitPlateManager : MonoBehaviour
{
    public GameObject powder;
    public GameObject ribbon;
    public GameObject plateRibbon;
    public ParticleSystem fireParticle;
    public ParticleSystem explosionParticle;
    public GameObject explosion;
    public float scaleStep = 0.0002f; // 호출할 때마다 얼마나 늘릴지
    public float maxScaleObj1 = 0.1f;
    public float maxScaleObj2 = 0.03f;
    private float obj1Scale = 0f;
    private float obj2Scale  = 0f;

    public float duration = 5f;   // 내려가는 시간

    void Start()
    {
        powder.SetActive(false);
        powder.transform.localScale = Vector3.zero;
        StopFire();
        HidePlateRibbon();
        StopExplosion();
    }

    public void PlayFire()
    {
        fireParticle.Play();
    }
    public void StopFire()
    {
        fireParticle.Stop();
    }
    public void PlayExplosion()
    {
        explosion.SetActive(true);
        explosionParticle.Play();
    }
    public void StopExplosion()
    {
        explosion.SetActive(false);
        explosionParticle.Stop();
    }
    public void ActivePowder()
    {
        powder.SetActive(true);
    }

    public void InactivePowder()
    {
        powder.SetActive(false);
    }

    public float GetCurrentScale()
    {
        return powder.transform.localScale.x; // uniform scale이라 x 하나만 리턴
    }

    public void IncreasePowderScale(GameObject sourceObj, GameObject obj1, GameObject obj2)
    {
        powder.SetActive(true);
        if (sourceObj == obj1)
        {
            obj1Scale = Mathf.Min(obj1Scale + scaleStep, maxScaleObj1);
        }
        else if (sourceObj == obj2)
        {
            obj2Scale = Mathf.Min(obj2Scale + scaleStep, maxScaleObj2);
        }

        float totalScale = obj1Scale + obj2Scale; 
        powder.transform.localScale = Vector3.one * totalScale;
    }

    // 비율 계산
    public Vector2 GetRatio()
    {
        if (obj2Scale  == 0f) return new Vector2(1f, 0f);
        return new Vector2(obj1Scale , obj2Scale );
    }

    public void ShowRibbon()
    {
        ribbon.SetActive(true);
    }
    public void HideRibbon()
    {
        ribbon.SetActive(false);
    }
    public void ShowPlateRibbon()
    {
        plateRibbon.SetActive(true);
    }
    public void HidePlateRibbon()
    {
        plateRibbon.SetActive(false);
    }

    public IEnumerator MoveFireCoroutine(System.Action onComplete = null)
    {
        PlayFire();
        yield return new WaitForSeconds(5f);

        StopFire();
        PlayExplosion();
        yield return new WaitForSeconds(2f);
        StopExplosion();

        onComplete?.Invoke();
    }


}
