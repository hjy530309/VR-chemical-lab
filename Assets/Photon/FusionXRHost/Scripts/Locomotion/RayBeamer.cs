using Fusion.XR.Host.Rig;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Fusion.XR.Host.Locomotion
{
    public struct RayData
    {
        public bool isRayEnabled;
        public Vector3 origin;
        public Vector3 target;
        public Color color;
    }

    /**
     * 
     * Display a line renderer when action input is pressed, and raycast other the selected layer mask to find a destination point
     * 
     **/

    public class RayBeamer : MonoBehaviour
    {
        public HardwareHand hand;

        public bool useRayActionInput = true;
        public InputActionProperty rayAction;
        public Transform origin;
        public LayerMask targetLayerMask = ~0;
        public float maxDistance = 100f;

        [Header("Representation")]
        public LineRenderer lineRenderer;
        public float width = 0.02f;
        public Material lineMaterial;

        //public Color hitColor = Color.green;
        //public Color noHitColor = Color.red;
        public Color hitColor = new Color(0f, 1f, 0f);    // #00FF00 (초록)
        public Color noHitColor = new Color(1f, 0f, 0f);  // #FF0000 (빨강)

        public UnityEvent<Collider, Vector3> onRelease = new UnityEvent<Collider, Vector3>();

        // Define if the beamer ray is active this frame
        public bool isRayEnabled = false;

        public enum Status
        {
            NoBeam,
            BeamNoHit,
            BeamHit
        }
        public Status status = Status.NoBeam;

        public RayData ray;
        Vector3 lastHit;
        Collider lastHitCollider = null;


        public virtual void Awake()
        {
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.material = lineMaterial;
                lineRenderer.numCapVertices = 4;
            }
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.useWorldSpace = true;
            lineRenderer.enabled = false;

            if (origin == null) origin = transform;
            if (hand == null) hand = GetComponentInParent<HardwareHand>();
        }

        public virtual void Start()
        {
            rayAction.EnableWithDefaultXRBindings(hand.side, new List<string> { "thumbstickClicked", "primaryButton", "secondaryButton" });

        }

        public bool BeamCast(out RaycastHit hitInfo, Vector3 origin, Vector3 direction)
        {
            Ray handRay = new Ray(origin, direction);
            return Physics.Raycast(handRay, out hitInfo, maxDistance, targetLayerMask);
        }

        public bool BeamCast(out RaycastHit hitInfo)
        {
            return BeamCast(out hitInfo, ray.origin, origin.forward);
        }


        // 기존 코드에 이 부분만 추가
        public virtual void Update()
        {
            if (useRayActionInput && rayAction != null && rayAction.action != null)
            {
                isRayEnabled = rayAction.action.ReadValue<float>() == 1;
                Debug.Log($"Ray 입력: {isRayEnabled}, 버튼 값: {rayAction.action.ReadValue<float>()}");
            }

            ray.isRayEnabled = isRayEnabled; // 이 줄이 중요!

            if (ray.isRayEnabled)
            {
                ray.origin = origin.position;
                if (BeamCast(out RaycastHit hit))
                {
                    lastHitCollider = hit.collider;
                    ray.target = hit.point;
                    ray.color = hitColor;
                    lastHit = hit.point;
                    status = Status.BeamHit;

                    //  디버그: 어떤 UI에 맞았는지 출력
                    Debug.Log($"UI 히트: {hit.collider?.name}, 위치: {hit.point}");
                    Debug.Log($"   레이어: {hit.collider.gameObject.layer} (UI: {LayerMask.LayerToName(hit.collider.gameObject.layer)})");
                    Debug.Log($"   월드 위치: {hit.point}");
                    Debug.Log($"   로컬 위치: {hit.collider.transform.InverseTransformPoint(hit.point)}");

                    //  디버그: 실제로 버튼 컴포넌트가 있는지 확인
                    Button button = hit.collider?.GetComponent<Button>();
                    if (button != null)
                    {
                        Debug.Log($" 버튼 발견: {button.name}, 상호작용 가능: {button.interactable}");
                    }
                }
                else
                {
                    //  디버그: 왜 안 맞는지 확인
                    Debug.Log($" UI 히트 실패. 레이어 마스크: {targetLayerMask.value}");
                    lastHitCollider = null;
                    ray.target = ray.origin + origin.forward * maxDistance;
                    ray.color = noHitColor;
                    status = Status.BeamNoHit;
                }
            }
            else
            {
                if (status == Status.BeamHit)
                {
                    //  디버그: 릴리스 시 어떤 오브젝트에 신호 보내는지
                    Debug.Log($" UI 릴리스: {lastHitCollider?.name}");
                    if (onRelease != null) onRelease.Invoke(lastHitCollider, lastHit);

                    //  자동 클릭 처리
                    if (lastHitCollider != null)
                    {
                        Button button = lastHitCollider.GetComponent<Button>();
                        if (button != null && button.interactable)
                        {
                            Debug.Log($" 버튼 클릭 실행: {button.name}");
                            button.onClick.Invoke();
                        }
                    }
                }
                status = Status.NoBeam;
                lastHitCollider = null;
            }

            UpdateRay();
        }

        public void CancelHit()
        {
            status = Status.NoBeam;
        }

        void UpdateRay()
        {
            lineRenderer.enabled = ray.isRayEnabled;
            if (ray.isRayEnabled)
            {
                lineRenderer.SetPositions(new Vector3[] { ray.origin, ray.target });
                lineRenderer.positionCount = 2;

                //  머티리얼 문제 해결 - 완전 새로 생성
                if (lineRenderer.material == null)
                {
                    CreateSimpleMaterial();
                }

                //  가장 중요한 부분: 머티리얼 색상 직접 설정
                lineRenderer.material.color = ray.color;

                //  LineRenderer 색상도 설정 (이중 보장)
                lineRenderer.startColor = ray.color;
                lineRenderer.endColor = ray.color;

                //  현재 적용된 색상 확인
                Debug.Log($" 적용된 색상: {ray.color}, 머티리얼: {lineRenderer.material.color}");
            }
        }
        void CreateSimpleMaterial()
        {
            // 가장 기본적인 머티리얼 생성
            Shader shader = Shader.Find("Unlit/Color");
            if (shader != null)
            {
                Material newMat = new Material(shader);
                newMat.name = "DynamicLineMaterial";
                newMat.color = Color.white;
                lineRenderer.material = newMat;
                Debug.Log("새로운 머티리얼 생성됨");
            }
        }
    }
}