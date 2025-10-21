using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Fusion;
using UnityEngine.SceneManagement;
using Christina.GameSystems;
using Fusion.XR.Host.Rig;

namespace Christina.GameSystems
{
    public class GunShootVR : NetworkBehaviour
    {
        [Header("총 설정")]
        public float fireRate = 0.25f;
        public float weaponRange = 20f;

        [Header("총구 & 파티클")]
        public Transform gunEnd;
        public ParticleSystem muzzleFlash;
        public ParticleSystem cartridgeEjection;
        public ElementCounter elementMgr;

        [Header("Hit 이펙트 프리팹")]
        public GameObject metalHitEffect;
        public GameObject sandHitEffect;
        public GameObject stoneHitEffect;
        public GameObject waterLeakEffect;
        public GameObject waterLeakExtinguishEffect;
        public GameObject[] fleshHitEffects;
        public GameObject woodHitEffect;

        [Header("오른손 HardwareHand 참조")]
        public HardwareHand rightHand;

        private float nextFire;
        private Animator anim;

        // 행성별 원소 데이터
        Dictionary<string, (string[] elements, float[] weights)> planetData;

        private void Awake()
        {
            anim = GetComponent<Animator>();

            planetData = new Dictionary<string, (string[], float[])>
            {
                { "Mercury", (new[] { "Fe", "Si", "O" }, new[] { 0.7f, 0.2f, 0.1f }) },
                { "Venus",   (new[] { "O", "Si", "Mg", "Fe" }, new[] { 0.5f, 0.3f, 0.15f, 0.05f }) },
                { "Earth",   (new[] { "O", "Si", "Al", "Fe", "Mg" }, new[] { 0.5f, 0.3f, 0.1f, 0.05f, 0.05f }) },
                { "Moon",    (new[] { "O", "Si", "Mg", "Fe", "Ca", "Al" }, new[] { 0.42f, 0.2f, 0.1f, 0.13f, 0.08f, 0.07f }) },
                { "Mars",    (new[] { "O", "Fe", "Si", "Mg" }, new[] { 0.35f, 0.25f, 0.2f, 0.2f }) },
                { "Jupiter", (new[] { "H", "He" }, new[] { 0.9f, 0.1f }) },
                { "Saturn",  (new[] { "H", "He" }, new[] { 0.96f, 0.04f }) },
                { "Uranus",  (new[] { "H", "He", "O" }, new[] { 0.83f, 0.15f, 0.02f }) },
                { "Neptune", (new[] { "H", "He", "O" }, new[] { 0.8f, 0.18f, 0.02f }) }
            };
        }

        private void Update()
        {
            if (!Object || !Object.HasInputAuthority)
                return;

            if (rightHand == null) return;

            // 🔹 HardwareHand triggerCommand 읽기
            float triggerValue = rightHand.handCommand.triggerCommand;

            // 🔹 발사 조건: trigger > 0.8f & fireRate 체크
            if (triggerValue > 0.8f && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Fire();
            }
        }

        private void Fire()
        {
            // 파티클 & 애니메이션
            muzzleFlash?.Play();
            cartridgeEjection?.Play();
            anim?.SetTrigger("Fire");

            // Raycast 발사
            if (Physics.Raycast(gunEnd.position, gunEnd.forward, out RaycastHit hit, weaponRange))
            {
                HandleHit(hit);
            }
        }

        private void HandleHit(RaycastHit hit)
        {
            if (hit.collider.sharedMaterial == null) return;

            string mat = hit.collider.sharedMaterial.name;
            GameObject effectPrefab = null;

            switch (mat)
            {
                case "Metal": effectPrefab = metalHitEffect; break;
                case "Sand": effectPrefab = sandHitEffect; break;
                case "Stone": effectPrefab = stoneHitEffect; break;
                case "WaterFilled": SpawnDecal(hit, waterLeakEffect); effectPrefab = metalHitEffect; break;
                case "WaterFilledExtinguish": SpawnDecal(hit, waterLeakExtinguishEffect); effectPrefab = metalHitEffect; break;
                case "Wood": effectPrefab = woodHitEffect; break;
                case "Meat":
                case "Character": effectPrefab = fleshHitEffects[Random.Range(0, fleshHitEffects.Length)]; break;
            }

            if (effectPrefab)
                SpawnDecal(hit, effectPrefab);

            SpawnTextAtHit(hit);
        }

        private void SpawnDecal(RaycastHit hit, GameObject prefab)
        {
            GameObject spawned = Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal));
            spawned.transform.SetParent(hit.collider.transform);
        }

        private void SpawnTextAtHit(RaycastHit hit)
        {
            foreach (var kvp in planetData)
            {
                if (hit.collider.name.Contains(kvp.Key))
                {
                    string chosen = PickByWeight(kvp.Value.elements, kvp.Value.weights);
                    elementMgr?.AddElement(chosen);
                    SpawnsDamagePopups.Instance?.DamageDone(chosen, hit.point, false);
                }
            }
        }

        private string PickByWeight(string[] elements, float[] weights)
        {
            if (elements == null || weights == null || elements.Length != weights.Length || elements.Length == 0)
                return null;

            float rand = Random.value;
            float cumulative = 0f;

            for (int i = 0; i < elements.Length; i++)
            {
                cumulative += weights[i];
                if (rand < cumulative)
                    return elements[i];
            }

            return elements[elements.Length - 1];
        }
    }
}
