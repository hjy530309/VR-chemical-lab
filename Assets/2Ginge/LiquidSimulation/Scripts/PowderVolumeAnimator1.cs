using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowderVolumeAnimator : MonoBehaviour {

	// Use this for initialization
    //just incase we have multiple materials;
    [HideInInspector]
    [SerializeField]
    public Material[] mats;
    //the level of the liquid on a hyperplane.
    [Range(0,1)]
    [SerializeField]
    public float level = 0.5f;
    private float finalLevel;
    private bool filling = false;
    
    //the current bound given a direction
    public Vector2 minMaxBounds;
    //the mesh to 
    [HideInInspector]
    [SerializeField]
    private MeshFilter mf;
    [HideInInspector]
    [SerializeField]
    private MeshRenderer mr;
    [HideInInspector]
    [SerializeField]
    private Mesh m;
    //in order to see how its working, physics is kind of hacked in
    public bool DebugAnchor = false;
    //debugsize is how large the handles are
    public float debugSize = 1.0f;
    //how much mass it has? kind of?
    public float _anchorLength = 0.5f;
    //how much the rotation and movement applies to the anchor
    [Range(0,1)]
    public float dampening;
    //it always wants to be rotated to down
    [HideInInspector]
    [SerializeField]
    private Vector3 anchor;
    //velocity of the direction of 'gravity'
    [HideInInspector]
    [SerializeField]
    private Vector3 anchorVelocity;
    //direction of velocity after clamp. ALSO used to see if the position has changed and use that as a velocity reference;
    [HideInInspector]
    [SerializeField]
    private Vector3 transformedPoint, prevTransformedPoint;
    public bool calculateTextureProjection = true;
    public float TextureSize = 1.0f;
    public float TextureSizeScalar = 1;
    public AnimationCurve texCurveSize = AnimationCurve.Linear(0, 1, 1, 1);
    //for rotation of the texture;
    Quaternion previous;
    float totalRotation = 0.0f;
    [HideInInspector]
    [SerializeField]
    private Vector3 TopLeft, TopRight, BottomLeft, BottomRight;

    public Transform ExposedLiquidT;
    public Vector3 GravityDirection = Vector3.down;
    public bool normalizeGravityDirection = true;
    //vert cache
    [HideInInspector]
    [SerializeField]
    Vector3[] verts;
	int shader_Key_localHeight;
	int shader_Key_anchor;
	int shader_Key_point;
	int shader_Key_level;
    float prvLevel = -1;
    Quaternion prevQ = Quaternion.identity;
    //for debugging purposes
    //for caching
    [HideInInspector]
    [SerializeField]
    Vector3 cPos = Vector3.zero;

	//output values:
	public Vector3 finalAnchor, finalPoint;
    [HideInInspector]
    [SerializeField]
    string[] shaderNames;
    void OnDrawGizmosSelected()
    {
        float anchorLength = _anchorLength * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
        if(DebugAnchor)
        {
            cPos = transform.position;
            //Vector3 prevTransformedPointD = transform.TransformDirection(Vector3.down);
            Vector3 anchorD = cPos - transform.TransformDirection(Vector3.up) * anchorLength;
            if (anchor == Vector3.zero)
                anchor = anchorD;
            //makes sure something is available (even while not playing).

            CalculateSquare(anchor);
            //the location of the anchor (static)
            Gizmos.DrawSphere(anchorD, 0.25f * transform.lossyScale.magnitude * 0.1f * debugSize);
            //line between the anchor and the surface
            Gizmos.DrawLine(cPos, anchorD);
            Gizmos.color = Color.blue;
            //the ocation of the anchor (physics)
            Gizmos.DrawSphere(anchor, 0.25f * transform.lossyScale.magnitude * 0.1f * debugSize);
           
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(cPos - (anchorD - cPos).normalized * finalLevel, 0.1f * transform.lossyScale.magnitude * 0.01f * debugSize);
            //All four corners of the texture
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(TopLeft, 0.25f * transform.lossyScale.magnitude * 0.01f * debugSize);
            Gizmos.DrawSphere(TopRight, 0.25f * transform.lossyScale.magnitude * 0.01f * debugSize);
            Gizmos.DrawSphere(BottomLeft, 0.25f * transform.lossyScale.magnitude * 0.01f * debugSize);
            Gizmos.DrawSphere(BottomRight, 0.25f * transform.lossyScale.magnitude * 0.01f * debugSize);
            Gizmos.color = Color.white;
            CalculateSquare(anchor);
        }
    }


	void Start ()
	{
        cPos = transform.position;
		shader_Key_localHeight = Shader.PropertyToID ("_localHeight");
		shader_Key_anchor = Shader.PropertyToID ("_anchor");
		shader_Key_point = Shader.PropertyToID ("_point");
		shader_Key_level = Shader.PropertyToID ("_level");
        //delta
		prevTransformedPoint = transformedPoint = transform.TransformDirection((normalizeGravityDirection) ? GravityDirection.normalized : GravityDirection);
        //anchor = cPos - transform.TransformDirection(Vector3.up) * anchorLength;
        anchor -= ((normalizeGravityDirection) ? GravityDirection.normalized : GravityDirection) * -1 * Time.deltaTime * (1.0f - dampening);
        anchor.Normalize();
	    mr = GetComponent<MeshRenderer>();
	    mats = mr.materials;
        shaderNames = new string[mats.Length];

        for (int i = 0; i < mats.Length; ++i)
	    {
	        mats[i] = Instantiate(mats[i]);
            shaderNames[i] = mats[i].shader.name;

        }
	    mf = GetComponent<MeshFilter>();
	    m = mf.sharedMesh;
		verts = new Vector3[m.vertices.Length];
	    verts = m.vertices;
	    minMaxBounds.x = minMaxBounds.y = verts[0].y;
        
	    for (int i = 0; i < verts.Length; ++i)
	    {
            Vector3 wPos = transform.TransformDirection(verts[i]);
            if (wPos.y > minMaxBounds.y)
	        {
	            minMaxBounds.y = wPos.y;
	        }
	        if (wPos.y < minMaxBounds.x)
	        {
	            minMaxBounds.x = wPos.y;
	        }
	    }
	    minMaxBounds.x -= cPos.y;
	    minMaxBounds.y -= cPos.y;
	    for (int i = 0; i < mats.Length; ++i)
	    {
			mats[i].SetFloat(shader_Key_localHeight, Mathf.Lerp(minMaxBounds.x,minMaxBounds.y,level));
	    }

	    mr.materials = mats;

	}
	public void AddForce(Vector3 force)
    {
        anchorVelocity += force;
    }
	// Update is called once per frame
	void FixedUpdate()
{
    // Filling 애니메이션
    if (!filling && level <= 0.03f)
        filling = true;
    if (filling && level <= 1f)
    {
        level = Mathf.MoveTowards(level, 1f, 0.5f * Time.deltaTime);
        if (level >= 1f) filling = false;
    }

    cPos = transform.position;
    float nLength = _anchorLength * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

    // Gravity 기반 anchor 이동
    Vector3 gravity = (normalizeGravityDirection ? GravityDirection.normalized : GravityDirection);
    anchorVelocity += gravity * Time.fixedDeltaTime * 0.2f; // 힘 조절 가능
    anchor += anchorVelocity * (1.0f - dampening);

    // 작은 Perlin/XZ/Y 흔들림 추가
    anchor += new Vector3(
        (Mathf.PerlinNoise(Time.time, anchor.z) - 0.5f) * 0.02f,
        (Mathf.PerlinNoise(Time.time * 1.5f, anchor.x) - 0.5f) * 0.02f,
        (Mathf.PerlinNoise(anchor.x, Time.time) - 0.5f) * 0.02f
    );

    // 랜덤 튀는 느낌
    anchorVelocity += new Vector3(
        (UnityEngine.Random.value - 0.5f) * 0.02f,
        (UnityEngine.Random.value - 0.5f) * 0.01f,
        (UnityEngine.Random.value - 0.5f) * 0.02f
    );

    // Clamp: 중력 방향 기준 최대 거리 유지
    Vector3 flatAnchor = new Vector3(anchor.x, 0, anchor.z);
    Vector3 flatCPos = new Vector3(cPos.x, 0, cPos.z);
    float d = Vector3.Distance(flatAnchor, flatCPos);
    if (d > nLength)
    {
        Vector3 dir = (flatAnchor - flatCPos).normalized;
        anchor.x = cPos.x + dir.x * nLength;
        anchor.z = cPos.z + dir.z * nLength;
    }

    // finalLevel 계산
    Matrix4x4 localToWorld = transform.localToWorldMatrix;
    minMaxBounds.x = minMaxBounds.y = (transform.TransformPoint(verts[0])).y;
    for (int i = 0; i < verts.Length; ++i)
    {
        Vector3 wPos = localToWorld.MultiplyPoint(verts[i]);
        if (wPos.y > minMaxBounds.y) minMaxBounds.y = wPos.y;
        if (wPos.y < minMaxBounds.x) minMaxBounds.x = wPos.y;
    }
    minMaxBounds.y -= cPos.y;
    minMaxBounds.x -= cPos.y;
    finalLevel = Mathf.Lerp(minMaxBounds.x, minMaxBounds.y, level);

    if (level <= Single.Epsilon * 10)
        anchor = Vector3.down * nLength + cPos;

    finalPoint = cPos - (anchor - cPos).normalized * finalLevel;

    // Shader 적용
    for (int i = 0; i < mats.Length; ++i)
    {
        mats[i].SetFloat(shader_Key_localHeight, Mathf.Lerp(minMaxBounds.x - Single.Epsilon * 10, minMaxBounds.y + Single.Epsilon * 10, level));
        mats[i].SetVector(shader_Key_anchor, transform.InverseTransformPoint(anchor));
        mats[i].SetVector(shader_Key_point, transform.InverseTransformPoint(finalPoint));
        mats[i].SetFloat(shader_Key_level, level - Single.Epsilon);
    }

    finalAnchor = anchor;
    CalculateSquare(anchor);

    // ExposedLiquidT 업데이트
    if (ExposedLiquidT != null)
    {
        ExposedLiquidT.position = finalPoint;
        ExposedLiquidT.localScale = Vector3.one * texCurveSize.Evaluate(Mathf.Clamp01(level)) * transform.lossyScale.magnitude * 0.001f * TextureSize * TextureSizeScalar;
        ExposedLiquidT.up = (finalPoint - finalAnchor).normalized;
    }

    // Plane 셋팅
    Vector4 sTopLeft = transform.InverseTransformPoint(TopLeft);
    Vector4 sTopRight = transform.InverseTransformPoint(TopRight);
    Vector4 sBotLeft = transform.InverseTransformPoint(BottomLeft);
    Vector4 sBotRight = transform.InverseTransformPoint(BottomRight);
    Vector4 sCenter = transform.InverseTransformPoint(finalPoint);

    for (int i = 0; i < mats.Length; ++i)
    {
        if (!shaderNames[i].Contains("_Texture")) continue;
        mats[i].SetVector("_TL", sTopLeft);
        mats[i].SetVector("_TR", sTopRight);
        mats[i].SetVector("_BL", sBotLeft);
        mats[i].SetVector("_BR", sBotRight);
        mats[i].SetVector("_CENTER", sCenter);
    }

    prevTransformedPoint = transformedPoint;
    previous = transform.rotation;
    prvLevel = level;
    prevQ = transform.rotation;
}
void CalculateSquare(Vector3 anch)
{
    if (!calculateTextureProjection)
    {
        return;
    }

    Vector3 pos1 = cPos - (anch - cPos).normalized * finalLevel;
    Vector3 nrm = (cPos - (anch - cPos).normalized * finalLevel - anch).normalized;
    Vector3 beginningRight = Quaternion.Euler(0, totalRotation, 0) * Vector3.right;
    Vector3 f1 = Vector3.Cross(beginningRight, nrm.normalized).normalized;
    Vector3 r1 = Vector3.Cross(f1, nrm).normalized;
    f1 = Vector3.Cross(r1, nrm).normalized;
    float tSize = TextureSize * texCurveSize.Evaluate(Mathf.Clamp01(level)) * transform.lossyScale.magnitude * 0.001f;

    // 여기에 Perlin Noise 기반 offset 추가 (가루 흩날림)
    float noiseStrength = 0.02f; // 흔들림 세기
    float time = Time.time;

    Vector3 offsetTL = new Vector3(Mathf.PerlinNoise(time, TopLeft.z) - 0.5f,
                                   Mathf.PerlinNoise(TopLeft.x, time) - 0.5f,
                                   Mathf.PerlinNoise(time, TopLeft.x) - 0.5f) * noiseStrength;

    Vector3 offsetTR = new Vector3(Mathf.PerlinNoise(time + 10, TopRight.z) - 0.5f,
                                   Mathf.PerlinNoise(TopRight.x, time + 10) - 0.5f,
                                   Mathf.PerlinNoise(time + 10, TopRight.x) - 0.5f) * noiseStrength;

    Vector3 offsetBL = new Vector3(Mathf.PerlinNoise(time + 20, BottomLeft.z) - 0.5f,
                                   Mathf.PerlinNoise(BottomLeft.x, time + 20) - 0.5f,
                                   Mathf.PerlinNoise(time + 20, BottomLeft.x) - 0.5f) * noiseStrength;

    Vector3 offsetBR = new Vector3(Mathf.PerlinNoise(time + 30, BottomRight.z) - 0.5f,
                                   Mathf.PerlinNoise(BottomRight.x, time + 30) - 0.5f,
                                   Mathf.PerlinNoise(time + 30, BottomRight.x) - 0.5f) * noiseStrength;

    TopLeft = r1 * tSize + f1 * tSize + pos1 + offsetTL;
    TopRight = r1 * tSize * -1 + f1 * tSize + pos1 + offsetTR;
    BottomLeft = r1 * tSize + f1 * tSize * -1 + pos1 + offsetBL;
    BottomRight = r1 * tSize * -1 - f1 * tSize + pos1 + offsetBR;
}

}