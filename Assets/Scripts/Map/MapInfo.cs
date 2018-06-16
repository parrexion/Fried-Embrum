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
	public TerrainTile mountain;
	public TerrainTile blocked;

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
public class EnemyPosition {
	public int x;
	public int y;
	public int level;
	public CharacterStats stats;
	public WeaponItem[] inventory;
	public CharacterSkill[] skills;
}
