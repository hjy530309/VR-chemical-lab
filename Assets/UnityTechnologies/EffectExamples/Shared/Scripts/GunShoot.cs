using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Christina.GameSystems;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class GunShoot : MonoBehaviour
{
    public float fireRate = 0.25f;
    public float weaponRange = 20f;

    public Transform gunEnd;
    public ParticleSystem muzzleFlash;
    public ParticleSystem cartridgeEjection;
    public ElementCounter elementMgr;

    public GameObject metalHitEffect;
    public GameObject sandHitEffect;
    public GameObject stoneHitEffect;
    public GameObject waterLeakEffect;
    public GameObject waterLeakExtinguishEffect;
    public GameObject[] fleshHitEffects;
    public GameObject woodHitEffect;

    private float nextFire;
    private Animator anim;
    private GunAim gunAim;

    // VR 지원을 위한 변수 추가
    [Header("VR Settings")]
    public bool isVRMode = false;
    public bool useTriggerInput = true;

    Dictionary<string, (string[] elements, float[] weights)> planetData;

    void Start()
    {
        anim = GetComponent<Animator>();

        // VR 모드가 아닐 때만 GunAim 사용
        if (!isVRMode)
        {
            gunAim = GetComponentInParent<GunAim>();
        }

        // VR 모드일 때 참조 찾기
        if (isVRMode)
        {
            FindVRReferences();
        }
    }

    void Awake()
    {
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

    void Update()
    {
        // VR 모드일 때는 Update에서 입력 처리하지 않음 (VR 컨트롤러에서 직접 호출)
        if (!isVRMode && Input.GetButtonDown("Fire1") && Time.time > nextFire && !gunAim.GetIsOutOfBounds())
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }
    }

    // 발사 공통 메서드
    private void Shoot()
    {
        muzzleFlash.Play();
        cartridgeEjection.Play();

        if (anim != null)
            anim.SetTrigger("Fire");

        Vector3 rayOrigin = gunEnd.position;
        RaycastHit hit;

        // 디버그 레이 그리기
        Debug.DrawRay(rayOrigin, gunEnd.forward * weaponRange, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, gunEnd.forward, out hit, weaponRange))
        {
            HandleHit(hit);
        }
    }

    public void HandleHit(RaycastHit hit)
    {
        if (hit.collider.sharedMaterial != null)
        {
            string materialName = hit.collider.sharedMaterial.name;

            switch (materialName)
            {
                case "Metal":
                    SpawnDecal(hit, metalHitEffect);
                    SpawnTextAtHit(hit);
                    break;
                case "Sand":
                    SpawnDecal(hit, sandHitEffect);
                    SpawnTextAtHit(hit);
                    break;
                case "Stone":
                    SpawnDecal(hit, stoneHitEffect);
                    SpawnTextAtHit(hit);
                    break;
                case "WaterFilled":
                    SpawnDecal(hit, waterLeakEffect);
                    SpawnDecal(hit, metalHitEffect);
                    SpawnTextAtHit(hit);
                    break;
                case "Wood":
                    SpawnDecal(hit, woodHitEffect);
                    SpawnTextAtHit(hit);
                    break;
                case "Meat":
                    SpawnDecal(hit, fleshHitEffects[Random.Range(0, fleshHitEffects.Length)]);
                    SpawnTextAtHit(hit);
                    break;
                case "Character":
                    SpawnDecal(hit, fleshHitEffects[Random.Range(0, fleshHitEffects.Length)]);
                    SpawnTextAtHit(hit);
                    break;
                case "WaterFilledExtinguish":
                    SpawnDecal(hit, waterLeakExtinguishEffect);
                    SpawnDecal(hit, metalHitEffect);
                    SpawnTextAtHit(hit);
                    break;
            }
        }
    }

    void SpawnDecal(RaycastHit hit, GameObject prefab)
    {
        GameObject spawnedDecal = GameObject.Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal));
        spawnedDecal.transform.SetParent(hit.collider.transform);
    }

    void SpawnTextAtHit(RaycastHit hit)
    {
        // elementMgr 확인
        if (elementMgr == null)
        {
            Debug.LogWarning("ElementCounter가 없습니다! 찾는 중...");
            FindVRReferences();
            if (elementMgr == null) return;
        }

        foreach (var kvp in planetData)
        {
            if (hit.collider.name.Contains(kvp.Key))
            {
                string chosen = PickByWeight(kvp.Value.elements, kvp.Value.weights);

                // 원소 추가
                elementMgr.AddElement(chosen);
                Debug.Log($"VR에서 원소 추가: {chosen} - {kvp.Key}");

                // 데미지 팝업
                if (SpawnsDamagePopups.Instance != null)
                {
                    SpawnsDamagePopups.Instance.DamageDone(chosen, hit.point, false);
                    Debug.Log($"VR에서 데미지 팝업: {chosen}");
                }
                else
                {
                    Debug.LogWarning("SpawnsDamagePopups.Instance가 없습니다!");
                }
                break;
            }
        }
    }

    string PickByWeight(string[] elements, float[] weights)
    {
        if (elements == null || weights == null || elements.Length != weights.Length || elements.Length == 0)
        {
            return null;
        }

        float rand = Random.value;
        float cumulative = 0f;

        for (int i = 0; i < elements.Length; i++)
        {
            cumulative += weights[i];
            if (rand < cumulative)
            {
                return elements[i];
            }
        }

        return elements[elements.Length - 1];
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FindElementMgrNextFrame());
    }

    private IEnumerator FindElementMgrNextFrame()
    {
        yield return null;
        elementMgr = FindObjectOfType<ElementCounter>();
    }

    // VR 모드일 때 참조 찾기
    private void FindVRReferences()
    {
        if (elementMgr == null)
        {
            elementMgr = FindObjectOfType<ElementCounter>();
            Debug.Log("VR 모드 - ElementCounter 찾기: " + (elementMgr != null ? "성공" : "실패"));
        }

        // 1초 후에 다시 시도 (씬 로딩 딜레이 고려)
        StartCoroutine(DelayedVRReferenceSearch());
    }

    private IEnumerator DelayedVRReferenceSearch()
    {
        yield return new WaitForSeconds(1f);

        if (elementMgr == null)
        {
            elementMgr = FindObjectOfType<ElementCounter>();
            Debug.Log("VR 모드 - 지연 검색 ElementCounter: " + (elementMgr != null ? "성공" : "실패"));
        }
    }

    // VR용 공개 발사 메서드
    public void VRShoot()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }
    }

    // VR용 간편 발사 메서드 (쿨다운 무시)
    public void SimpleVRShoot()
    {
        Shoot();
    }

    // VR 모드 설정 메서드
    public void SetVRMode(bool vrMode)
    {
        isVRMode = vrMode;
        if (isVRMode)
        {
            FindVRReferences();
        }
    }

    // 발사 가능 여부 확인
    public bool CanShoot()
    {
        return Time.time > nextFire;
    }

    // 참조 수동 설정 (VR에서 사용)
    public void SetElementCounter(ElementCounter counter)
    {
        elementMgr = counter;
        Debug.Log("ElementCounter 수동 설정됨");
    }
}

