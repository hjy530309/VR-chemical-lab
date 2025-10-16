using System.Collections.Generic;
using UnityEngine;

public class NeutronManager : MonoBehaviour
{
    public static NeutronManager Instance;

    [Header("Neutron 설정")]
    public GameObject neutronPrefab;
    public int maxNeutrons = 50;
    public float initialSpeed = 5f; // 초기 속도를 체감하기 좋게 증가

    [Header("ControlRod")]
    public ControlRod controlRod;

    private List<Neutron> neutrons = new List<Neutron>();

    void Awake() => Instance = this;

    void Update()
    {
        //float slowdown = controlRod != null ? controlRod.controlLevel : 0f;
        //float speedMultiplier = Mathf.Lerp(1f, 0.1f, slowdown); // 속도 10%~100% 적용

        // 역순 순회 + 안전 처리
        //for (int i = neutrons.Count - 1; i >= 0; i--)
        //{
        //    Neutron n = neutrons[i];
        //    if (n == null)
        //    {
        //        neutrons.RemoveAt(i);
        //        continue;
        //    }

        //    Rigidbody rb = n.GetComponent<Rigidbody>();
        //    if (rb != null)
        //    {
        //        rb.velocity = rb.velocity.normalized * initialSpeed * speedMultiplier;

        //        // 시각적 체감: 속도에 따라 색상 변경
        //        Renderer rend = n.GetComponent<Renderer>();
        //        if (rend != null)
        //        {
        //            rend.material.color = Color.Lerp(Color.red, Color.yellow, speedMultiplier);
        //        }
        //    }
        //}
    }

    public void RegisterNeutron(Neutron n)
    {
        if (!neutrons.Contains(n))
            neutrons.Add(n);

        if (neutrons.Count > maxNeutrons)
            neutrons[0].DestroySelf();
    }

    public void UnregisterNeutron(Neutron n)
    {
        if (neutrons.Contains(n))
            neutrons.Remove(n);
    }

    public void SpawnNeutron(Vector3 pos)
    {
        if (neutrons.Count >= maxNeutrons) return;

        Vector3 offset = Random.onUnitSphere * 0.2f; // FuelPellet 중심 피하기
        GameObject n = Instantiate(neutronPrefab, pos + offset, Random.rotation);

        Rigidbody rb = n.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            //float speedMultiplier = controlRod != null ? Mathf.Lerp(1f, 0.1f, controlRod.controlLevel) : 1f;
            //rb.velocity = Random.onUnitSphere * initialSpeed * speedMultiplier;
        }

        Neutron neutronScript = n.GetComponent<Neutron>();
        if (neutronScript != null)
            RegisterNeutron(neutronScript);
    }

    // 외부에서 중성자 리스트 가져오기
    public List<Neutron> GetNeutrons()
    {
        // 안전하게 null 제거
        neutrons.RemoveAll(n => n == null);
        return neutrons;
    }
}
