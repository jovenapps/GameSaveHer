using UnityEngine;

namespace Tweeners 
{
	/// <summary>
	/// A basic tweener for moving an objects transform.
	/// - Carl Joven
	/// </summary>
	public class Mover : MonoBehaviour, ITweener
	{
		[Header("Mover Settings")]
		[SerializeField] protected float speed = 1f;

		protected Vector3 start;
		protected Vector3 end;

		protected bool playing = false;
		protected float timer = 0f;
		protected float duration = 0f;
		protected float delay = 0f;

		protected System.Action onEnd;
		protected System.Action onStartMoving;

		#region Mono

		public virtual void Update ()
		{
			if (IsPlaying ())
			{
				UpdateTweener ();
			}
		}

		void OnDestroy ()
		{
			onEnd = null; 	// Ensure no references are kept
		}

		#endregion

		#region Tweeners

		public virtual void Play ()
		{
			playing = true;
			Vector2 startV2 = new Vector2 (start.x, start.y);
			Vector2 endV2 = new Vector2 (end.x, end.y);

			duration =  Vector2.Distance (startV2, endV2)/GetSpeed (); // Duration only considers 2D distance, x and y
			timer = 0f;

			if(onStartMoving != null)
				onStartMoving();
		}

		public void Stop ()
		{
			playing = false;
		}

		public void SetFinishedListener (System.Action callback)
		{
			onEnd = callback;
		}

		public void SetStartMovementListener (System.Action callback)
		{
			onStartMoving = callback;
		}

		public void SetDelay (float newDelay)
		{
			delay = newDelay;
		}

		public bool IsPlaying ()
		{
			return playing;
		}

		public float GetSpeed ()
		{
			return speed;
		}

		public virtual void UpdateTweener ()
		{
			if(delay > 0f)
			{
				delay -= Time.deltaTime;
				return;
			}
			
			timer += Time.deltaTime;
			float t = Mathf.Clamp(timer / duration, 0f, 1f);
			this.transform.position = Vector3.Lerp (start, end, t);
			playing = (t >= 0f && t < 1f);

			if (!playing)
			{
				OnFinished ();
			}
		}

		public virtual void OnFinished ()
		{
			if (onEnd != null)
			{
				onEnd ();
			}
		}

		#endregion

		public void StartMoving (Vector3 start, Vector3 end, float speed)
		{
			this.speed = speed;
			this.start = start;
			this.end = end;
			Play ();
		}

		public void StartMoving (Vector3 start, Vector3 end)
		{
			StartMoving (start, end, GetSpeed ());
		}

		public void StartMoving (Vector3 target, float speed)
		{
			this.speed = speed;
			this.start = this.transform.position;
			this.end = target;
			Play ();
		}

		public void StartMoving (Vector3 target)
		{
			StartMoving (target, GetSpeed());
		}

		public float GetDirectionX ()
		{
			if(end.x > start.x)
				return -1f;
			else
				return 1f;
		}

		// For following a target
		public void AdjustEnd (Vector3 newEnd)
		{
			end = newEnd;
		}

		protected void OverridePlaying (bool newPlaying)
		{
			playing = newPlaying;
		}

		protected void OverrideDuration (float newDuration)
		{
			duration = newDuration;
		}
	}
}
