using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DK
{
    public class CameraHandler : MonoBehaviour
    {
        public static CameraHandler Instance { get; private set; }

        public enum FollowInterpolationType
        {
            Lerp,
            SmoothDamp
        }

        [SerializeField]
        Transform targetTransform;
        [SerializeField]
        Transform cameraTransform;
        [SerializeField]
        Transform cameraPivotTransform;

        [SerializeField]
        float yawSpeed = 360f;
        [SerializeField]
        float pitchSpeed = 360f;
        [SerializeField]
        float followSpeed = 10f;

        [SerializeField]
        FollowInterpolationType followInterpolationType = FollowInterpolationType.Lerp;

        [SerializeField]
        float minimumPitch = -35;
        [SerializeField]
        float maximumPitch = 35;

        float defaultDistance;
        float targetDistance;
        float yawAngle;
        float pitchAngle;

        [SerializeField]
        float cameraSphereRadius = .2f;
        [SerializeField]
        float cameraCollisionOffset = .2f;
        [SerializeField]
        float minimumCollisionOffset = .2f;

        Vector3 cameraCurrentVelocity = Vector3.zero;

        LayerMask ignoreLayers;

        private void Awake()
        {
            Instance = this;
            defaultDistance = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        public void FollowTarget(float delta)
        {
            Vector3 targetPosition;
            switch (followInterpolationType)
            {
                case FollowInterpolationType.Lerp:
                    targetPosition = Vector3.Lerp(transform.position, targetTransform.position, followSpeed * delta);
                    break;
                case FollowInterpolationType.SmoothDamp:
                    targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraCurrentVelocity, followSpeed * delta);
                    break;
                default:
                    targetPosition = Vector3.Lerp(transform.position, targetTransform.position, followSpeed * delta);
                    break;
            }
            transform.position = targetPosition;

            HandleCameraCollision(delta);
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            yawAngle += mouseXInput * yawSpeed * delta;
            pitchAngle -= mouseYInput * pitchSpeed * delta;
            pitchAngle = Mathf.Clamp(pitchAngle, minimumPitch, maximumPitch);

            // Apply rotation to CameraHandler
            Vector3 rotation = Vector3.zero;
            rotation.y = yawAngle;
            transform.rotation = Quaternion.Euler(rotation);

            // Apply rotation to CameraPivot
            rotation = Vector3.zero;
            rotation.x = pitchAngle;
            cameraPivotTransform.localRotation = Quaternion.Euler(rotation);
        }

        public void HandleCameraCollision(float delta)
        {
            targetDistance = defaultDistance;

            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out RaycastHit hit, Mathf.Abs(defaultDistance), ignoreLayers))
            {
                float distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetDistance = -distance + cameraCollisionOffset;
            }

            if (Mathf.Abs(targetDistance) < minimumCollisionOffset)
            {
                targetDistance = -minimumCollisionOffset;
            }

            float localPositionZ = Mathf.Lerp(cameraTransform.localPosition.z, targetDistance, followSpeed * delta);
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraTransform.localPosition.y, localPositionZ);
        }
    }
}
