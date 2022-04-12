using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DK
{
    public class PlayerLocomotion : MonoBehaviour
    {
        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [HideInInspector]
        public Transform myTransform;
        [HideInInspector]
        public AnimatorHandler animatorHandler;

        Transform cameraObject;
        InputHandler inputHandler;
        Vector3 movementDirection;

        [Header("Stats")]
        [SerializeField]
        float movementSpeed = 5;
        [SerializeField]
        float rotationSpeed = 10;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();

            myTransform = transform;
            cameraObject = Camera.main.transform;

            animatorHandler.Initialize();
        }

        private void Update()
        {
            inputHandler.UpdateInput(Time.deltaTime);

            movementDirection = cameraObject.forward * inputHandler.vertical;
            movementDirection += cameraObject.right * inputHandler.horizontal;
            movementDirection.Normalize();
            movementDirection *= movementSpeed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(movementDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

            if (animatorHandler.canRotate)
                HandleRotation(Time.deltaTime);
        }

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        private void HandleRotation(float delta)
        {
            float moveOverride = inputHandler.moveAmount;

            Vector3 targetDirection;
            targetDirection = cameraObject.forward * inputHandler.vertical;
            targetDirection += cameraObject.right * inputHandler.horizontal;
            targetDirection.Normalize();
            targetDirection.y = 0;

            if (targetDirection == Vector3.zero)
                targetDirection = myTransform.forward;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            targetRotation = Quaternion.Slerp(myTransform.rotation, targetRotation, rotationSpeed * delta);

            myTransform.rotation = targetRotation;
        }
        #endregion
    }
}
