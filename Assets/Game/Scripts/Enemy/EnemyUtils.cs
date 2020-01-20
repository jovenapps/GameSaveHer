using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUtils 
{
	// Check that data content is valid
	public static bool IsDataValid (EnemySpawnData data)
	{
		bool isValid = false;
		//string log = "null? " + (data == null);
		if (data != null)
		{ 
			isValid = (data.endSpawnTime > data.startSpawnTime) && 
					data.startCount > 0 && 
			        data.spawnIntervals > 0f;

			//log += ", start " + data.startSpawnTime + 
   //                 ", end " + data.endSpawnTime + 
   //                 ", startcount " + data.startCount + 
   //                 ", interval " + data.spawnIntervals;
			//Debug.Log("EnemySpawnData invalid: " + log + ", isValid " + isValid);
		}
		return isValid;
	}

	// Check that the spawn data is still usable
	public static bool CheckDataActive (EnemySpawnData data, float time)
	{
		if(!IsDataValid(data))
			return false;

		return data.startSpawnTime <= time && time <= data.endSpawnTime;
	}

	// Check spawn data if new enemies must be spawned
	public static bool CheckSpawnNeeded (EnemySpawnData data, float currentTime, float controllerTime, int spawnCount)
	{
		if (!CheckDataActive(data, currentTime))
		{
			return false;
		}

		if(spawnCount == 0)
		{
			// on first spawn, check spawn timer
			return controllerTime >= data.startSpawnTime;
		}
		else 
		{
			// after the first spawn, rely on checking timer vs spawn intervals
			return (controllerTime >= data.spawnIntervals);
		}
	}

	public static void GetHits (Vector3 heroPos, List<IEnemy> enemies, ref List<IEnemy> hits)
	{
		hits.Clear();

		for(int i=0; i < enemies.Count; i++)
		{
			if(enemies[i].CheckHit(heroPos))
			{
				hits.Add(enemies[i]);
			}
		}
	}

	public static bool CheckBoxHit (Vector3 a, Vector3 b, float range)
	{
		// Simple box check first for 2d checking
		float x = Mathf.Abs(a.x - b.x);
		float y = Mathf.Abs(a.y - b.y);
		return (x <= range || y <= range);
	}

	public static bool CheckDistanceHit (Vector3 a, Vector3 b, float range)
	{
		a.z = b.z;
		float dist = Vector3.Distance(a, b);
		return dist <= range;
	}


}
