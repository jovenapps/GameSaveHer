using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Basic game state for the game manager.
// - Carl Jovenn
public class GameState
{
	public enum State
	{
		MENU = 0,
		PLAY = 1,
		RESULT = 2,
		HIGH_SCORE_INPUT = 3,
		HIGH_SCORE = 4
	}

	public static Dictionary<State,GameState> CreateStates ()
	{
		// Create basic states for the game.
		Dictionary<State, GameState> stateList = new Dictionary<State, GameState> ();

		GameStateMenu menuState = new GameStateMenu ();
		GameStatePlay playState = new GameStatePlay ();
		GameStateResult resultState = new GameStateResult ();
		GameStateHighScore highScoreState = new GameStateHighScore();
		GameStateHighScoreInput highScoreInput = new GameStateHighScoreInput();

		stateList.Add (menuState.GetStateType (), menuState);
		stateList.Add (playState.GetStateType (), playState);
		stateList.Add (resultState.GetStateType (), resultState);
		stateList.Add (highScoreInput.GetStateType (), highScoreInput);
		stateList.Add (highScoreState.GetStateType (), highScoreState);

		return stateList;
	}

	public virtual State GetStateType ()
	{
		return State.MENU;
	}

	public virtual void Start (GameManager gm)
	{
		
	}

	public virtual void Update (GameManager gm)
	{
		
	}

	public virtual void End (GameManager gm)
	{
		
	}

}
