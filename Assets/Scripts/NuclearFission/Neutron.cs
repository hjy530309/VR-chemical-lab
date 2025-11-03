using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder;

[RequireComponent(typeof(Rigidbody))]
public class Neutron : MonoBehaviour
{
    public float baseSpeed = 0.2f;
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

        rb.velocity = Random.onUnitSphere * baseSpeed;
        //rb.velocity = Random.onUnitSphere * initialSpeed;
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
        if (rb.velocity.magnitude > 0.01f)
            rb.velocity = rb.velocity.normalized * baseSpeed * multiplier;
        rb.velocity = rb.velocity.normalized * speed * multiplier;
    }
}
