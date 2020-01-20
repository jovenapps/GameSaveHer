using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tweeners;

namespace Characters
{
	/// <summary>
	/// Character touch controlled - Moves the character on points touched on the screen.
	/// - Carl Joven
	/// </summary>
	public class CharacterTouchControlled : MonoBehaviour
	{
		//[SerializeField] private GameObject vipObject;
		[Header("Character Speed Settings")]
		[SerializeField] private float attackSpeed;
		[SerializeField] private float returnSpeed;
		[SerializeField] public float groundY = -4f;

		private MoverPath pathMover;
		private bool returning = false;

		private int hitId = 0;

		public bool touchActive = false;

		public System.Action onCharacterLanded = null;

		#region Mono

		// Use this for initialization
		void Start ()
		{
			TouchListener.AddTouchListener (this.gameObject);
			pathMover = GetComponent<MoverPath> ();
			pathMover.SetFinishedListener (OnMoveEnd);
		}

		void OnDestroy ()
		{
			TouchListener.RemoveTouchListener (this.gameObject);
		}

		// Update is called once per frame
		void Update ()
		{

		}

		#endregion

		#region Touch listener

		public void OnTouch (Vector3 pos)
		{
			if(!touchActive)
				return;
			
			pos.z = this.transform.position.z;
			bool prevReturning = returning;
			returning = false;
			pathMover.SetFinishedListener (OnMoveEnd);
			IncrementHitId();

			if (prevReturning)
			{
				pathMover.MoveAndClearPath (pos, attackSpeed);
			}
			else
			{
				pathMover.AddToPath (pos, attackSpeed);
			}
		}

		public void OnMoveEnd ()
		{
			LandToGround();
		}

		public void OnLanded ()
		{
			if(onCharacterLanded != null)
				onCharacterLanded();
		}

		private void LandToGround ()
		{
			Vector3 currentPos = this.transform.position;
			float diffY = Mathf.Abs(groundY - currentPos.y);
			if(diffY < 0.1f)
			{
				OnLanded();
				return;
			}

			returning = true;
			Vector3 newPos = new Vector3(currentPos.x, groundY, currentPos.z);
			pathMover.AddToPath (newPos, returnSpeed);
			pathMover.SetFinishedListener (OnMoveEnd);
		}

		/*
		private void GoToGirl ()
		{
			Vector3 targetPos = vipObject.transform.position;
			Vector3 currentPos = this.transform.position;
			bool isVertical = (Mathf.Abs (currentPos.y - targetPos.y) > Mathf.Abs (currentPos.x - targetPos.x));
			float nearDistance = isVertical ? 0.8f : 1.2f;

			Vector3 heading = targetPos - currentPos;
			float dist = heading.magnitude;

			Vector3 direction = heading / dist;
			dist -= nearDistance;
			Vector3 nearTarget = direction * dist;

			hitId++;

			if (Mathf.Abs (dist) < 0.2f)
			{
				returning = false;
				return;
			}

			Vector3 newPos = this.transform.position + nearTarget;
			Vector3 newHeading = targetPos - currentPos;
			if (newHeading.magnitude < 0.2f)
			{
				returning = false;
				return;
			}

			returning = true;
			pathMover.AddToPath (newPos, returnSpeed);
		}
		*/

		#endregion

		public bool IsReturning ()
		{
			return returning;
		}

		public int GetHitId ()
		{
			return hitId;
		}

		private void IncrementHitId ()
		{
			if(int.MaxValue > hitId)
				hitId++;
			else
				hitId = 0;
		}
	}

}
