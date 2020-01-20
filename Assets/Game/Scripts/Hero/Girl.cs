using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tweeners;

public class Girl : MonoBehaviour 
{
	public const float OUTER_X = 9.8f;

	[SerializeField] private Mover mover;
	[SerializeField] private float groundY = -4f;
	[SerializeField] private GameObject[] stateObjects;

	public enum State 
	{
		IDLE = 0,
		TAKEN = 1,
		LANDING = 2,
		LOST = 3
	}

	private IEnemy captor = null;
	private State state = State.IDLE;


	void Update ()
	{
		if(captor != null)
		{
			this.transform.position = captor.GetCarryPosition();

			if(state == State.TAKEN)
			{
				float x = this.transform.position.x;
				if(x < -OUTER_X || x > OUTER_X)
				{
					LostTheGirl();
				}
			}
		}
	}

	public bool IsTaken ()
	{
		return (captor != null && state == State.TAKEN) || state == State.LOST;
	}

	public void SetCaptor (IEnemy enemy)
	{
		captor = enemy;
		if(captor != null)
		{
			SetState(State.TAKEN);
		}
	}

	public void Release (IEnemy assumedCaptor, bool saved = true)
	{
		if(captor == assumedCaptor)
		{
			captor = null;
			 
			// now move to ground
			Vector3 pos = this.transform.position;
			pos.y = groundY;
			mover.StartMoving(pos);
			mover.SetFinishedListener(OnLanded);

			if(saved)
				SoundHandler.GetInstance().PlayCharSFX("girl_saved");
		}
	}

	private void OnLanded ()
	{
		SetState(State.IDLE);
	}

	private void SetState (State newState)
	{
		state = newState;

		int stateIdx = (int)state;
		if(stateIdx < 0 || stateIdx >= stateObjects.Length)
		{
			stateIdx = 0;
		}

		for(int i=0; i < stateObjects.Length; i++)
		{
			if(stateIdx != i)
				stateObjects[i].SetActive(false);
		}
		stateObjects[stateIdx].SetActive(true);
	}

	private void LostTheGirl ()
	{
		SetState(State.LOST);
		EnemyHandler.GetInstance().LostTheGirl ();
	}

	public void StartGame ()
	{
		mover.Stop();
		captor = null;
		SetState(State.IDLE);
		this.transform.position = new Vector3(0f, -4f, 0f);
	}

}
