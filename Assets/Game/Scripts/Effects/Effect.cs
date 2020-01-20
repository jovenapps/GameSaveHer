using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
	public float effectTime = 1f;
	public bool destroyOnEnd = true;

	void OnEnable ()
	{
		SetupTweeners();
	}

	public virtual void Create (Vector3 pos)
	{
		Create(pos, Vector3.zero);
	}

	public virtual void Create (Vector3 pos, Vector3 dir)
	{
		Effect newEffect = Instantiate(this) as Effect;
		newEffect.transform.SetParent(this.transform.parent);
		newEffect.transform.position = pos;
		newEffect.Play();
	}

	public virtual void Play ()
	{
		this.gameObject.SetActive(true);
		Invoke("OnEffectsEnd", effectTime);
	}

	public virtual void SetupTweeners ()
	{
	}

	public virtual void OnEntryEnd ()
	{
	}

	public virtual void OnEffectsEnd ()
	{
		if(destroyOnEnd)
			GameObject.DestroyImmediate(this.gameObject);
		else
			this.gameObject.SetActive(false);
	}
}
