using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEnemy : MovingEnemy 
{
	public Vector2 bounceHeight = new Vector2(6f, 10f);
	public float bounceSpeed;

	private float bounceY = 0f;
	private float t;
	private float addY;
	private float tbounce;
	public Vector3[] bouncePositions;

	public override void Play ()
	{
		base.Play();
		bounceY = Random.Range(bounceHeight.x, bounceHeight.y);
		OverrideDuration(bounceY/3f);
	}

	public override void UpdateTweener ()
	{
		if(delay > 0f)
		{
			delay -= Time.deltaTime;
			return;
		}

		timer += Time.deltaTime;
		t = Mathf.Clamp(timer / duration, 0f, 1f);
		addY = 0f;
		if(t <= 0.5f)
		{
			tbounce = Mathf.Sqrt(t/0.5f);
			addY = Mathf.Lerp(0, bounceY, tbounce);
		}
		else
		{
			tbounce = ((t-0.5f)/0.5f);
			tbounce *= tbounce;
			addY = Mathf.Lerp(bounceY, 0, tbounce);
		}
		Vector3 newPos = Vector3.Lerp (start, end, t);
		newPos.y += addY;
		this.transform.position = newPos;
		playing = (t >= 0f && t < 1f);

		if (!playing)
		{
			OnFinished ();
		}
	}

	public override void Enter ()
	{ 
		SetStartMovementListener(OnMoveStarted);
		SetState(State.ENTERING);

		SimplePathData startPath = GetRandomStartPath();
		Vector3 point1 = NormalizeVector(startPath.start);
		Vector3 point2 = NormalizeVector(startPath.end);
		this.gameObject.transform.position = point1;
		BounceToPosition(point2);

		SetFinishedListener(OnEntered);

		if(!gameObject.activeSelf)
			gameObject.SetActive(true);

	}

	protected override void OnEntered ()
	{
		SetState(State.IDLING);

		Vector3 nextPos = this.transform.position;
		nextPos = NormalizeVector(nextPos);
		if(start.x > end.x)
		{
			// Going left
			nextPos.x -= Random.Range(4f, 6f);
		}
		else
		{
			nextPos.x += Random.Range(4f, 6f);
		}
		BounceToPosition(nextPos);
		SetFinishedListener(OnStartExit);
	}

	protected override void OnStartExit ()
	{
		Vector3 nextPos = this.transform.position;
		nextPos = NormalizeVector(nextPos);
		if(nextPos.x > 0)
			nextPos.x = 12f;
		else
			nextPos.x = -12f;
		SetState(State.EXITING);
		BounceToPosition(nextPos);
		SetFinishedListener(OnExitFinished);
	}

	private void BounceToPosition (Vector3 nextPos)
	{
		nextPos = NormalizeVector(nextPos);

		Vector3 currPos = transform.position;
		float x = currPos.x;
		float xDist = Mathf.Abs(nextPos.x - x);
		List<Vector3> bpos = new List<Vector3>();
		if(xDist > 6f)
		{
			int div = Mathf.CeilToInt(xDist/4f);
			float divSpace = xDist/div;
			float dir = (x < nextPos.x) ? 1f : -1f;

			//Debug.Log("BounceToPosition: " + nextPos.x + ", div " + div + ", divSpace " + divSpace);

			for(int i=1; i <= div; i++)
			{
				Vector3 newPos = new Vector3(x + dir*divSpace * i, currPos.y, currPos.z);
				bpos.Add(newPos);
				if(i == 0)
				{
					MoveAndClearPath(newPos);
				}
				else
				{
					AddToPath(newPos);
				}
			}
		}
		else
		{
			MoveAndClearPath(nextPos);
			bpos.Add(nextPos);
		}
		SetDelay(0f);
		PlayIdle();

		bouncePositions = bpos.ToArray();
	}
}
