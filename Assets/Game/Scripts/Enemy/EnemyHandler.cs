using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tweeners;

using Characters;

public class EnemyHandler : MonoBehaviour
{
	// Singletonn
	private static EnemyHandler instance;
	public static EnemyHandler GetInstance ()
	{
		return instance;
	}

	// Handling of child objects
	private List<IEnemy> objectList = new List<IEnemy> ();
	private List<IEnemy> hitList = new List<IEnemy> ();
	private long counter;

	// Enemy references
	[SerializeField] private MovingEnemy [] enemyReferences;
	[SerializeField] private Transform target;
	[SerializeField] private Hero hero;
	[SerializeField] private Girl girl;

	public float ground = -4.4f;

	// Enemy levels
	[SerializeField] private EnemyLevelData [] levelData;
	private float levelTime = 0f;
	private int level = 0;
	private long score;
	private Results results;

	private EnemyLevelData currentLevel = null;
	private LevelController levelController = null;


	public static bool Exists ()
	{
		return instance != null;
	}

	public static float GROUND
	{
		get 
		{
			if(instance != null)
				return instance.ground;
			else
				return -4.4f;
		}
	}

	#region MONO

	void Start ()
	{
		instance = this;
	}

	void OnDestroy ()
	{
		instance = null;

		levelController = null;
		currentLevel = null;
	}

	void Update ()
	{
	}

	#endregion

	#region ENEMY MANAGEMENT

	public long AddEnemy (IEnemy enemy)
	{
		long id = -1;
		if (instance != null && enemy != null)
		{
			objectList.Add (enemy);
			counter++;
			id = counter;
			enemy.SetId (counter);
		}
		return id;	
	}

	public void RemoveEnemy (IEnemy enemy)
	{
		if(objectList.Contains(enemy))
		{
			objectList.Remove (enemy);
			enemy.Kill ();
		}
	}

	public MovingEnemy GetEnemyReference (int id)
	{
		for(int i=0; i < enemyReferences.Length; i++)
		{
			if(enemyReferences[i] != null && enemyReferences[i].id == id)
			{
				return enemyReferences[i];
			}
		}
		return null;
	}

	public void SpawnEnemies (int enemyID, EnemyData.Type enemyType, List<Vector3> positions, Vector3 basePos)
	{
		MovingEnemy refEnemy = GetEnemyReference(enemyID);
		if(refEnemy != null)
		{
			int count = positions.Count;
			for(int i=0; i < count; i++)
			{
				MovingEnemy newEnemy = (MovingEnemy)refEnemy.Create(transform, target);
				newEnemy.id = AddEnemy(newEnemy);
				newEnemy.gameObject.name = "enemy_" + newEnemy.GetDataId() + "_" + newEnemy.id;
				//newEnemy.SetAttackTimer(Random.Range(1f, 2f));
				newEnemy.SetAttackTimer(3f);
				newEnemy.EnterFromCarrier(basePos, positions[i], 0.5f);
			}
		}
		EffectsHandler.PlayPortal2(basePos);
	}

	public int SpawnEnemies (EnemySpawnData data, int count, int controllerId)
	{
		//Debug.Log("SPAWN ENEMY: id " + data.enemyId + ", count " + count);
		int created = 0;
		if(data == null)
		{
			return created;
		}

		// First count existing enemies
		int existingEnemies = CountEnemiesFromController(controllerId);
		int allowableEnemies = data.maxEnemies - existingEnemies;
		if(allowableEnemies <= 0)
		{
			return created;
		}

		if(count > allowableEnemies)
		{
			count = allowableEnemies;	// Limit the enemies to create 
		}
		
		//EnemyData.Type enemyType = data.type;
		int enemyID = data.enemyId;

		MovingEnemy refEnemy = GetEnemyReference(enemyID);
		if(refEnemy != null)
		{
			for(int i=0; i < count; i++)
			{
				MovingEnemy newEnemy = (MovingEnemy)refEnemy.Create(transform, target);
				newEnemy.id = AddEnemy(newEnemy);
				newEnemy.gameObject.name = "enemy_" + newEnemy.GetDataId() + "_" + newEnemy.id;
				newEnemy.spawnControllerId = controllerId;
				newEnemy.Enter();
			}
		}


		return created;
	}

	public void OnEnemyExit (MovingEnemy target)
	{
		target.gameObject.SetActive(false);
		objectList.Remove(target);

		target.DestroyObject();
		target = null;
	}

