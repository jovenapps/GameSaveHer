using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tweeners
{
	/// <summary>
	/// A mover that can store multiple target positions to use as a path.
	/// - Carl Joven
	/// </summary>
	public class MoverPath : Mover 
	{
		public Queue<Vector3> path = new Queue<Vector3> ();
		public Queue<float> pathSpeed = new Queue<float> ();

		public System.Action onSubPathEnd;

		public void MoveAndClearPath (Vector3 pos)
		{
			MoveAndClearPath (pos, GetSpeed ());
		}

		public void MoveAndClearPath (Vector3 pos, float speed)
		{
			//Debug.Log(this.name + "MoveAndClearPath");
			path.Clear ();
			pathSpeed.Clear ();
			StartMoving (pos, speed);
		}

		public void StopAndClearPath ()
		{
			path.Clear ();
			pathSpeed.Clear ();
			Stop();
		}

		public void AddToPath (Vector3 pos, float speed)
		{
			//Debug.Log(this.name + " AddToPath");
			if (IsPlaying ())
			{
				path.Enqueue (pos);
				pathSpeed.Enqueue (speed);
			}
			else
			{
				StartMoving (pos, speed);
			}
		}

		public void AddToPath (Vector3 pos)
		{
			AddToPath (pos, GetSpeed ());
		}

		public override void OnFinished ()
		{
			if (path.Count > 0)
			{
				ContinuePath();
			}
			else
			{
				base.OnFinished ();
			}
		}

		protected virtual void ContinuePath ()
		{
			if (path.Count > 0)
			{
				StartMoving (path.Dequeue (), pathSpeed.Dequeue());
				if(onSubPathEnd != null)
					onSubPathEnd();
			}
		}
	}
}