using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DK
{
	public class AnimatorHandler : MonoBehaviour
	{
		public Animator animator;
		public bool canRotate;

		int vertical;
		int horizontal;

		public void Initialize()
        {
			animator = GetComponent<Animator>();
			vertical = Animator.StringToHash("Vertical");
			horizontal = Animator.StringToHash("Horizontal");
        }

		public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
			const float threshold = .55f;
			const float dampTime = .1f;

			#region Vertical
			float targetVertical = 0;

			if (verticalMovement > 0 && verticalMovement < threshold)
				targetVertical = .5f;
			else if (verticalMovement > threshold)
				targetVertical = 1;
			else if (verticalMovement < 0 && verticalMovement > -threshold)
				targetVertical = -.5f;
			else if (verticalMovement < -threshold)
				targetVertical = -1;
            #endregion

            #region Horizontal
			float targetHorizontal = 0;

			if (horizontalMovement > 0 && horizontalMovement < threshold)
				targetHorizontal = .5f;
			else if (horizontalMovement > threshold)
				targetHorizontal = 1;
			else if (horizontalMovement < 0 && horizontalMovement > -threshold)
				targetHorizontal = -.55f;
			else if (horizontalMovement < -threshold)
				targetHorizontal = 1;
			#endregion

			animator.SetFloat(vertical, targetVertical, dampTime, Time.deltaTime);
			animator.SetFloat(horizontal, targetHorizontal, dampTime, Time.deltaTime);
        }

		public void CanRotate()
        {
			canRotate = true;
        }

		public void StopRotation()
        {
			canRotate = false;
        }
    }
}
