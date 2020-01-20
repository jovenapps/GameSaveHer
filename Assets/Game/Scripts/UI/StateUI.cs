using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateUI : MonoBehaviour 
{
	public GameState.State state;

	public virtual void Show ()
	{
		//Debug.Log(this.name + " show");
		gameObject.SetActive (true);
	}

	public void Hide ()
	{
		//Debug.Log(this.name + " hide");
		gameObject.SetActive (false);
	}

	public void UpdateState (GameState.State newState)
	{
		if(state != newState)
		{
			if (gameObject.activeSelf)
				Hide ();
		}
		else
		{
			if (!gameObject.activeSelf)
				Show ();
		}
	}

	public virtual void UpdateScore (long score)
	{
	}

	public virtual void UpdateResults (Results result)
	{
	}
}
