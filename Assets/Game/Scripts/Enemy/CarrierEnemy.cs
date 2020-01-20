using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierEnemy : MovingEnemy
{
	[SerializeField] private Vector2 spawnTimes = new Vector2(4, 8) ;
	[SerializeField] private float spawnSpacing = 2f;
	[SerializeField] private AnimationClip spawnClip;

	public int attackCounter = 0;

	protected override void OnEntered ()
	{
		SetState(State.IDLING);
		Attack();

		int level = EnemyHandler.GetInstance().GetLevel();
		if(level > 0)
		{
			hp = data.life * level;
		}
	}

	public override void Update ()
	{
		base.Update();

		if(attackTimer > 0f)
		{
			attackTimer -= Time.deltaTime;
			if(attackTimer <= 0f)
				Attack();
		}
	}

	public override void Attack ()
	{
		if(enemyAnim != null)
		{
			enemyAnim.clip = spawnClip;
			enemyAnim.Play();
			Invoke("PlayIdle", 4f);
		}

		attackTimer = Random.Range(spawnTimes.x, spawnTimes.y);

		// Create attackers
		int enemyId = 1;
		int level = EnemyHandler.GetInstance().GetLevel();
		if(level > 4)
			level = 4;

		enemyId = Random.Range(enemyId, level);
		Vector3 basePos = this.transform.position;
		basePos.y -= 1.6f;

		List<Vector3> positions = new List<Vector3>();
		float xSpace = spawnSpacing;
		float yPos = basePos.y - 2f;
		int count = attackCounter + 2;
		if(count > 6)
			count = 6;

		float xStart = basePos.x - (count-1) * 0.5f * xSpace;
		for(int i=0; i < count; i++)
		{
			positions.Add(new Vector3(xStart + xSpace*i, yPos, 0f));
		}

		EnemyHandler.GetInstance().SpawnEnemies(enemyId, EnemyData.Type.ATTACKER, positions, basePos);

		attackCounter++;
	}
}
