using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkShellManager : MonoBehaviour
{
    public FireworkProgress progress;
    public GameObject gunpowder;
    public GameObject sawdustpowder;
    public GameObject star;
    public Renderer starRenderer;
    public Renderer heartRenderer;
    public Renderer circleRenderer;
    public GameObject heart;
    public GameObject circle;
    public GameObject wick;
    public Color starColor = Color.white;
    public ParticleColorModifier particleModifier;
    public int nowstar = 0;
    bool firstGunPowder = true;
    bool firstSawdustPowder = true;

    public float totalScale = 0f;
    public float scaleStep = 0.001f; // 호출할 때마다 얼마나 늘릴지
    public float maxScaleObj1 = 0.07f;
    public float maxScaleObj2 = 0.16f;
    private float obj1Scale = 0f;
    private float obj2Scale  = 0f;

    public float duration = 5f;   // 내려가는 시간

    void Start()
    {
        InactiveGunPowder();
        InactiveSawdustPowder();
        InactiveStar();
        InactiveHeart();
        InactiveCircle();
        wick.SetActive(false);
        gunpowder.transform.localScale = Vector3.zero;
    }

    public void ActiveGunPowder()
    {
        gunpowder.SetActive(true);
    }

    public void InactiveGunPowder()
    {
        gunpowder.SetActive(false);
    }

    public void ActiveSawdustPowder()
    {
        sawdustpowder.SetActive(true);
    }
    public void InactiveSawdustPowder()
    {
        sawdustpowder.SetActive(false);
    }

    public void ActiveStar()
    {
        nowstar = 1;
        InactiveHeart();
        InactiveCircle();
        star.SetActive(true);
    }
    public void InactiveStar()
    {
        star.SetActive(false);
    }

    public void ActiveHeart()
    {
        nowstar = 2;
        InactiveStar();
        InactiveCircle();
        heart.SetActive(true);
    }
    public void InactiveHeart()
    {
        heart.SetActive(false);
    }

    public void ActiveCircle()
    {
        nowstar = 3;
        InactiveStar();
        InactiveHeart();
        circle.SetActive(true);
    }
    public void InactiveCircle()
    {
        circle.SetActive(false);
    }

    public void ActiveWick()
    {
        wick.SetActive(true);
    }
    public float GetCurrentScale()
    {
        return gunpowder.transform.localScale.x; // uniform scale이라 x 하나만 리턴
    }

    public void IncreasePowderScale1()
    {
        if (firstGunPowder)
        {
            ActiveGunPowder();
            firstGunPowder = false; // 한 번만 실행되게
        }

        obj1Scale = Mathf.Min(obj1Scale + scaleStep, maxScaleObj1);
        gunpowder.transform.localScale = Vector3.one * obj1Scale;
        totalScale = totalScale + scaleStep;
    }

    public void IncreasePowderScale2()
    {
        if (firstSawdustPowder)
        {
            InactiveGunPowder();
            ActiveSawdustPowder();

            // gunpowder 현재 스케일을 sawdustpowder에 복사
            obj2Scale = obj1Scale;
            sawdustpowder.transform.localPosition = gunpowder.transform.localPosition;

            firstSawdustPowder = false; // 한 번만 실행되게
        }

        obj2Scale = Mathf.Min(obj2Scale + scaleStep, maxScaleObj2);
        sawdustpowder.transform.localScale = new Vector3(
            obj2Scale,
            sawdustpowder.transform.localScale.y,
            obj2Scale
        );
        totalScale = totalScale + scaleStep;

        float maxY = 0.7f;
        float ratio = obj2Scale / maxScaleObj2; // 현재 스케일 비율
        float newY = ratio * maxY;

        sawdustpowder.transform.localPosition = new Vector3(
            sawdustpowder.transform.localPosition.x,
            newY,
            sawdustpowder.transform.localPosition.z
        );

    }

    public void ChangeStarColor(Color color)
    {
        starRenderer.material.color = color;
        heartRenderer.material.color = color;
        circleRenderer.material.color = color;
        particleModifier.ChangeMiddleColor(color);
        progress.CheckChangeColor();
    }
}
