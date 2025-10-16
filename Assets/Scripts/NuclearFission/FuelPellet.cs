using UnityEngine;


public class FuelPellet : MonoBehaviour
{
    [Header("연쇄 반응 설정")]
    public int maxSpawnPerFission = 3;         // 한 번 핵분열 시 생성할 Neutron 수
    public float neutronSpeed = 5f;            // 기본 Neutron 속도
    public GameObject neutronPrefab;           // Neutron Prefab
    public ControlRod controlRod;              // Scene에서 연결 (Control Rod)

    [Header("에너지 & 효과")]
    public int energyPerFission = 10;          // 한 번 핵분열 시 생성 에너지
    public ParticleSystem fissionEffect;       // 파티클 Prefab

    private bool hasFissioned = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true; // 기본 고정
    }

/*    // XR Grab 이벤트 
    public void OnGrab()
    {
        rb.isKinematic = true; // 잡는 동안 위치 직접 제어
    }

    public void OnRelease()
    {
        rb.isKinematic = true; // 놓아도 고정 (중성자 충돌 무시)
        rb.velocity = Vector3.zero; // 흔들림 방지
    }*/

    public void Fission()
    {
        if (hasFissioned) return;
        hasFissioned = true;

        // 1? 파티클 효과
        if (fissionEffect != null)
            Instantiate(fissionEffect, transform.position, Quaternion.identity);

        // 2? 연쇄 Neutron 생성
        for (int i = 0; i < maxSpawnPerFission; i++)
        {
            // FuelPellet 중심 겹치지 않도록 offset
            float spawnDistance = 0.2f + 0.05f; // FuelPellet 반지름 + Neutron 반지름
            Vector3 offset = Random.onUnitSphere * spawnDistance;

            GameObject n = Instantiate(neutronPrefab, transform.position + offset, Random.rotation);
            Rigidbody nRb = n.GetComponent<Rigidbody>();

            // Control Rod 속도 적용
            //float speedMultiplier = controlRod != null ? controlRod.controlLevel : 1f;
            //nRb.velocity = Random.onUnitSphere * neutronSpeed * speedMultiplier;

            // Rigidbody 세팅
            nRb.useGravity = false;
            nRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            nRb.interpolation = RigidbodyInterpolation.Interpolate;
            nRb.drag = 0;

            // NeutronManager 등록
            NeutronManager.Instance.RegisterNeutron(n.GetComponent<Neutron>());
        }

        // 3? 에너지 기록
        if (ExperimentUI.Instance != null)
            ExperimentUI.Instance.AddEnergy(energyPerFission);

        Debug.Log("?? Fission! Energy +" + energyPerFission);
    }
}

/*public class FuelPellet : MonoBehaviour
{
    public GameObject neutronPrefab;
    public int emittedNeutrons = 3;
    private bool hasFissioned = false;
    public ParticleSystem fissionEffect;
    public int energyPerFission = 10;

    public void Fission()
    {
        if (hasFissioned) return; // 이미 분열했으면 무시
        hasFissioned = true;

        Debug.Log("핵분열 발생! 에너지 방출");

        // 파티클 효과
        if (fissionEffect != null)
            Instantiate(fissionEffect, transform.position, Quaternion.identity);

        // 여러 개의 중성자 방출
        for (int i = 0; i < emittedNeutrons; i++)
        {
            Vector3 spawnPos = transform.position + Random.onUnitSphere * 0.5f;
            NeutronManager.Instance.SpawnNeutron(spawnPos);
            Debug.Log("새 중성자 방출됨");

            // 에너지 기록
            ExperimentUI.Instance.AddEnergy(energyPerFission);
            Debug.Log("?? Fission! Energy +" + energyPerFission);
        }
    }
}*/
