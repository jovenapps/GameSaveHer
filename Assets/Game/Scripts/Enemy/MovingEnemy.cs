using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tweeners;

public class MovingEnemy : MoverPath, IEnemy
{
	public const float OUTER_X = 10f;

	[SerializeField] protected EnemyData data;
	[SerializeField] protected GameObject hitObject;
	[SerializeField] protected Animation enemyAnim;
	[SerializeField] protected AnimationClip idleClip;
	[SerializeField] protected AnimationClip defeatClip;
	[SerializeField] protected AnimationClip hitClip;
	[SerializeField] protected Vector3 carryOffset;
	[SerializeField] protected Scaler scaler;

	public enum State
	{
		ENTERING = 0,
		IDLING = 1,
		EXITING = 2,
		ATTACKING = 3,
		KIDNAPPING = 4,
		DEAD = 5
	}

	public long id = -1;
	public int spawnControllerId = 0;

	public EnemyData.Type forcedType = EnemyData.Type.PASSERBY;

	protected int hp = 1;
	protected int hitId = -1;

	protected float squareSize = 1f;

	protected State state = State.ENTERING;
	protected float attackTimer = 0f;

	protected Transform target;
	protected int dataId;

	public override void Update ()
	{
		base.Update();

		if(attackTimer > 0f)
		{
			attackTimer -= Time.deltaTime;
			if(attackTimer <= 0f)
				Attack();
		}

		if(target != null && state == State.ATTACKING)
		{
			Vector3 targetPos = target.transform.position;
			targetPos.y -= carryOffset.y;
			AdjustEnd(targetPos);
		}
	}


	#region ENEMY 

	public int GetDataId ()
	{
		return dataId;
	}

	// Enemy creation 
	public virtual IEnemy Create (Transform parent, Transform target)
	{
		MovingEnemy newEnemy = Instantiate (this, parent);
		newEnemy.hp = data.life;
		newEnemy.dataId = (int)this.id;
		return (IEnemy)newEnemy;
	}

	public long GetId ()
	{
		return id;
	}

	public void SetId (long id)
	{
		this.id = id;
	}

	public int GetControllerId ()
	{
		return spawnControllerId;
	}

	public virtual Vector3 GetBodySpritePosition ()
	{
		if(hitObject != null)
			return hitObject.transform.position;
		else
			return this.transform.position;
	}

	public Vector3 GetPosition ()
	{
		if(hitObject != null)
			return GetBodySpritePosition();
		else
			return this.transform.position;
	}

	public virtual Vector3 GetCarryPosition ()
	{
		if(hitObject != null)
			return hitObject.transform.position + carryOffset;
		else
			return this.transform.position  + carryOffset;
	}

	public bool CheckHit (Vector3 heroPos, int hitId)
	{
		if(this.hitId == hitId || !IsAlive())
			return false; 

		// Hit id is used to allow one hero movement per hit 
		// (since each update scans for hits, a hero moving across an enemy may trigger multiple hits.
		Vector3 pos = GetBodySpritePosition();
		if (EnemyUtils.CheckBoxHit(pos, heroPos, data.size))
		{
			if( EnemyUtils.CheckDistanceHit(pos, heroPos, data.size) )
			{
				this.hitId = hitId;
				return true;
			}
		}
		return false;
	}

	public int GetHP ()
	{
		return hp;
	}

	public void Hit ()
	{
		if(hp > 0)
			hp--;
	}

	public bool IsAlive ()
	{
		return hp > 0 && state != State.DEAD;
	}

	public int GetHitScore ()
	{
		return data.hitScore;
	}

	public int GetKillScore ()
	{
		return data.killScore;
	}

	public void DestroyObject ()
	{
		ReleaseGirl (false);
		Stop();
			
		this.gameObject.SetActive(false);
		GameObject.Destroy(this.gameObject);
	}

	public virtual void Enter ()
	{ 
		SetStartMovementListener(OnMoveStarted);
		SimplePathData startPath = GetRandomStartPath();
		startPath = NormalizePath(startPath);
		this.transform.position = startPath.start;
		AddToPath(startPath.end, data.speed);
		SetFinishedListener(OnEntered);

		this.gameObject.SetActive(true);
	}

	public virtual void Enter (Vector3 startPos, float delay)
	{ 
		transform.position = NormalizeVector(startPos);
		Invoke("OnEntered", delay);
	}

	public virtual void EnterFromCarrier (Vector3 startPos, Vector3 endPos, float delay)
	{ 
		transform.position = NormalizeVector(startPos);
		StartMoving(endPos);
		SetFinishedListener(OnEntered);

		if(scaler != null)
		{
			scaler.gameObject.transform.localScale = Vector3.zero;
			scaler.StartScaling(Vector3.zero, Vector3.one);
		}

		this.gameObject.SetActive(true);
		//Debug.Log(name + "EnterFromCarrier(), endPos " + endPos.ToString());
	}

	public virtual void Kill ()
	{
		ReleaseGirl();

		// Destroy this enemy
		SetState(State.DEAD);
		Stop();
		PlayDefeat();
		Invoke("DestroyObject", 0.66f);
	}

	public virtual void PlayIdle ()
	{
		if(enemyAnim != null && idleClip != null)
		{
			//Debug.Log(this.name +".PlayIdle()");
			enemyAnim.clip = idleClip;
			enemyAnim.Play();
		}
	}

	public virtual void PlayDefeat ()
	{
		if(enemyAnim != null && defeatClip != null)
		{
			enemyAnim.clip = defeatClip;
			enemyAnim.Play();
		}
	}

