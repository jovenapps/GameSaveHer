using UnityEngine;
using UnityEngine.UI;

public class StateUI_HighScoreInput : StateUI
{
	[SerializeField] private Text scoreText;
	[SerializeField] private InputField input;

	private bool submitDone = false;

	public override void UpdateResults (Results result)
	{
		scoreText.text = result.score.ToString("N0");
	}

	public override void Show ()
	{
		input.text = "";
		submitDone = false;
		base.Show();
	}

	public void OnSubmit ()
	{
		if(submitDone)
			return;	// Prevent spamming
		
		submitDone = true;
		GameManager.GetInstance().ShowHighScores(input.text);
	}
}