	public void CheckHitEnemies ()
	{
		if (!hero.IsAttacking())
			return;

		Vector3 pos = hero.GetStrikePosition();
		Vector3 heroPos = hero.transform.position;
		int hitId = hero.GetHitId();
		hitList.Clear();
		for(int i=0; i < objectList.Count; i++)
		{
			IEnemy enemyObj = objectList[i];
			if(enemyObj != null && enemyObj.IsAlive() && enemyObj.CheckHit(pos, hitId))
			{
				enemyObj.Hit();
				hitList.Add(enemyObj);
			}
		}

		if(hitList.Count == 0)
			return;

		hero.strikeHits += hitList.Count;
		SoundHandler.PlaySlash();

		int hitScores = 0;
		int kills = 0;
		int killScores = 0;
		int killMultiplier = 1;

		int bombs = 0;
		Vector3 bombPos = Vector3.zero;
		for(int i=0; i < hitList.Count; i++)
		{
			IEnemy hitEnemy = hitList[i];
			if(hitEnemy == null)
				continue;

			if(hitEnemy.IsBomb())
			{
				bombs++;
				bombPos = hitEnemy.GetPosition();
			}
			
			if(hitEnemy.IsAlive())
			{
				hitScores += hitEnemy.GetHitScore();
			}
			else
			{
				kills++;
				killScores += hitEnemy.GetKillScore();
				Vector3 enemyPos = hitEnemy.GetPosition();
				Vector3 dir = enemyPos - heroPos;
				EffectsHandler.PlaySlash(enemyPos, dir);

				hitEnemy.Kill();
				RemoveEnemy(hitEnemy); 
			}
		}
		hitList.Clear();

		if(kills >= 10)
			killMultiplier = 5;
		else if(kills >= 5)
			killMultiplier = 3;
		else if(kills >= 3)
			killMultiplier = 2;

		int totalScore = hitScores + killMultiplier * killScores;
		score += totalScore;

		results.score += totalScore;
		results.enemiesKilled += kills;

		UIManager.GetInstance().UpdateScore(score);

		if(bombs > 0)
		{
			EffectsHandler.PlayBomb(bombPos);
			SoundHandler.GetInstance().PlayEnemySFX("bomb1");
			results.bombed = true;
			LostTheGirl();
		}
		else if(hero.strikeHits >= 10)
		{
			EffectsHandler.PlayCombo();
		}

	}

	private void ReportHitsAndKills ()
	{
		
	}

	public int CountEnemiesFromController (int controllerId)
	{
		int count = 0;
		for(int i=0; i < objectList.Count; i++)
		{
			if(objectList[i] != null)
			{
				if(objectList[i].GetControllerId() == controllerId)
					count++;
			}
		}
		return count;
	}
	#endregion

	#region LEVEL MANAGEMENT

	public void StartGame ()
	{
		results = new Results();
		levelTime = 0f;
		level = 0;
		score = 0;

		SoundHandler.GetInstance().PlayBGM();
		girl.StartGame();
		hero.StartGame();
		StartLevel();
	}

	public void StartLevel ()
	{
		currentLevel = GetLevelData(level);

		//Debug.Log("StartLevel, currentLevel : " + (currentLevel != null) + ", " + currentLevel.level);
		if(currentLevel != null)
		{
			levelController = new LevelController(currentLevel);
		}

		//Debug.Log("StartLevel, levelController : " + (levelController != null));
	}

	public int GetLevel ()
	{
		return this.level;
	}

	public EnemyLevelData GetLevelData (int level)
	{
		int levelIdxClosest = 0;
		int levelDiff = 99999;

		for(int i=0; i < levelData.Length; i++)
		{
			if(levelData[i] == null)
				continue;
			
			int diff = Mathf.Abs(levelData[i].level - level);
			if(diff < levelDiff)
			{
				levelDiff = diff;
				levelIdxClosest = i;
			}

			if(levelData[i] != null && levelData[i].level == level)
				return levelData[i];
		}

		if(levelData.Length > 0)
		{
			if(levelIdxClosest < levelData.Length)
				return levelData[levelIdxClosest];
			else
				return levelData[0];
		}
		else
			return null;
	}

	public void UpdateLevel ()
	{
		if(levelController != null)
		{
			levelTime += Time.deltaTime;
			levelController.Update(this, levelTime, Time.deltaTime);

			CheckHitEnemies();

			if(levelController != null &&!levelController.IsActive(levelTime))
			{
				// If enemies are all dead, start new level
				//if(objectList.Count == 0)
				//	StartLevel();
			}
		}
	}

	public Girl GetGirl ()
	{
		return this.girl;
	}

	public void LostTheGirl ()
	{
		hero.HeroFailed();
		levelController.End();
		SoundHandler.GetInstance().StopBGM ();

		Invoke("ShowResults", 1.5f);
	}

	private void ShowResults ()
	{
		GameManager.GetInstance().StartResults();
		UIManager.GetInstance().SetResults(results);

		int objCount = objectList.Count;
		for(int i = objCount-1; i >= 0; i--)
		{
			IEnemy enemyObj = objectList[i];
			if(enemyObj != null)
				enemyObj.DestroyObject();

			enemyObj = null;
		}
		objectList.Clear();
		counter = 0;

		SoundHandler.GetInstance().PlaySFX("level_fail");
	}

	public long GetScore ()
	{
		if(results != null)
			return results.score;
		else
			return 0;
	}

	#endregion

}
