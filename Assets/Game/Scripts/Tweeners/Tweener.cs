using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tweeners
{
	/// <summary>
	/// Tweener: Basuc functions for all tweensers (movers, scalers, rotation).
	/// - Carl Joven
	/// </summary>
	public interface ITweener 
	{
		void Play ();
		void Stop ();

		void SetFinishedListener (System.Action callback);
		void OnFinished ();

		bool IsPlaying ();
		float GetSpeed ();
		void UpdateTweener ();
	}

}