/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Christina.GameSystems;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;
using Fusion;

public class GunShoot : MonoBehaviour {
	public float fireRate = 0.25f;										// Number in seconds which controls how often the player can fire
	public float weaponRange = 20f;										// Distance in Unity units over which the player can fire

	public Transform gunEnd;
	public ParticleSystem muzzleFlash;
	public ParticleSystem cartridgeEjection;
    public ElementCounter elementMgr;

    public GameObject metalHitEffect;
	public GameObject sandHitEffect;
	public GameObject stoneHitEffect;
	public GameObject waterLeakEffect;
    public GameObject waterLeakExtinguishEffect;
	public GameObject[] fleshHitEffects;
	public GameObject woodHitEffect;

	private float nextFire;												// Float to store the time the player will be allowed to fire again, after firing
	private Animator anim;
	private GunAim gunAim;

    // VR 지원을 위한 변수 추가
    [Header("VR Settings")]
    public bool isVRMode = false;
    public bool useTriggerInput = true;

    Dictionary<string, (string[] elements, float[] weights)> planetData;

	void Start () 
	{
		anim = GetComponent<Animator> ();
		gunAim = GetComponentInParent<GunAim>();
	}

	void Awake()
	{
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


    void Update()
    {
        // VR 모드일 때는 Update에서 입력 처리하지 않음 (VR 컨트롤러에서 직접 호출)
        if (!isVRMode && Input.GetButtonDown("Fire1") && Time.time > nextFire && !gunAim.GetIsOutOfBounds())
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }
    }

