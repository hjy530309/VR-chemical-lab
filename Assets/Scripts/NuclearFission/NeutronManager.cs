using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NeutronManager : MonoBehaviour
{
    public static NeutronManager Instance;

    [Header("Neutron 설정")]
    public GameObject neutronPrefab;
    public int maxNeutrons = 200;
    public float initialSpeed = 5f;

    [Header("ControlRod")]
    public ControlRod controlRod;

    [Header("UI")]
    public TMP_Text dangerText;       // 위험도 표시
    public int dangerThreshold = 50; // 위험 임계값

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
        //if (count >= dangerThreshold && !isOverThreshold)
        //{
        //    isOverThreshold = true;
        //    StopAllNeutrons();
        //}
        //else if (count < dangerThreshold)
        //{
        //    isOverThreshold = false;
        //}
        if (count >= dangerThreshold)
        {
            if (ExperimentUI.Instance != null)
                ExperimentUI.Instance.ShowFail();
        }
    }

    //void StopAllNeutrons()
    //{
    //    foreach (var n in neutrons)
    //    {
    //        if (n != null)
    //            n.ModifySpeed(0f); // 속도를 0으로
    //    }

    //    if (dangerText != null)
    //        dangerText.text += " - DANGER! Chain Reaction STOPPED!";
    //}

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
