using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy 
{
	// Enemy creation 
	IEnemy Create (Transform parent, Transform target);

	int GetDataId ();

	long GetId ();

	int GetControllerId ();

	void SetId (long id);

	void Enter ();

	void Kill ();

	Vector3 GetPosition ();
	Vector3 GetCarryPosition ();

	bool CheckHit (Vector3 heroPos, int hitId = 0);
	int GetHP ();
	void Hit ();	// Reduce life
	bool IsAlive ();

	void PlayIdle ();
	void PlayHit ();
	void PlayDefeat ();
	void DestroyObject ();

	int GetHitScore ();
	int GetKillScore ();

	bool IsBomb ();

}
