using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedEnemy : MovingEnemy
{
	[SerializeField] GameObject bodyIdle;
	[SerializeField] GameObject bodyJump;

	public float jumpTime = 1f;	// Length of animation jump clip
	public float jumpDelay = 0.41f;
	public float postJumpDelay = 0.082f;
	public string idleClipName = "frog_monster_anim";

	public Vector3[] bouncePositions;

	protected float postJumpDelayTimer = 0f;

	#region MOVER

	public override void UpdateTweener ()
	{
		if(delay > 0f)
		{
			delay -= Time.deltaTime;
			return;
		}

		if(timer < duration)
		{
			timer += Time.deltaTime;
			float t = Mathf.Clamp(timer / duration, 0f, 1f);
			this.transform.position = Vector3.Lerp (start, end, t);

			if (timer >= duration)
			{
				postJumpDelayTimer = postJumpDelay;
			}
		}
		else if(postJumpDelayTimer > 0)
		{
			postJumpDelayTimer -= Time.deltaTime;

			if(postJumpDelayTimer <= 0)
			{
				playing = false;
				OnFinished ();
			}
		}
	}

	#endregion


	public override void Update ()
	{
		if (IsPlaying ())
		{
			UpdateTweener ();
		}

		if(attackTimer > 0f)
		{
			attackTimer -= Time.deltaTime;
			if(attackTimer <= 0f)
				Attack();
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

	protected override void ContinuePath ()
	{
		if(enemyAnim != null)
			enemyAnim.Stop();

		OverridePlaying(false);
		//Debug.Log(this.name + " ContinuePath() " + end.ToString());
		Invoke("ContinePathAfterPause", Random.Range(0.5f, 1.5f));
	}

	public override Vector3 GetBodySpritePosition ()
	{
		if (bodyIdle.activeSelf)
			return bodyIdle.transform.position;
		else if (bodyJump.activeSelf)
			return bodyJump.transform.position;
		else if (hitObject != null)
			return hitObject.transform.position;
		else
			return this.transform.position;
	}

	public override Vector3 GetCarryPosition ()
	{
		if (bodyIdle.activeSelf)
			return bodyIdle.transform.position + carryOffset;
		else if (bodyJump.activeSelf)
			return bodyJump.transform.position + carryOffset;
		else
			return hitObject.transform.position + carryOffset;
	}

	protected override Vector3 NormalizeVector (Vector3 pos)
	{
		pos.y = EnemyHandler.GROUND;
		return pos;
	}

	private void ContinePathAfterPause ()
	{
		if (path.Count > 0)
		{
			PlayIdle();
			StartMoving (path.Dequeue (), pathSpeed.Dequeue());
			OverridePlaying(true);
			SetDelay(jumpDelay);
			OverrideDuration(jumpTime - jumpDelay - postJumpDelay);

			//Debug.Log(this.name + " ContinePathAfterPause() to " + base.end.ToString());
		}
	}

	protected override void OnEntered ()
	{
		Invoke("OnEnteredDelayed", 0.5f);
	}

	protected override void OnStartExit ()
	{
		Invoke("OnStartExitDelayed", 0.5f);
	}

	protected void OnEnteredDelayed ()
	{
		//if attacker, set some delay before trying an attack
		if(data.type == EnemyData.Type.ATTACKER || forcedType == EnemyData.Type.ATTACKER)
		{
			Attack();
		}
		else
		{
			SetState(State.IDLING);

			Vector3 nextPos = GetRandomPostEntryPath();
			BounceToPosition(nextPos);
			SetFinishedListener(OnStartExit);
		}
	}

	public override void Attack ()
	{
		Girl girl = EnemyHandler.GetInstance().GetGirl();
		if(girl == null)
			return;

		if(girl.IsTaken())
			return;

		Stop();
		SetState(State.ATTACKING);
		Vector3 targetPos = girl.transform.position;
		targetPos.y = EnemyHandler.GROUND;
		BounceToPosition(targetPos);
		SetFinishedListener(OnAttackApproach);

		target = girl.transform;
	}

	protected override void OnAttackApproach ()
	{
		Debug.Log(this.name + " OnAttackApproach() " + LogPosition());
		this.transform.position = NormalizeVector(this.transform.position);
		Invoke("OnAttackApproachDelayed", 0.1f);
	}

	protected void OnAttackApproachDelayed()
	{
		Girl girl = EnemyHandler.GetInstance().GetGirl();
		bool exit = true;
		if(state == State.ATTACKING)
		{
			if(girl != null)
			{
				if(!girl.IsTaken())
				{
					Vector3 girlPos = girl.transform.position;
					girlPos.z = this.transform.position.z;
					float dist = Vector3.Distance(girlPos, transform.position);
					if(dist < 0.5f)
					{
						girl.SetCaptor(this);
						exit = false;

						Vector3 exitPos = this.transform.position;
						if(exitPos.x > 0f)
							exitPos.x = 12f;
						else
							exitPos.x = -12f;

						exitPos = NormalizeVector(exitPos);
						BounceToPosition(exitPos);
						SetState(State.KIDNAPPING);
						SetFinishedListener(OnEscaped);
					}
					else
					{
						BounceToPosition(girlPos);
						SetFinishedListener(OnAttackApproach);
					}
				}
			}
		}

		Debug.Log( this.name + " OnAttackApproachDelayed() " + this.transform.position.y);
		if(exit)
		{
			OnStartExit();
		}

	}

	protected void OnStartExitDelayed ()
	{
		Vector3 exitPos = this.transform.position;
		if(exitPos.x > 0f)
			exitPos.x = 12f;
		else
			exitPos.x = -12f;
		SetState(State.EXITING);
		exitPos = NormalizeVector(exitPos);
		BounceToPosition(exitPos);
		SetFinishedListener(OnExitFinished);

		Debug.Log(this.name + " OnStartExitDelayed() " + this.transform.position.y);
	}

	private void BounceToPosition (Vector3 nextPos)
	{
		nextPos = NormalizeVector(nextPos);

		Vector3 currPos = transform.position;
		float x = currPos.x;
		float y = nextPos.y;
		float xDist = Mathf.Abs(nextPos.x - x);
		List<Vector3> bpos = new List<Vector3>();
		if(xDist > 3f)
		{
			int div = (int)(xDist/2f);
			float divSpace = xDist/div;
			float dir = (x < nextPos.x) ? 1f : -1f;
			for(int i=1; i <= div; i++)
			{
				Vector3 newPos = new Vector3(x + dir*divSpace * i, y, currPos.z);
				newPos = NormalizeVector(newPos);
				bpos.Add(newPos);
				if(i == 1)
				{
					MoveAndClearPath(newPos);

					//Debug.Log(this.name + "BounceToPos p1 " + LogVector(newPos));
				}
				else
				{
					AddToPath(newPos);

					//Debug.Log(this.name + "BounceToPos pNext " + LogVector(newPos));
				}
			}
		}
		else
		{
			nextPos = NormalizeVector(nextPos);
			MoveAndClearPath(nextPos);
			bpos.Add(nextPos);
		}
		SetDelay(jumpDelay);
		OverrideDuration(jumpTime - jumpDelay - postJumpDelay);
		PlayIdle();

		bouncePositions = bpos.ToArray();

		//Debug.Log(this.name + "BounceToPos " + LogVector(this.end));
	}
}
