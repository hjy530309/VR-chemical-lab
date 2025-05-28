using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fusion.XR.Host.Locomotion
{
    /**
     * 
     * Simple locomotion system
     * - trigger a fade and rotation when input horizontal axis is above a threshold
     * - trigger a fade and teleport when Teleport method called (usually from a RayBeamer)
     * 
     * Look for child RayBeamer, to trigger Teleport on the onRelease event of the beamers
     * 
     * Ensure that there is no bounce effect, with several movements called too quickly when the user keeps pressing the associated input  
     * 
     **/

    [RequireComponent(typeof(HardwareRig))]
    public class RigLocomotion : MonoBehaviour
    {
        [Header("Snap turn")]
        public InputActionProperty leftControllerTurnAction;
        public InputActionProperty rightControllerTurnAction;

        public float debounceTime = 0.5f;
        public float snapDegree = 45f;
        public float rotationInputThreshold = 0.5f;

        [Header("Teleportation")]
        [Tooltip("Automatically found if not set")]
        public List<RayBeamer> teleportBeamers;

        bool rotating = false;
        float timeStarted = 0;

        HardwareRig rig;

        public LayerMask locomotionLayerMask = 0;

        // If locomotion constraints are needed, a ILocomotionValidationHandler can restrict them
        ILocomotionValidationHandler locomotionValidationHandler;

        // 이동
        [Header("Smooth Locomotion")]
        public InputActionProperty leftControllerMoveAction;
        public float moveSpeed = 2f;


        private void Awake()
        {
            rig = GetComponentInParent<HardwareRig>();
            locomotionValidationHandler = GetComponentInParent<ILocomotionValidationHandler>();
            leftControllerMoveAction.EnableWithDefaultXRBindings(leftBindings: new List<string> { "joystick" }); // 이동

            if (teleportBeamers.Count == 0) teleportBeamers = new List<RayBeamer>(GetComponentsInChildren<RayBeamer>());
            foreach (var beamer in teleportBeamers)
            {
                beamer.onRelease.AddListener(OnBeamRelease);
            }

            var bindings = new List<string> { "joystick" };
            //leftControllerTurnAction.EnableWithDefaultXRBindings(leftBindings: bindings);
            rightControllerTurnAction.EnableWithDefaultXRBindings(rightBindings: bindings);
        }

        private void Start()
        {
            if (locomotionLayerMask == 0)
                Debug.LogError("RigLocomotion: for locomotion to be possible, at least one layer has to be added to locomotionLayerMask, add used on locomotion surface colliders");
        }

        protected virtual void Update()
        {
            CheckSnapTurn();
            HandleSmoothLocomotion(); // 이동
        }

        protected virtual void CheckSnapTurn()
        {
            if (rotating) return;
            if (timeStarted > 0f)
            {
                // Wait for a certain amount of time before allowing another turn.
                if (timeStarted + debounceTime < Time.time)
                {
                    timeStarted = 0f;
                }
                return;
            }

            //var leftStickTurn = leftControllerTurnAction.action.ReadValue<Vector2>().x;
            var rightStickTurn = rightControllerTurnAction.action.ReadValue<Vector2>().x;

            //if (Mathf.Abs(leftStickTurn) > rotationInputThreshold)
            //{
            //    timeStarted = Time.time;
            //    StartCoroutine(Rotate(Mathf.Sign(leftStickTurn) * snapDegree));
            //}
            if (Mathf.Abs(rightStickTurn) > rotationInputThreshold)
            {
                timeStarted = Time.time;
                StartCoroutine(Rotate(Mathf.Sign(rightStickTurn) * snapDegree));
            }
        }

        IEnumerator Rotate(float angle)
        {
            timeStarted = Time.time;
            rotating = true;
            yield return rig.FadedRotate(angle);
            rotating = false;
        }

        public virtual bool ValidLocomotionSurface(Collider surfaceCollider)
        {
            // We check if the hit collider is in the locomoation layer mask
            bool colliderInLocomotionLayerMask = locomotionLayerMask == (locomotionLayerMask | (1 << surfaceCollider.gameObject.layer));
            return colliderInLocomotionLayerMask;
        }

        protected virtual void OnBeamRelease(Collider lastHitCollider, Vector3 position)
        {
            if (!enabled) return;
            // Checking potential validation handler
            if (locomotionValidationHandler != null)
            {
                var headsetPositionRelativeToRig = rig.transform.InverseTransformPoint(rig.headset.transform.position);
                Vector3 newHeadsetPosition = position + headsetPositionRelativeToRig.y * rig.transform.up;
                if (!locomotionValidationHandler.CanMoveHeadset(newHeadsetPosition)) return;
            }

            // Checking target surface layer
            if (ValidLocomotionSurface(lastHitCollider))
            {
                StartCoroutine(rig.FadedTeleport(position));
            }
        }

        //이동
        protected virtual void HandleSmoothLocomotion()
        {
            Vector2 moveInput = leftControllerMoveAction.action.ReadValue<Vector2>();
            if (moveInput.sqrMagnitude < 0.01f) return;

            // 헤드셋의 전방 방향 기준으로 이동
            Vector3 forward = rig.headset.transform.forward;
            Vector3 right = rig.headset.transform.right;

            // 수직 성분 제거 (지면에 평행하게)
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
            transform.position += moveSpeed * Time.deltaTime * moveDirection;
        }
    }
}
