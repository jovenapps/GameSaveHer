using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Tweeners;

public class Slash : Effect 
{
	public float scaleSpeed;
	public Vector3 size;

	public float fadeTime;

	// Scales up to show, fades out to hide
	[SerializeField] private Scaler scaler;
	[SerializeField] private Fader fader;

	public override void Create (Vector3 pos)
	{
		Create(pos, Vector3.zero);
	}

	public override void Create (Vector3 pos, Vector3 dir)
	{
		Slash newSlash = Instantiate(this) as Slash;
		newSlash.transform.SetParent(this.transform.parent);
		newSlash.transform.position = pos;
		newSlash.Play();

		SpriteRenderer spr = GetComponentInChildren<SpriteRenderer>();
		spr.flipX = (dir.x > 0f);
		spr.flipY = (dir.y > 0f);
	}

	public override void SetupTweeners ()
	{
		scaler.SetSpeed(scaleSpeed);
		scaler.StartScaling(Vector3.zero, size);
		scaler.SetFinishedListener(OnEntryEnd);
		if(fader != null)
		{
			fader.SetFadeTime(fadeTime);
		}
	}

	public override void OnEntryEnd ()
	{
		base.OnEntryEnd ();
		if(fader != null)
		{
			fader.Play();
			fader.SetFinishedListener(OnEffectsEnd);
		}
	}

}
