using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tweeners;


namespace Characters
{

	public class Hero : MonoBehaviour 
	{
		[SerializeField] public MoverPath mover;
		[SerializeField] public CharacterTouchControlled controller;
		[SerializeField] private Transform bodyTransform;
		[SerializeField] private GameObject booster;
		[SerializeField] private SpriteRenderer bodySprite;
		[SerializeField] private Sprite[] sprites;


		public int strikeHits = 0;

		public enum State
		{
			STANDING = 0,
			FLYING = 1,
			LANDING = 2,
			FAILED = 3
		}

		public string status = "";

		private State state = State.STANDING;

		void Start ()
		{
			mover.SetStartMovementListener(OnMoveStarted);
			controller.onCharacterLanded = OnLanded;
		}

		void Update ()
		{
			status = string.Format("attacking: " + IsAttacking() + 
			                       ", hit id: " + GetHitId());
		}

		public bool IsAttacking ()
		{
			return mover.IsPlaying() && !controller.IsReturning();
		}

		public int GetHitId ()
		{
			return controller.GetHitId();
		}

		public void SetState (State newState)
		{
			if(state == newState)
				return;

			state = newState;
		}

		private void UpdateSprites ()
		{
			if(bodySprite != null)
			{
				if ((int)state < sprites.Length)
				{
					bodySprite.sprite = sprites[(int)state];
				}

				Vector3 bodyScale = bodyTransform.localScale;
				bodyScale.x = mover.GetDirectionX();
				bodyTransform.localScale = bodyScale;

				bool needsBoost = state == State.FLYING;
				if(needsBoost != booster.activeSelf)
				{
					booster.SetActive(needsBoost);
				}
			}
		}

		private void OnMoveStarted ()
		{
			strikeHits = 0;
			if(IsAttacking())
			{
				SetState(State.FLYING);
			}
			else if(controller.IsReturning())	
			{
				SetState(State.LANDING);
			}
			else
			{
				SetState(State.STANDING);
			}
			UpdateSprites ();
		}

		private void OnLanded ()
		{
			SetState(State.STANDING);
			UpdateSprites ();
		}

		public Vector3 GetStrikePosition ()
		{
			Vector3 pos = this.transform.position;
			pos.y +=  0.7f; 	// pos is at the feet of the character
			float offset = -0.5f;
			if(mover.GetDirectionX() < 0f)
				offset = 0.5f;

			pos.x += offset;
			return pos;
		}

		public void HeroFailed ()
		{
			mover.Stop();
			SetState(State.FAILED);
			UpdateSprites();

			SetTouchActive(false);
		}

		public void StartGame ()
		{
			mover.StopAndClearPath();
			SetState(State.STANDING);
			this.transform.position = new Vector3(2f, controller.groundY, 0f);
			UpdateSprites();
			SetTouchActive(true);
		}

		public void SetTouchActive(bool touchActive)
		{
			controller.touchActive = touchActive;
		}
	}

}