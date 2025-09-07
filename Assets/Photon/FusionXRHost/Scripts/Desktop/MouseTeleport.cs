using UnityEngine.InputSystem;
using UnityEngine;
using Fusion.XR.Host.Rig;
using Fusion.XR.Host.Grabbing;
using Fusion.XR.Host.Locomotion;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Fusion.XR.Host.Desktop
{
    public interface IMouseTeleportHover
    {
        void OnHoverHit(RaycastHit hit);
        void OnNoHover();
    }
    /**
     * Allow to rotate the rig head with the mouse
     * Allow to left click to teleport to a valid target
     * Allow to have partial interaction capability 
     */
    public class MouseTeleport : MonoBehaviour
    {
        public HardwareRig rig;
        public Camera mouseCamera;
        public Vector3 defaultLeftHandPosition;
        public Vector3 defaultRightHandPosition;
        public Quaternion defaultLeftHandRotation;
        public Quaternion defaultRightHandRotation;
        public bool forceFirstPersonView = false;
        public DesktopController hardwareRigControl;
        public HardwareHand grabberHand;
        public HardwareHand beamerHand;
        public RayBeamer rayBeamer;
        GameObject grabbed = null;
        
        public IMouseTeleportHover hoverListener;

        public float HAND_RANGE = 0.7f;

        float grabHandDistance = 0;

        Transform Head => rig == null ? null : rig.headset.transform;
        public InputActionReference triggerAction;

        private void Awake()
        {
            hardwareRigControl = GetComponentInParent<DesktopController>();

            if (mouseCamera == null) mouseCamera = GetComponentInChildren<Camera>();
            if (rig == null) rig = GetComponentInParent<HardwareRig>();

            defaultLeftHandPosition = Head.InverseTransformPoint(rig.leftHand.transform.position);
            defaultRightHandPosition = Head.InverseTransformPoint(rig.rightHand.transform.position);
            defaultLeftHandRotation = Quaternion.Inverse(Head.rotation) * rig.leftHand.transform.rotation;
            defaultRightHandRotation = Quaternion.Inverse(Head.rotation) * rig.rightHand.transform.rotation;
        }

        void Start()
        {
            if (forceFirstPersonView)
            {
                mouseCamera.transform.position = Head.position;
                mouseCamera.transform.rotation = Head.rotation;
            }

            // grab and teleport are done with left hand
            grabberHand = rig.leftHand;
            beamerHand = rig.leftHand;
            rayBeamer = rig.leftHand.GetComponentInChildren<RayBeamer>();
        }



        bool CheckGrab(Ray mouseRay)
        {
            bool didTouch = false;

            // No teleport/click when right click is pressed. Only rotation
            if (!Mouse.current.rightButton.isPressed && Mouse.current.leftButton.isPressed)
            {
                if (Physics.Raycast(mouseRay, out RaycastHit hit, 40f))
                {
                    // check if there is already a grabbed object
                    if (grabbed == null)
                    {

                        /*
                        // check if the hit object can be grab
                        var grabbable = hit.collider.GetComponentInParent<Grabbable>();
                        if (grabbable)
                        {
                            // the ray hit a grabbable object
                            didTouch = true;
                            grabbed = grabbable;

                            // do not display the ray
                            rayBeamer.CancelHit();

                            // We move the local hand to the hit position, and active isGrabbing
                            grabberHand.transform.position = hit.point;
                            grabberHand.isGrabbing = true;
                            grabHandDistance = (hit.point - mouseRay.origin).magnitude;


                            // TO DO Update the position of the grabbed object
                        }

*/


                        // check if the hit object can be grab
                        GameObject grabbableObject = null;
                        var grabbable = hit.collider.GetComponentInParent<Grabbable>();
                        if (grabbable)
                        {
                            grabbableObject = grabbable.gameObject;
                        }
                       /* else
                        {
                            var networkGrabbable = hit.collider.GetComponentInParent<NetworkHandColliderGrabbable>();
                            if (networkGrabbable)
                            {
                                grabbableObject = networkGrabbable.gameObject;
                            }
                        }*/
                        if (grabbableObject != null)
                        {
                            // the ray hit a grabbable object
                            didTouch = true;
                            grabbed = grabbableObject;

                            // do not display the ray
                            rayBeamer.CancelHit();

                            // We move the local hand to the hit position, and active isGrabbing
                            grabberHand.transform.position = hit.point;
                            grabberHand.isGrabbing = true;
                            grabHandDistance = (hit.point - mouseRay.origin).magnitude;

                            // TO DO Update the position of the grabbed object
                        }
                    }
                }
            }
            return didTouch;
        }

        void CheckUngrab()
        {
            // Check if we should ungrab
            if (!Mouse.current.leftButton.wasReleasedThisFrame && !Mouse.current.leftButton.isPressed)
            {
                if (grabbed != null)
                {
                    grabbed = null;
                    grabberHand.isGrabbing = false;
                }
            }
        }

        Vector3 SearchTarget(Ray mouseRay)
        {
            var target = mouseRay.origin + mouseRay.direction * 20;
            if (rayBeamer.BeamCast(out RaycastHit hit, mouseRay.origin, mouseRay.direction))
            {
                target = hit.point;
                if (hoverListener != null) hoverListener.OnHoverHit(hit);
            }
            else
            {
                if (hoverListener != null) hoverListener.OnNoHover();
            }
            return target;
        }

        public bool CheckUIInteractionWithRay(Ray vrRay)
        {
            if (EventSystem.current == null) return false;

            GraphicRaycaster[] raycasters = GameObject.FindObjectsOfType<GraphicRaycaster>();
            bool didHit = false;

            foreach (var gr in raycasters)
            {
                Canvas canvas = gr.GetComponent<Canvas>();
                if (canvas == null) continue;

                Camera uiCamera = canvas.worldCamera;
                if (uiCamera == null) continue;

                // Ray가 Canvas와 교차하는 지점 찾기
                if (Physics.Raycast(vrRay, out RaycastHit hit, 100f))
                {
                    // PointerEventData 생성
                    PointerEventData pointerData = new PointerEventData(EventSystem.current);
                    pointerData.pointerId = -1; // VR Ray용 ID

                    // Ray가 맞은 지점을 UI카메라 기준 ScreenPoint로 변환
                    pointerData.position = uiCamera.WorldToScreenPoint(hit.point);

                    List<RaycastResult> results = new List<RaycastResult>();
                    gr.Raycast(pointerData, results);

                    foreach (var result in results)
                    {
                        didHit = true;

                        bool vrTriggerPressed = triggerAction.action.WasPressedThisFrame();

                        if (Mouse.current.leftButton.wasPressedThisFrame || vrTriggerPressed)
                        {
                            ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
                        }

                    }
                }
            }

            return didHit;
        }


        void Update()
        {
            rayBeamer.isRayEnabled = false;
            var mouseRay = mouseCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Ray vrRay = new Ray(rayBeamer.origin.position, rayBeamer.origin.forward);

            // UI Ray 체크 및 클릭 시뮬레이션 (PointerEventData를 VR Ray 기준으로 처리)
            bool didUIHit = CheckUIInteractionWithRay(vrRay);

            // 손과 RayBeamer 항상 업데이트
            UpdateHandAndRay(mouseRay, vrRay, didUIHit);

            float scroll = Mouse.current.scroll.ReadValue().y; // 위로 스크롤: +, 아래로: -
            if (scroll != 0 && grabbed != null) // 물건을 잡고 있을 때만
            {
                float sensitivity = 0.02f; // 변화량을 줄여서 조금씩 움직이게 함
                float delta = scroll * sensitivity;

                grabHandDistance += delta;
                grabHandDistance = Mathf.Max(0.1f, grabHandDistance);
            }

            if (grabbed == null) grabHandDistance = 0;

            // Check if the mouse hit a grabbable object
            bool didTouch = CheckGrab(mouseRay);
            bool beamerRotationHandled = false;
            bool grabberHandPositionHandled = didTouch;
            CheckUngrab();

            Vector3 target = Vector3.zero;
            bool targetSearched = false;
            if (hoverListener != null)
            {
                // If a hover listener expect to know if a hover occured, we have to check whatever buttons are clicked
                target = SearchTarget(mouseRay);
                targetSearched = true;
            }

            if (!didTouch && grabbed == null && Mouse.current.rightButton.isPressed == false)
            {
                if (Mouse.current.leftButton.isPressed || Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    rayBeamer.isRayEnabled = true;
                    if (targetSearched == false)
                    {
                        target = SearchTarget(mouseRay);
                    }

                    beamerRotationHandled = true;
                    var currentBeamLocalRotation = Quaternion.Inverse(beamerHand.transform.rotation) * rayBeamer.origin.rotation;
                    var beamRotation = Quaternion.LookRotation(target - rayBeamer.origin.position);
                    // Explanation:
                    // We have: beamerHand.transform.rotation * currentBeamLocalRotation = rayBeamer.origin.rotation
                    // We want: rayBeamer.origin.rotation = beamRotation;
                    // So beamerHand.transform.rotation * currentBeamLocalRotation = beamRotation
                    // So beamerHand.transform.rotation * currentBeamLocalRotation * Quaternion.Inverse(currentBeamLocalRotation) = beamRotation * Quaternion.Inverse(currentBeamLocalRotation)
                    // So beamerHand.transform.rotation * Quaternion.Identity = beamRotation * Quaternion.Inverse(currentBeamLocalRotation). Simplified to:
                    beamerHand.transform.rotation = beamRotation * Quaternion.Inverse(currentBeamLocalRotation);
                }
            }

            rig.rightHand.transform.position = Head.TransformPoint(defaultRightHandPosition);
            rig.rightHand.transform.rotation = Head.rotation * defaultLeftHandRotation;

            if (grabHandDistance != 0)
            {
                grabberHandPositionHandled = true;
                rig.leftHand.transform.position = mouseRay.origin + mouseRay.direction * grabHandDistance;
            }

            if (!grabberHandPositionHandled)
            {
                rig.leftHand.transform.position = Head.TransformPoint(defaultLeftHandPosition) + mouseRay.direction * HAND_RANGE;
            }
            if (!beamerRotationHandled)
            {
                rig.leftHand.transform.LookAt(mouseRay.origin + mouseRay.direction * 2f);
                rig.leftHand.transform.Rotate(-40, 0, 0);
            }
        }

        void UpdateHandAndRay(Ray mouseRay, Ray vrRay, bool didUIHit)
        {
            // UI 위에서도 항상 손 위치/회전 업데이트
            rig.rightHand.transform.position = Head.TransformPoint(defaultRightHandPosition);
            rig.rightHand.transform.rotation = Head.rotation * defaultLeftHandRotation;

            if (grabHandDistance != 0)
            {
                rig.leftHand.transform.position = mouseRay.origin + mouseRay.direction * grabHandDistance;
            }
            else
            {
                rig.leftHand.transform.position = Head.TransformPoint(defaultLeftHandPosition) + mouseRay.direction * HAND_RANGE;
            }

            // RayBeamer 방향 처리
            if (!didUIHit)
            {
                rig.leftHand.transform.LookAt(mouseRay.origin + mouseRay.direction * 2f);
                rig.leftHand.transform.Rotate(-40, 0, 0);
            }
        }


    }

}