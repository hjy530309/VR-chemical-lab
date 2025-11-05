using UnityEngine;

public class ExplosionForceFromTarget : MonoBehaviour
{
    public AudioSource ironAudio;
    public float explosionForce = 500f;    // 힘의 크기
    public float explosionRadius = 5f;     // 폭발 범위
    public Transform explosionTarget;       // 폭발 중심이 될 오브젝트

    void OnEnable()
    {
        if (explosionTarget == null)
        {
            Debug.LogWarning("Explosion target not assigned!");
            return;
        }

        transform.position = explosionTarget.position;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddExplosionForce(explosionForce, explosionTarget.position, explosionRadius);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (ironAudio != null && !ironAudio.isPlaying)
        {
            ironAudio.Play();
        }
    }
}
