using UnityEngine.InputSystem;
using UnityEngine;
using Fusion.XR.Host.Rig;
using Fusion.XR.Host.Grabbing;
using Fusion.XR.Host.Locomotion;

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
        public GameObject weaponObject;
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

        public const float HAND_RANGE = 0.7f;

        float grabHandDistance = 0;

        Transform Head => rig == null ? null : rig.headset.transform;

        private float handZOffset = 0f;
        private Quaternion previousCameraRotation;

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
            previousCameraRotation = mouseCamera.transform.rotation;
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

                    // 손을 원래 위치로 되돌리기
                    handZOffset = 0f;

                    // 손 회전 초기화
                    rig.leftHand.transform.localRotation = defaultLeftHandRotation;
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

        void Update()
        {
            rayBeamer.isRayEnabled = false;
            var mouseRay = mouseCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            // Storing the distance before checking ungrab, as we want to reset the hand position at ungrab during the next Update
            //  so that the grabbing system has the time to drop it where it is
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
                        target = SearchTarget(mouseRay);

                    beamerRotationHandled = true;

                    if (hoverListener != null) // 마우스가 어떤 타겟을 가리키는 경우
                    {
                        // 실제 레이 캐스트에 히트한 경우에만 회전
                        if (Physics.Raycast(mouseRay, out RaycastHit hit, 40f))
                        {
                            var currentBeamLocalRotation = Quaternion.Inverse(beamerHand.transform.rotation) * rayBeamer.origin.rotation;
                            var beamRotation = Quaternion.LookRotation(hit.point - rayBeamer.origin.position);
                            beamerHand.transform.rotation = beamRotation * Quaternion.Inverse(currentBeamLocalRotation);
                        }
                        else
                        {
                            // 히트하지 않으면 손을 기본 회전으로
                            beamerHand.transform.localRotation = defaultLeftHandRotation;
                        }
                    }
                    else
                    {
                        // hoverListener가 없으면 기본 회전
                        beamerHand.transform.localRotation = defaultLeftHandRotation;
                    }
                }
            }


            rig.rightHand.transform.position = Head.TransformPoint(defaultRightHandPosition);
            rig.rightHand.transform.rotation = Head.rotation * defaultLeftHandRotation;

            if (grabHandDistance != 0)
            {
                grabberHandPositionHandled = true;
                rig.leftHand.transform.position = mouseRay.origin + mouseRay.direction * (grabHandDistance + handZOffset);
            }

            if (!grabberHandPositionHandled)
            {
                Vector3 handOffset = defaultLeftHandPosition;
                Vector3 targetDir = (mouseRay.GetPoint(HAND_RANGE) - Head.position).normalized;
                rig.leftHand.transform.position = Head.position + Head.rotation * handOffset + targetDir * HAND_RANGE;
                rig.leftHand.transform.rotation = Quaternion.LookRotation(targetDir) * defaultLeftHandRotation;
            }

            
            if (grabberHand.isGrabbing)
            {
                // 회전 처리
                float rotationSpeed = 100f * Time.deltaTime;
                if (Input.GetKey(KeyCode.Alpha1))
                    rig.leftHand.transform.Rotate(rotationSpeed, 0f, 0f, Space.World);
                else if (Input.GetKey(KeyCode.Alpha2))
                    rig.leftHand.transform.Rotate(-rotationSpeed, 0f, 0f, Space.World);
                            // 물건 회전 초기화
                if (Input.GetKeyDown(KeyCode.Alpha3) && grabbed != null)
                {
                    Quaternion deltaRotation = Quaternion.Inverse(grabbed.transform.rotation);

                    // 손도 같은 delta만큼 회전
                    rig.leftHand.transform.rotation = deltaRotation * rig.leftHand.transform.rotation;
                }

                // 마우스 휠 이동 처리
                float scroll = Mouse.current.scroll.ReadValue().y;
                if (Mathf.Abs(scroll) > 0.01f)
                {
                    float moveSpeed = 0.001f;
                    handZOffset += scroll * moveSpeed;
                }
            }

            if (grabbed == null && Mouse.current.rightButton.isPressed && weaponObject != null)
            {
                Quaternion cameraDelta = mouseCamera.transform.rotation * Quaternion.Inverse(previousCameraRotation);
    
                // 기존 총에 회전 delta 적용
                weaponObject.transform.rotation = cameraDelta * weaponObject.transform.rotation;
                
                // 이전 카메라 회전 갱신
                previousCameraRotation = mouseCamera.transform.rotation;

            }

        }

    }

}