using System;
using UnityEngine;

[Serializable]
public class LevelController
{
	public int level;

	private SpawnController[] spawners;
	private float minTime = -1f;
	private float maxTime = -1f;

	public LevelController (EnemyLevelData data)
	{
		level = data.level;
		spawners = new SpawnController[data.spawnData.Length];

		int created = 0;
		for (int i=0; i < data.spawnData.Length; i++)
		{
			EnemySpawnData spawnData = data.spawnData[i];
			spawners[i] = SpawnController.CreateController(spawnData, i);

			if(spawners[i] != null)
			{
				created++;

				// Setting extremes of spawn times as the level time range.
				if(minTime < 0f)
				{
					minTime = spawnData.startSpawnTime;
				}
				else
				{
					if(minTime > spawnData.startSpawnTime)
					{
						minTime = spawnData.startSpawnTime;
					}
				}

				if(maxTime < 0f)
				{
					maxTime = spawnData.endSpawnTime;
				}
				else
				{
					if(maxTime < spawnData.endSpawnTime)
					{
						maxTime = spawnData.endSpawnTime;
					}
				}
			}
		}
		//Debug.Log("spawners.Length : " + spawners.Length + ", created: " + created);
	}

	public void Update (EnemyHandler enemyHandler, float gameTime, float delta)
	{
		if (!IsActive(gameTime) || spawners == null)
		{
			return;
		}

		for(int i=0; i < spawners.Length; i++)
		{
			if(spawners[i] != null)
				spawners[i].Update(enemyHandler, gameTime, delta);
		}
	}

	public bool IsActive (float gameTime)
	{
		return minTime <= gameTime && gameTime <= maxTime;
	}

	public void End ()
	{
		spawners = null;
	}

}
