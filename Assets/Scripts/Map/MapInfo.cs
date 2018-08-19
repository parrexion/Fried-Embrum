using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MapInfo : ScriptableObject {

	public int sizeX;
	public int sizeY;
	public Texture2D mapSprite;

	[Header("Terrain Tiles")]
	public TerrainTile normal;
	public TerrainTile forest;
	public TerrainTile river;
	public TerrainTile mountain;
	public TerrainTile bridge;
	public TerrainTile ledge;
	public TerrainTile wall;

	[Header("Spawn Points")]
	public PlayerPosition[] spawnPoints;
	
	[Header("Enemies")]
	public EnemyPosition[] enemies;
	
}


[System.Serializable]
public class PlayerPosition {
	public int x;
	public int y;
}

[System.Serializable]
public class WeaponTuple {
	public WeaponItem item;
	public bool droppable;
}

[System.Serializable]
public class EnemyPosition {
	public int x;
	public int y;
	public int level;
	public CharacterStats stats;
	public WeaponTuple[] inventory;
	public CharacterSkill[] skills;
	public AggroType aggroType;
}
