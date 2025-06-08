using UnityEngine;
using System.Collections.Generic;

public class PowderCollector : MonoBehaviour
{
    public ParticleSystem powderParticles1;
    public ParticleSystem powderParticles2;
    public ParticleSystem powderParticles3;

    public Transform grainLayer;

    public Renderer grainRenderer; // grainLayer에 할당된 MeshRenderer
    public float powderNum = 500f;
    public float plusYpos = 0.0002f;
    public float step = 0.0005f;
    public float maxHeight = 0.12f;

    public Vector3 startPosition = Vector3.zero;

    private float collisionCounter1 = 0f;
    private float collisionCounter2 = 0f;
private float collisionCounter3 = 0f;

    private float current1 = 0f;
    private float current2 = 0f;
    private float current3 = 0f;


    private Color baseColor = new Color(122f / 255f, 64f / 255f, 64f / 255f);
    private Color targetColor = new Color(150f / 255f, 94f / 255f, 94f / 255f);
    private Color colorByParticle3 = new Color(158f / 255f, 113f / 255f, 113f / 255f);



    void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> events = new List<ParticleCollisionEvent>();
        int num = 0;

        if (other == powderParticles1.gameObject)
        {
            num = powderParticles1.GetCollisionEvents(gameObject, events);
            if (num <= 0) return;

            collisionCounter1 += num;

            if (collisionCounter1 >= powderNum)
            {
                int steps = Mathf.FloorToInt(collisionCounter1 / powderNum);
                current1 += step * steps;
                current1 = Mathf.Min(current1, maxHeight * 0.75f);
                collisionCounter1 %= powderNum;
            }
        }
        else if (other == powderParticles2.gameObject)
        {
            num = powderParticles2.GetCollisionEvents(gameObject, events);
            if (num <= 0) return;

            collisionCounter2 += num;

            if (collisionCounter2 >= powderNum)
            {
                int steps = Mathf.FloorToInt(collisionCounter2 / powderNum);
                current2 += step * steps;
                current2 = Mathf.Min(current2, maxHeight * 0.25f);
                collisionCounter2 %= powderNum;
            }
        }else if (other == powderParticles3.gameObject)
            {
                num = powderParticles3.GetCollisionEvents(gameObject, events);
                if (num <= 0) return;

                collisionCounter3 += num;

                if (collisionCounter3 >= powderNum)
                {
                    int steps = Mathf.FloorToInt(collisionCounter3 / powderNum);
                    current3 += step * steps;
                    current3 = Mathf.Min(current3, maxHeight); // full influence for color
                    collisionCounter3 %= powderNum;
                }
            }

        float totalHeight = current1 + current2;
        totalHeight = Mathf.Min(totalHeight, maxHeight);

        // 스케일 & 위치
        grainLayer.localScale = new Vector3(totalHeight, totalHeight, totalHeight);
        Vector3 pos = startPosition;
        pos.y += totalHeight + plusYpos;
        grainLayer.localPosition = pos;

        // 색상 변화: 파티클2의 비율에 따라 색 보간
        float ratio = Mathf.Clamp01(current2 / (maxHeight * 0.25f)); // 0 ~ 1
        Color currentColor = Color.Lerp(baseColor, targetColor, ratio);

        grainRenderer.material.color = currentColor;
    }
}
