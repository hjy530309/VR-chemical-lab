using UnityEngine;
using System.Collections.Generic;

public class PowderCollector : MonoBehaviour
{
    public ParticleSystem powderParticles;
    public Transform grainLayer; // 평면 오브젝트

    public float maxHeight = 0.12f; // 최대 높이 (y)
    public float maxSizeXZ = 0.1f; // x, z 최대 넓이 (예: 10cm)
    public int maxHits = 500;

    private int hitCount = 0;

    public float minSize = 0.01f; // 시작 크기
public float maxSize = 0.1f;  // 최대 크기 (x, z)

void OnParticleCollision(GameObject other)
{
    List<ParticleCollisionEvent> events = new List<ParticleCollisionEvent>();
    int num = powderParticles.GetCollisionEvents(gameObject, events);
    hitCount += num;

    float t = Mathf.Clamp01((float)hitCount / maxHits);

    // 1. 두께 (y축) 늘리기
    Vector3 scale = grainLayer.localScale;
    scale.y = Mathf.Lerp(0.01f, maxHeight, t);

    // 2. 가로/세로(x, z)는 일정 범위 안에서만 커지게
    float limitedXZ = Mathf.Lerp(minSize, maxSize, t);
    scale.x = scale.z = limitedXZ;

    grainLayer.localScale = scale;

    // 3. 바닥 기준으로 위로 쌓이게 보이기
    Vector3 pos = grainLayer.localPosition;
    pos.y = scale.y / 2f;
    grainLayer.localPosition = pos;
}

}
