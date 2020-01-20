using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	[SerializeField] private StateUI [] uiObjects;
	[SerializeField] private GameObject instructions;
	[SerializeField] private GameObject heroSkins;

	private bool skipIntro = true;

	private static UIManager instance;
	public static UIManager GetInstance()
	{
		return instance;
	}

	void Awake ()
	{
		instance = this;
	}

	void OnDestroy ()
	{
		instance = null;
	}

	#region BUTTON CALLBACKS
	public void OnPlay ()
	{
		GameManager.GetInstance ().StartGame ();
		SoundHandler.PlayButton();
	}

	public void OnResultConfirm ()
	{
		GameManager.GetInstance ().ShowHighScoresInput ();
		SoundHandler.PlayButton();
	}

	public void OnHighScoreConfirm ()
	{
		GameManager.GetInstance ().StartMenu ();
		SoundHandler.PlayButton();
	}

	public void OnInstructionClicked ()
	{
		if(instructions != null)
		{
			instructions.gameObject.SetActive(false);
			if(heroSkins != null)
			{
				heroSkins.gameObject.SetActive(true);
			}
		}
	}

	public void OnHeroSkinsClicked ()
	{
		if(heroSkins != null)
		{
			heroSkins.gameObject.SetActive(false);
		}
	}

 	#endregion

	public void OnGameStateChange ()
	{
		GameState.State newState = GameManager.GetInstance ().GetState ();
		for (int i = 0; i < uiObjects.Length; i++)
		{
			uiObjects [i].UpdateState (newState);
		}
	}

	public void ShowInstructions ()
	{
		if(skipIntro)
			return;
		
		if(instructions != null)
		{
			instructions.gameObject.SetActive(true);
		}
	}

	public void UpdateScore (long score)
	{
		for (int i = 0; i < uiObjects.Length; i++)
		{
			uiObjects [i].UpdateScore (score);
		}
	}

	public void SetResults (Results result)
	{
		for (int i = 0; i < uiObjects.Length; i++)
		{
			uiObjects [i].UpdateResults (result);
		}
	}

}
