using System;

[Serializable]
public class EnemyLevelData 
{
	public int level;

	// Enemies spawn scheduling
	public EnemySpawnData[] spawnData;
}

[Serializable]
public class EnemySpawnData
{
	public EnemyData.Type type;
	public int enemyId;
	public int startCount;

	public float startSpawnTime;
	public float endSpawnTime;

	public float spawnIntervals;
	public int maxEnemies;
}


