using UnityEngine;

namespace Tweeners 
{
	/// <summary>
	/// A basic tweener for fading sprites
	/// - Carl Joven
	/// </summary>
	public class Fader : MonoBehaviour, ITweener
	{
		public enum FadeType
		{
			FADE_IN = 0,
			FADE_OUT = 1
		}

		[SerializeField] public FadeType type = FadeType.FADE_OUT;
		[SerializeField] private float duration = 1f;

		private float start;
		private float end;

		private bool playing = false;
		private float timer = 0f;
		private SpriteRenderer spriteRenderer;

		protected System.Action onEnd;

		#region MONO

		void Start ()
		{
			spriteRenderer = this.transform.GetComponent<SpriteRenderer>();
		}

		void OnEnable ()
		{
			spriteRenderer = this.transform.GetComponent<SpriteRenderer>();
		}

		void Update ()
		{
			if(spriteRenderer == null)
				return;

			if(!playing)
				return;

			timer += Time.deltaTime;
			float t = timer/duration;
			playing = t < 1f;

			if(t > 1f)
				t = 1f;

			float alpha = Mathf.Lerp(start, end, t);
			Color c = spriteRenderer.color;
			c.a = alpha;
			spriteRenderer.color = c;

			if(!playing)
			{
				OnFinished();
			}
		}

		#endregion

		#region Tweener

		public void Play ()
		{
			if(type == FadeType.FADE_OUT)
			{
				start = 1f;
				end = 0f;
			}
			else
			{
				start = 0f;
				end = 1f;
			}
			playing = true;
			timer = 0f;
		}

		public void Stop ()
		{
			playing = false;
		}

		public void SetFinishedListener (System.Action callback)
		{
			onEnd = callback;
		}

		public void OnFinished ()
		{
			if(onEnd != null)
				onEnd();
		}

		public bool IsPlaying ()
		{
			return playing;
		}

		public float GetSpeed ()
		{
			return duration;
		}

		public void UpdateTweener ()
		{
			
		}

		#endregion

		public void SetFadeTime (float time)
		{
			duration = time;
		}
	}
}