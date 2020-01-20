using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateHighScoreInput : GameState
{
	public override State GetStateType ()
	{
		return State.HIGH_SCORE_INPUT;
	}


}
