/*using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class Neutron : MonoBehaviour
{
    public float speed = 3f;
    private Rigidbody rb;
    private bool canCollide = false;

    private Collider neutronCollider;

    void Awake()
    {
        neutronCollider = GetComponent<Collider>();
    }

    public void OnGrabStart(Collider handCollider)
    {
        // 플레이어 손과 FuelPellet 충돌 무시
        Collider[] fuelPellets = GameObject.FindGameObjectsWithTag("FuelPellet")
                                     .Select(go => go.GetComponent<Collider>())
                                     .ToArray();

        foreach (var pelletCol in fuelPellets)
            Physics.IgnoreCollision(neutronCollider, pelletCol, true);

        // 필요하다면 Chamber Collider도 무시
        Collider chamberCol = GameObject.Find("Chamber").GetComponent<Collider>();
        Physics.IgnoreCollision(neutronCollider, chamberCol, true);
    }

    public void OnGrabEnd()
    {
        // 중성자 놓으면 다시 충돌 활성화
        Collider[] fuelPellets = GameObject.FindGameObjectsWithTag("FuelPellet")
                                     .Select(go => go.GetComponent<Collider>())
                                     .ToArray();

        foreach (var pelletCol in fuelPellets)
            Physics.IgnoreCollision(neutronCollider, pelletCol, false);

        Collider chamberCol = GameObject.Find("Chamber").GetComponent<Collider>();
        Physics.IgnoreCollision(neutronCollider, chamberCol, false);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // 초기 정지 (플레이어가 던질 수 있음)
        rb.velocity = Vector3.zero;

        NeutronManager.Instance.RegisterNeutron(this);
        StartCoroutine(EnableCollisionAfterDelay(0.05f));
    }

    private IEnumerator EnableCollisionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canCollide = true;
    }

    void Update()
    {
        // 전역 속도 배율 반영
        float multiplier = NeutronManager.Instance != null
            ? NeutronManager.Instance.globalSpeedMultiplier
            : 1f;

        // 현재 방향 유지하면서 속도 갱신
        if (rb.velocity.magnitude > 0.01f)
            rb.velocity = rb.velocity.normalized * speed * multiplier;

    }

    void OnCollisionEnter(Collision collision)
    {
        if (!canCollide) return;

        if (collision.collider.CompareTag("FuelPellet"))
        {
            FuelPellet pellet = collision.collider.GetComponent<FuelPellet>();
            if (pellet != null)
            {
                pellet.Fission();
                NeutronManager.Instance.SpawnNeutron(collision.contacts[0].point);
            }
            StartCoroutine(DestroyAfterFrame());
        }

        if (collision.collider.CompareTag("Chamber"))
        {
            Vector3 normal = collision.contacts[0].normal;
            if (Vector3.Dot(normal, rb.velocity) > 0)
                normal = -normal;

            rb.velocity = Vector3.Reflect(rb.velocity, normal).normalized * speed;
        }
    }

    private IEnumerator DestroyAfterFrame()
    {
        yield return null;
        DestroySelf();
    }

    public void DestroySelf()
    {
        NeutronManager.Instance.UnregisterNeutron(this);
        Destroy(gameObject);
    }
}*/






using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder;

[RequireComponent(typeof(Rigidbody))]
public class Neutron : MonoBehaviour
{
    public float speed = 3f;
    public float lifeTime = 20f; // 일정 시간 후 자동 소멸
    private Rigidbody rb;
    private bool canCollide = false;
    public ControlRod controlRod;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

       float initialSpeed = speed;
        //if (controlRod != null)
        //    initialSpeed *= controlRod.controlLevel;

        rb.velocity = Random.onUnitSphere * initialSpeed;
        NeutronManager.Instance.RegisterNeutron(this);

        StartCoroutine(EnableCollisionAfterDelay(0.05f));

        //Destroy(gameObject, 10f); // 수명 제한
    }

    private IEnumerator EnableCollisionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canCollide = true;
    }


    void OnCollisionEnter(Collision collision)
    {
        if (!canCollide) return;
        if (collision.collider.CompareTag("FuelPellet"))
        {
            FuelPellet pellet = collision.collider.GetComponent<FuelPellet>();
            if (pellet != null)
            {

                pellet.Fission();
                // 핵분열 시 새로운 중성자 방출
                NeutronManager.Instance.SpawnNeutron(collision.contacts[0].point, speed);
            }

            // 한 프레임 뒤에 Destroy
            StartCoroutine(DestroyAfterFrame());
        }

        if (collision.collider.CompareTag("Chamber"))
        {
            Vector3 normal = collision.contacts[0].normal;
            if (Vector3.Dot(normal, rb.velocity) > 0)
                normal = -normal;

            rb.velocity = Vector3.Reflect(rb.velocity, normal).normalized * speed;
        }
    }

    private IEnumerator DestroyAfterFrame()
    {
        yield return null; // 다음 프레임까지 대기
        DestroySelf();
    }


    public void DestroySelf()
    {
        NeutronManager.Instance.UnregisterNeutron(this);
        Destroy(gameObject);
    }

    public void ModifySpeed(float multiplier)
    {
        rb.velocity = rb.velocity.normalized * speed * multiplier;
    }
}
