using UnityEngine;
using UnityEngine.UI;

public class StateUI_Play : StateUI
{
	[SerializeField] private Text scoreText;

	public override void UpdateScore (long score)
	{
		scoreText.text = score.ToString("N0");
	}
}
