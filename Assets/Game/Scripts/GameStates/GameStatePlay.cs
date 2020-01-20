using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatePlay : GameState 
{

	public override State GetStateType ()
	{
		return State.PLAY;
	}

	public override void Start (GameManager gm)
	{
		UIManager.GetInstance().UpdateScore(0);
		UIManager.GetInstance().ShowInstructions();
		gm.enemyHandler.StartGame();
	}

	public override void Update (GameManager gm)
	{
		gm.enemyHandler.UpdateLevel();
	}

	public override void End (GameManager gm)
	{
	}

}
