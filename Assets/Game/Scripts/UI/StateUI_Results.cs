using UnityEngine;
using UnityEngine.UI;

public class StateUI_Results : StateUI
{
	[SerializeField] private Text resultText;

	public override void UpdateResults (Results result)
	{
		string text = "Score   " + result.score.ToString("N0");
		text += "\nEnemies Killed  " + result.enemiesKilled.ToString("N0");
		if(result.bombed)
		{
			text += "\n\nLost your LIFE!";
		}
		else
		{
			text += "\n\nLost your GIRL!";
		}
		resultText.text = text;

		SoundHandler.GetInstance().PlaySFX("score_counter");
	}
}
