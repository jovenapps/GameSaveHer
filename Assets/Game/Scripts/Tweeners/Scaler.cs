using UnityEngine;

namespace Tweeners 
{
	/// <summary>
	/// A basic tweener for scaling objects
	/// - Carl Joven
	/// </summary>
	public class Scaler : MonoBehaviour, ITweener
	{
		[SerializeField] private float speed = 1f;

		[SerializeField] private Vector3 start;
		[SerializeField] private Vector3 end;

		private bool playing = false;
		private float timer = 0f;
		private float duration = 0f;

		protected System.Action onEnd;

		#region MONO

		void Start ()
		{
		}

		void OnEnable ()
		{

		}

		void Update ()
		{
			if(!playing)
				return;

			timer += Time.deltaTime;
			float t = timer/duration;
			playing = t < 1f;

			if(t > 1f)
				t = 1f;

			Vector3 newScale = Vector3.Lerp(start, end, t);
			transform.localScale = newScale;

			if(!playing)
			{
				OnFinished();
			}
		}

		#endregion

		#region Tweener

		public void Play ()
		{
			playing = true;
			timer = 0f;
			float scaleDistance = Vector3.Distance(start, end);
			duration = scaleDistance/speed;
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
			return speed;
		}

		public void UpdateTweener ()
		{

		}

		#endregion

		public void StartScaling (Vector3 endScale)
		{
			start = this.transform.localScale;
			end = endScale;
			Play();
		}

		public void StartScaling (Vector3 startScale, Vector3 endScale)
		{
			start = startScale;
			end = endScale;
			Play();
		}

		public void SetSpeed (float newSpeed)
		{
			speed = newSpeed;
		}
	}
}