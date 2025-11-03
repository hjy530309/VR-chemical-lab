using UnityEngine;
using Fusion;

public class NetworkedGun : NetworkBehaviour
{
    [Header("Gun Settings")]
    public float shootForce = 100f;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    [Networked] private TickTimer shootCooldown { get; set; }
    [Networked] private NetworkButtons previousButtons { get; set; }

    public struct NetworkInputData : INetworkInput
    {
        public NetworkButtons Buttons;
        public Vector3 HandPosition;
        public Quaternion HandRotation;
    }

    public static class NetworkInputButton
    {
        public const int Trigger = 0;
        public const int Grip = 1;
        public const int Primary = 2;
        public const int Secondary = 3;
    }


    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        HandleInput();
    }

    private void HandleInput()
    {
        if (GetInput<NetworkInputData>(out var input))
        {
            var buttons = input.Buttons;
            var pressed = buttons.GetPressed(previousButtons);

            // 트리거 입력 처리
            if (pressed.IsSet(NetworkInputButton.Trigger))
            {
                TryShoot();
            }

            previousButtons = buttons;
        }
    }

    private void TryShoot()
    {
        if (shootCooldown.ExpiredOrNotRunning(Runner))
        {
            Shoot();
            shootCooldown = TickTimer.CreateFromSeconds(Runner, 0.2f);
        }
    }

    private void Shoot()
    {
        // 발사 로직 구현
        Runner.Spawn(bulletPrefab,
            shootPoint.position,
            shootPoint.rotation,
            Object.InputAuthority);

        // RPC로 효과 재생
        RPC_PlayShootEffects();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayShootEffects()
    {
        // 발사 효과음, 파티클 등 재생
        GetComponent<AudioSource>()?.Play();
    }
}