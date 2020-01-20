using UnityEngine;
using UnityEngine.SceneManagement;

using Tweeners;

public class SplashManager : MonoBehaviour 
{
	[SerializeField] private Fader logoFader;
	[SerializeField] private float transitionTime;


	void Start ()
	{
		logoFader.Play();
		Invoke("StartTransition", transitionTime);
	}

	void StartTransition ()
	{
		SplashSceneManager.StartGame();
	}
}

public class SplashSceneManager 
{
	public static void StartGame ()
	{
		SceneManager.LoadScene("UIScene", LoadSceneMode.Single);
		SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
	}
}