	public virtual void PlayHit ()
	{
		if(enemyAnim != null && hitClip != null)
		{
			enemyAnim.clip = hitClip;
			enemyAnim.Play();
		}
	}

	protected virtual void ReleaseGirl (bool saved = true)
	{
		if(state == State.KIDNAPPING)
		{
			Girl girl = EnemyHandler.GetInstance().GetGirl();
			if(girl.IsTaken())
			{
				girl.Release(this, saved);
			}
		}
	}

	protected virtual void OnEntered ()
	{
		Vector3 nextPos = GetRandomPostEntryPath();
		nextPos = NormalizeVector(nextPos);
		MoveAndClearPath(nextPos);
		SetFinishedListener(OnStartExit);

		SetState(State.IDLING);

		// if attacker, set some delay before trying an attack
		if(data.type == EnemyData.Type.ATTACKER || forcedType == EnemyData.Type.ATTACKER)
		{
			attackTimer = UnityEngine.Random.Range(0.5f, 3f);
		}

		//Debug.Log(name + "OnEntered()");
	}

	protected virtual void OnStartExit ()
	{
		Vector3 exitPos = GetRandomExitPath();
		exitPos = NormalizeVector(exitPos);
		SetState(State.EXITING);
		MoveAndClearPath(exitPos);
		SetFinishedListener(OnExitFinished);
	}

	protected void OnExitFinished ()
	{
		SendMessageUpwards("OnEnemyExit", this);
	}

	protected virtual void OnMoveStarted ()
	{
		Transform child = this.transform.GetChild(0);
		if(child == null)
			return; 

		Vector3 childScale = child.transform.localScale;
		if(GetDirectionX() < 0f)
		{
			if(childScale.x > 0f)
				childScale.x *= -1f;
		}
		else
		{
			if(childScale.x < 0f)
				childScale.x *= -1f;	
		}
		child.transform.localScale = childScale	;
	}

	protected SimplePathData GetRandomStartPath ()
	{
		if(data.startingPaths != null && data.startingPaths.Length > 0)
		{
			int rand = Random.Range(0, data.startingPaths.Length);
			return data.startingPaths[rand];
		}
		else
		{
			return new SimplePathData();
		}
	}

	protected Vector3 GetRandomPostEntryPath ()
	{
		if(data.pathsAfterEntrance != null && data.pathsAfterEntrance.Length > 0)
		{
			int rand = Random.Range(0, data.pathsAfterEntrance.Length);
			return data.pathsAfterEntrance[rand];
		}
		else
		{
			return Vector3.zero;
		}
	}

	protected Vector3 GetRandomExitPath ()
	{
		if(data.exitPositions != null && data.exitPositions.Length > 0)
		{
			int rand = Random.Range(0, data.exitPositions.Length);
			return data.exitPositions[rand];
		}
		else
		{
			return Vector3.zero;
		}
	}

	protected Vector3 GetNearExitPath ()
	{
		float x = OUTER_X;
		if(this.transform.position.x < 0f)
			x = -OUTER_X;
		
		if(data.exitPositions != null && data.exitPositions.Length > 0)
		{
			List<Vector3> choices = new List<Vector3>();
			for(int i=0; i < data.exitPositions.Length; i++)
			{
				if((x <= 0f && data.exitPositions[i].x <= 0f) || (x > 0f && data.exitPositions[i].x > 0f))
				{
					choices.Add(data.exitPositions[i]);
				}
			}

			int randIdx = UnityEngine.Random.Range(0, choices.Count);
			if(randIdx < choices.Count)
				return choices[randIdx];
		}

		return new Vector3(x, 0f, 0f);
	}

	protected void SetState (State newState)
	{
		state = newState;
	}

	public bool IsBomb ()
	{
		return data.type == EnemyData.Type.BOMB;
	}

	#endregion

	protected virtual Vector3 NormalizeVector (Vector3 pos)
	{
		if(pos.y < EnemyHandler.GROUND)
			pos.y = EnemyHandler.GROUND;
		return pos;
	}

	protected SimplePathData NormalizePath (SimplePathData path)
	{
		path.start = NormalizeVector(path.start);
		path.end = NormalizeVector(path.end);
		return path;
	}

	public void SetAttackTimer (float timer)
	{
		forcedType = EnemyData.Type.ATTACKER;
		attackTimer = timer;
	}

	public virtual void Attack ()
	{
		Girl girl = EnemyHandler.GetInstance().GetGirl();
		if(girl == null)
			return;

		if(girl.IsTaken())
			return;

		Stop();
		SetState(State.ATTACKING);
		Vector3 targetPos = girl.transform.position;
		targetPos.y -= carryOffset.y;
		StartMoving(targetPos, data.attackSpeed);
		SetFinishedListener(OnAttackApproach);

		target = girl.transform;
	}

	protected virtual void OnAttackApproach ()
	{
		Girl girl = EnemyHandler.GetInstance().GetGirl();
		bool exit = true;
		if(state == State.ATTACKING)
		{
			if(girl != null)
			{
				if(!girl.IsTaken())
				{
					girl.SetCaptor(this);
					exit = false;

					Vector3 exitPos = GetNearExitPath();
					exitPos = NormalizeVector(exitPos);
					StartMoving(exitPos);
					SetState(State.KIDNAPPING);
					SetFinishedListener(OnEscaped);
				}
			}
		}

		if(exit)
		{
			OnStartExit();
		}
	}

	protected void OnEscaped ()
	{
	}

	public virtual string LogPosition ()
	{
		return LogVector(this.transform.position);
	}

	public static string LogVector (Vector3 pos)
	{
		return string.Format("({0:n2}, {1:n2})", pos.x, pos.y);
	}

}
