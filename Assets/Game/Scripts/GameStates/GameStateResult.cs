using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateResult : GameState
{
	public override State GetStateType ()
	{
		return State.RESULT;
	}


}