    // 발사 공통 메서드
    private void Shoot()
    {
        muzzleFlash.Play();
        cartridgeEjection.Play();

        if (anim != null)
            anim.SetTrigger("Fire");

        Vector3 rayOrigin = gunEnd.position;
        RaycastHit hit;

        // 디버그 레이 그리기
        UnityEngine.Debug.DrawRay(rayOrigin, gunEnd.forward * weaponRange, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, gunEnd.forward, out hit, weaponRange))
        {
            HandleHit(hit);
        }
    }
    //   void Update () 
    //{
    //	if (Input.GetButtonDown("Fire1") && Time.time > nextFire && !gunAim.GetIsOutOfBounds()) 
    //	{
    //		nextFire = Time.time + fireRate;
    //		muzzleFlash.Play();
    //		cartridgeEjection.Play();
    //		anim.SetTrigger ("Fire");

    //		Vector3 rayOrigin = gunEnd.position;
    //		RaycastHit hit;
    //		if (Physics.Raycast(rayOrigin, gunEnd.forward, out hit, weaponRange))
    //		{
    //			HandleHit(hit);
    //		}
    //	}
    //}


    public void HandleHit(RaycastHit hit)
	{
		if(hit.collider.sharedMaterial != null)
		{
			string materialName = hit.collider.sharedMaterial.name;

			switch(materialName)
			{
				case "Metal":
					SpawnDecal(hit, metalHitEffect);
					SpawnTextAtHit(hit);
					break;
				case "Sand":
					SpawnDecal(hit, sandHitEffect);
					SpawnTextAtHit(hit);
					break;
				case  "Stone":
					SpawnDecal(hit, stoneHitEffect);
					SpawnTextAtHit(hit);
					break;
				case "WaterFilled":
					SpawnDecal(hit, waterLeakEffect);
					SpawnDecal(hit, metalHitEffect);
					SpawnTextAtHit(hit);
					break;
				case "Wood":
					SpawnDecal(hit, woodHitEffect);
					SpawnTextAtHit(hit);
					break;
				case "Meat":
					SpawnDecal(hit, fleshHitEffects[Random.Range(0, fleshHitEffects.Length)]);
					SpawnTextAtHit(hit);
					break;
				case "Character":
					SpawnDecal(hit, fleshHitEffects[Random.Range(0, fleshHitEffects.Length)]);
					SpawnTextAtHit(hit);
					break;
                case "WaterFilledExtinguish":
                    SpawnDecal(hit, waterLeakExtinguishEffect);
                    SpawnDecal(hit, metalHitEffect);
					SpawnTextAtHit(hit);
                    break;
            }
		}
	}

	void SpawnDecal(RaycastHit hit, GameObject prefab)
	{
		GameObject spawnedDecal = GameObject.Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal));
		spawnedDecal.transform.SetParent(hit.collider.transform);
	}

//오브젝트 스폰
//SpawnWithCameraForce(hit, prefab); 이런식으로 사용
	void SpawnWithCameraForce(RaycastHit hit, GameObject prefab, bool applyCameraForce = false, float forceStrength = 5f)
	{
		// ① 오브젝트 생성
		GameObject spawned = Instantiate(prefab, hit.point, Quaternion.LookRotation(hit.normal));

		// ② 충돌 오브젝트 자식으로 붙이기 (선택)
		spawned.transform.SetParent(hit.collider.transform);

		// ③ Rigidbody 가져오기
		Rigidbody rb = spawned.GetComponent<Rigidbody>();
		if (rb != null && applyCameraForce)
		{
			// 카메라 방향 계산
			Vector3 dir = (Camera.main.transform.position - hit.point).normalized;

			// 힘 적용
			rb.AddForce(-dir * forceStrength, ForceMode.Impulse);

			// 선택 사항: 살짝 회전 추가
			rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
		}
	}

	void SpawnTextAtHit(RaycastHit hit)
	{
		foreach (var kvp in planetData)
		{
			if (hit.collider.name.Contains(kvp.Key))
			{
				string chosen = PickByWeight(kvp.Value.elements, kvp.Value.weights);
                elementMgr.AddElement(chosen);
                // SpawnsDamagePopups에서 월드 좌표 기준으로 텍스트 스폰
                SpawnsDamagePopups.Instance.DamageDone(chosen, hit.point, false);
			}
		}
	}

	string PickByWeight(string[] elements, float[] weights)
	{
		// 방어 코드: 배열 길이 다르거나 비어있을 때
		if (elements == null || weights == null || elements.Length != weights.Length || elements.Length == 0)
		{
			return null;
		}

		// 0~1 사이의 랜덤값
		float rand = UnityEngine.Random.value;
		float cumulative = 0f;

		// 각 weight를 누적하면서 랜덤값이 걸리는 지점에서 반환
		for (int i = 0; i < elements.Length; i++)
		{
			cumulative += weights[i];
			if (rand < cumulative)
			{
				return elements[i];
			}
		}

		// 누적합이 1보다 작을 경우 대비 — 마지막 값 반환
		return elements[elements.Length - 1];
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		StartCoroutine(FindElementMgrNextFrame());
	}

	private IEnumerator FindElementMgrNextFrame()
	{
		yield return null; // 다음 프레임까지 대기
		elementMgr = FindObjectOfType<ElementCounter>();
	}

    // VR용 공개 발사 메서드
    public void VRShoot()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }
    }

    // VR용 간편 발사 메서드 (쿨다운 무시)
    public void SimpleVRShoot()
    {
        Shoot();
    }

    // VR 모드 설정 메서드
    public void SetVRMode(bool vrMode)
    {
        isVRMode = vrMode;
    }

    // 발사 가능 여부 확인
    public bool CanShoot()
    {
        return Time.time > nextFire;
    }

}*/