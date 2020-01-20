using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour 
{
	
	private static SoundHandler instance;
	public static SoundHandler GetInstance ()
	{
		return instance;
	}

	[SerializeField] private AudioSource bgmSource;
	[SerializeField] private AudioSource enemySfx;
	[SerializeField] private AudioSource characterSfx;
	[SerializeField] private AudioSource uiSfx;

	[SerializeField] private AudioClip[] enemyClips;
	[SerializeField] private AudioClip[] characterClips;
	[SerializeField] private AudioClip[] uiClips;

	[SerializeField] private SoundData data;


	void Awake ()
	{
		instance = this;
	}

	void OnDestroy ()
	{
		instance = null;
	}

	#region Play Sounds

	public static void PlaySlash ()
	{
		if(instance != null)
			instance.uiSfx.PlayOneShot(instance.data.slash);
	}

	public static void PlayButton ()
	{
		if(instance != null)
			instance.uiSfx.PlayOneShot(instance.data.button);
	}

	public void PlayCharSFX (string soundName)
	{
		AudioClip clip = null;
		for(int i=0; i < characterClips.Length; i++)
		{
			if(characterClips[i] == null)
				continue;

			if(soundName == characterClips[i].name)
			{
				clip = characterClips[i];
				break;
			}
		}

		if(clip != null)
			characterSfx.PlayOneShot(clip);
	}

	public void PlaySFX (string soundName)
	{
		AudioClip clip = null;
		for(int i=0; i < uiClips.Length; i++)
		{
			if(uiClips[i] == null)
				continue;

			if(soundName == uiClips[i].name)
			{
				clip = uiClips[i];
				break;
			}
		}

		if(clip != null)
			uiSfx.PlayOneShot(clip);
	}

	public void PlayEnemySFX (string soundName)
	{
		AudioClip clip = null;
		for(int i=0; i < enemyClips.Length; i++)
		{
			if(enemyClips[i] == null)
				continue;

			if(soundName == enemyClips[i].name)
			{
				clip = enemyClips[i];
				break;
			}
		}

		if(clip != null)
			enemySfx.PlayOneShot(clip);
	}

	#endregion

	public void StopBGM ()
	{
		bgmSource.Stop();
	}

	public void PlayBGM ()
	{
		bgmSource.Play();
	}
}


[System.Serializable]
public class SoundData
{
	public AudioClip slash;
	public AudioClip button;
}