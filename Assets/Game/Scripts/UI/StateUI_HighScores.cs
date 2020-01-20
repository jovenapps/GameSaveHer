using UnityEngine;
using UnityEngine.UI;

public class StateUI_HighScores : StateUI
{
	[SerializeField] private Text scoreText;

	public override void Show ()
	{
		base.Show();

		scoreText.text = GameManager.GetInstance().GetHighScoreLog();
	}
}
