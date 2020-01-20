using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Enemy data will store all data needed to describe an enemy's behavior
/// - Carl Joven
/// </summary>
[Serializable]
public class EnemyData 
{
	public enum Type
	{
		ATTACKER,		// Enter the scene and attack
		PASSERBY,		// Simply pass by the scene
		CARRIER,			// Enters the scene and brings out more attackers
		BOMB
	}

	public Type type = Type.PASSERBY;

	public float speed = 2f;        // base movement of the enemyy
	public float attackSpeed = 2f;	// movement when the enemy decides to attack
	public int life = 1;
	public float size = 0.5f;

	public int hitScore = 1;
	public int killScore = 2;


	public SimplePathData [] startingPaths;    // Start the enemy at any of these random position and enter the scenes

	public Vector3 [] pathsAfterEntrance;   // Other possible paths to move to in the scene
	public Vector3 [] exitPositions;		// Some enemies will exit after leaving the scene

}

[Serializable]
public struct SimplePathData
{
	public Vector3 start;
	public Vector3 end;
}

[Serializable]
public struct PathData
{
	public Vector3 [] path;
}
