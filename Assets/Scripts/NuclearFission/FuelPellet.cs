using System.Collections;
using UnityEngine;


public class FuelPellet : MonoBehaviour
{
    [Header("연쇄 반응 설정")]
    public int maxSpawnPerFission = 2;         // 한 번 핵분열 시 생성할 Neutron 수
    public float neutronSpeed = 5f;            // 기본 Neutron 속도
    public GameObject neutronPrefab;           // Neutron Prefab
    public ControlRod controlRod;              // Scene에서 연결 (Control Rod)

    [Header("에너지 & 효과")]
    public int energyPerFission = 3;          // 한 번 핵분열 시 생성 에너지
    public ParticleSystem fissionEffect;       // 파티클 Prefab

    private bool hasFissioned = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    public void Fission()
    {
        if (hasFissioned) return;
        hasFissioned = true;

        // 1 파티클 효과
        if (fissionEffect != null)
            Instantiate(fissionEffect, transform.position, Quaternion.identity);

        // 2. Neutron 생성
        for (int i = 0; i < maxSpawnPerFission; i++)
        {
            Vector3 offset = Random.onUnitSphere * 0.25f; // FuelPellet 중심 겹치지 않도록
            GameObject n = Instantiate(neutronPrefab, transform.position + offset, Random.rotation);

            Rigidbody nRb = n.GetComponent<Rigidbody>();
            nRb.useGravity = false;
            nRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            nRb.interpolation = RigidbodyInterpolation.Interpolate;
            nRb.velocity = Random.onUnitSphere * neutronSpeed; // 생성 즉시 움직이도록
            nRb.drag = 0;

            // NeutronManager 등록
            Neutron neutronScript = n.GetComponent<Neutron>();
            if (neutronScript != null)
                NeutronManager.Instance.RegisterNeutron(neutronScript);
        }

        // 3 에너지 기록
        if (ExperimentUI.Instance != null)
            ExperimentUI.Instance.AddEnergy(energyPerFission);

        Debug.Log("?? Fission! Energy +" + energyPerFission);
    }

        // 중성자 충돌 시 호출
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Neutron"))
        {
            Fission();
        }
    }
}

