using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsHandler : MonoBehaviour 
{

	private static EffectsHandler instance;
	public static EffectsHandler GetInstance ()
	{
		return instance;
	}

	[SerializeField] private Slash slash;
	[SerializeField] private Effect bomb1;
	[SerializeField] private Effect bomb2;
	[SerializeField] private Effect portal1;
	[SerializeField] private Effect portal2;
	[SerializeField] private Effect portalBlur;
	[SerializeField] private Effect combo;

	void Awake ()
	{
		instance = this;
	}

	void OnDestroy ()
	{
		instance = null;
	}

	public static void PlaySlash (Vector3 pos, Vector3 dir)
	{
		if(instance != null)
			instance.slash.Create(pos, dir);
	}

	public static void PlayBomb (Vector3 pos)
	{
		if(instance != null)
		{
			instance.bomb1.Create(pos);
			instance.bomb2.Create(pos);
		}
	}

	public static void PlayPortal1 (Vector3 pos)
	{
		if(instance != null)
		{
			instance.portal1.Create(pos);
		}
	}

	public static void PlayPortal2 (Vector3 pos)
	{
		if(instance != null)
		{
			instance.portal2.Create(pos);
		}
	}

	public static void PlayBlur (Vector3 pos)
	{
		if(instance != null)
		{
			instance.portalBlur.Create(pos);
		}
	}

	public static void PlayCombo ()
	{
		if(instance != null)
		{
			if(!instance.combo.gameObject.activeSelf)
				instance.combo.Play();
		}
	}
}
