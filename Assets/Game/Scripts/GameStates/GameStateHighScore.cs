using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateHighScore : GameState
{
	public override State GetStateType ()
	{
		return State.HIGH_SCORE;
	}


}
