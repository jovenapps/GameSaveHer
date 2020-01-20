using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMenu : GameState 
{
	public override State GetStateType ()
	{
		return State.MENU;	
	}
}
