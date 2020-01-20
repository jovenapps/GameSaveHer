using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UFOEnemy : MovingEnemy
{
	[Header("UFO Ray Object")]
	[SerializeField] private GameObject ray;

	private Vector3 DOWN = new Vector3(0f, -1f, 0f);
	private Vector3 prevTarget = Vector3.zero;

	public float girlMovementSpeed = 2f;
	private bool takingGirl = false;
	private Vector3 girlStartPos;
	private float girlMovementTimer = 0f;
	private float girlMovementDuration = 0f;
	private float girlT;

	public override void Update ()
	{
		if(takingGirl)
		{
			girlMovementTimer += Time.deltaTime;
			girlT = girlMovementTimer/girlMovementDuration;
			if(girlT > 1f)
			{
				girlT = 1f;
				takingGirl = false;
				OnStartExit();
			}

			target.position = Vector3.Lerp(girlStartPos, ray.transform.position, girlT);
		}
		else
		{
			base.Update();
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
		target = girl.transform;
		Vector3 targetPos = target.position - ray.transform.position;
		SetRayAngle(targetPos);

		girl.SetCaptor(this);
		takingGirl = true;
		girlStartPos = target.position;
		girlMovementTimer = 0f;
		girlMovementDuration = Vector3.Distance(ray.transform.position, girlStartPos)/girlMovementSpeed;
	}

	private void SetRayAngle (Vector3 targetPos)
	{
		if(Vector3.Distance(prevTarget, targetPos) < 0.001f)
			return;
		
		Vector3 a = DOWN;
		Vector3 b = targetPos;
		float angle = Vector3.Angle(a,b);
		if(b.x > 0f)
			angle *= -1f;
		
		SetRayAngle (angle);
		prevTarget = targetPos;
	}

	private void SetRayAngle (float angle)
	{
		Vector3 angles = ray.transform.eulerAngles;
		angles.z = -angle;
		ray.transform.eulerAngles = angles;

		if (!ray.gameObject.activeSelf)
		{
			ray.gameObject.SetActive(true);
		}
	}

}
