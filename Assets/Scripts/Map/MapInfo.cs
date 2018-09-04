using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LibraryEntries/MapInfo")]
public class MapInfo : ScriptableObject {

	public string mapName;

	[Header("Dialogues")]
	public DialogueEntry preDialogue;
	public DialogueEntry postDialogue;

	[Header("Music")]
	public MusicEntry owMusic;
	public MusicEntry battleMusic;
	public MusicEntry healMusic;

	[Header("Map")]
	public int sizeX;
	public int sizeY;
	public Texture2D mapSprite;

	[Header("Terrain Tiles")]
	public TerrainTile breakable;
	public TerrainTile bridge;
	public TerrainTile forest;
	public TerrainTile fort;
	public TerrainTile house;
	public TerrainTile ledge;
	public TerrainTile mountain;
	public TerrainTile normal;
	public TerrainTile pillar;
	public TerrainTile river;
	public TerrainTile throne;
	public TerrainTile wall;

	[Header("Players")]
	public PlayerPosition[] spawnPoints;
	
	[Header("Enemies")]
	public EnemyPosition[] enemies;
	
}


[System.Serializable]
public class PlayerPosition {
	public int x;
	public int y;
	public int level;
	public CharData stats;
	public WeaponTuple[] inventory;
	public CharacterSkill[] skills;
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
	public CharData stats;
	public WeaponTuple[] inventory;
	public CharacterSkill[] skills;
	public AggroType aggroType;
}
