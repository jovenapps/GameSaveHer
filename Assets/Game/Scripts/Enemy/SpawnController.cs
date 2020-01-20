using System;


[Serializable]
public class SpawnController
{
	// This class handles timing of spawning of enemies based on spawn data
	public EnemySpawnData data;

	// Time for counting intervals for spawning enemies
	public float time;

	// Count enemies created
	public int count;

	public int id;


	// Create a controller if the data is valid
	public static SpawnController CreateController(EnemySpawnData spawnData, int id)
	{
		if (EnemyUtils.IsDataValid(spawnData))
		{
			SpawnController  newController = new SpawnController();
			newController.data = spawnData;
			newController.time = 0f;
			newController.count = 0;
			newController.id = id;
			//UnityEngine.Debug.Log("Creating spawn controller, " + spawnData.enemyId + ", time: " + spawnData.startSpawnTime + " - " + spawnData.endSpawnTime);
			return newController;
		}
		return null;
	}

	public void Update (EnemyHandler handler, float gameTime, float delta)
	{
		time += delta;
		if (EnemyUtils.CheckSpawnNeeded(data, gameTime, time, count))
		{
			// Ask the enemy handler to create new enemies
			time = 0f;
			int createEnemies = 1;
			if(count == 0)
				createEnemies = data.startCount;
			
			count += handler.SpawnEnemies(data, createEnemies, id);
		}
	}
}