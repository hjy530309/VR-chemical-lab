using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NeutronManager : MonoBehaviour
{
    public static NeutronManager Instance;

    [Header("Neutron 설정")]
    public GameObject neutronPrefab;
    public int maxNeutrons = 50;
    public float initialSpeed = 5f;

    [Header("ControlRod")]
    public ControlRod controlRod;

    [Header("UI")]
    public TMP_Text dangerText;       // 위험도 표시
    public int dangerThreshold = 20; // 위험 임계값

    private List<Neutron> neutrons = new List<Neutron>();
    private bool isOverThreshold = false;

    void Awake() => Instance = this;

    void Update()
    {
        // 위험도 표시
        int count = neutrons.Count;
        if (dangerText != null)
            dangerText.text = $"Neutrons: {count}";

        // 일정 수 이상이면 모든 중성자 멈춤
        if (count >= dangerThreshold && !isOverThreshold)
        {
            isOverThreshold = true;
            StopAllNeutrons();
        }
        else if (count < dangerThreshold)
        {
            isOverThreshold = false;
        }
    }

    void StopAllNeutrons()
    {
        foreach (var n in neutrons)
        {
            if (n != null)
                n.ModifySpeed(0f); // 속도를 0으로
        }

        if (dangerText != null)
            dangerText.text += " - DANGER! Chain Reaction STOPPED!";
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

    public void SpawnNeutron(Vector3 pos, float speed = 3f)
    {
        if (neutrons.Count >= maxNeutrons) return;

        Vector3 offset = Random.onUnitSphere * 0.2f;
        GameObject n = Instantiate(neutronPrefab, pos + offset, Random.rotation);

        Rigidbody rb = n.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.velocity = Random.onUnitSphere * speed;
        }

        Neutron neutronScript = n.GetComponent<Neutron>();
        if (neutronScript != null)
            RegisterNeutron(neutronScript);
    }

    public List<Neutron> GetNeutrons()
    {
        neutrons.RemoveAll(n => n == null);
        return neutrons;
    }
}


/*using System.Collections.Generic;
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

    public void SpawnNeutron(Vector3 pos, float speed = 3f)
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
            rb.velocity = Random.onUnitSphere * speed;
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
}*/
