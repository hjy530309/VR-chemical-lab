using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject neutronPrefab;
    public Transform spawnPoint;
    public float launchSpeed = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 테스트용 PC 발사
            ShootNeutron();
    }

    public void ShootNeutron()
    {
        GameObject n = Instantiate(neutronPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = n.GetComponent<Rigidbody>();
        rb.velocity = spawnPoint.forward * launchSpeed;
    }
}
