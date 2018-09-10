using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LibraryEntries/MapEntry")]
public class MapEntry : ScrObjLibraryEntry {

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

	[Header("Players")]
	public List<PlayerPosition> spawnPoints = new List<PlayerPosition>();
	
	[Header("Enemies")]
	public List<EnemyPosition> enemies = new List<EnemyPosition>();
	
	[Header("Interactions")]
	public List<InteractPosition> interactions = new List<InteractPosition>();


	public override void ResetValues() {
		base.ResetValues();
		preDialogue = null;
		postDialogue = null;

		owMusic = null;
		battleMusic = null;
		healMusic = null;

		sizeX = 0;
		sizeY = 0;
		mapSprite = null;

		spawnPoints = new List<PlayerPosition>();
		enemies = new List<EnemyPosition>();
		interactions = new List<InteractPosition>();
	}

	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		MapEntry map = (MapEntry)other;

		preDialogue = map.preDialogue;
		postDialogue = map.postDialogue;

		owMusic = map.owMusic;
		battleMusic = map.battleMusic;
		healMusic = map.healMusic;

		sizeX = map.sizeX;
		sizeY = map.sizeY;
		mapSprite = map.mapSprite;

		spawnPoints = new List<PlayerPosition>();
		for (int i = 0; i < map.spawnPoints.Count; i++){
			spawnPoints.Add(map.spawnPoints[i]);
		}
		enemies = new List<EnemyPosition>();
		for (int i = 0; i < map.enemies.Count; i++){
			enemies.Add(map.enemies[i]);
		}
		interactions = new List<InteractPosition>();
		for (int i = 0; i < map.interactions.Count; i++){
			interactions.Add(map.interactions[i]);
		}
	}
}


[System.Serializable]
public class PlayerPosition {
	public int x;
	public int y;
	public int level;
	public CharData stats;
	public List<WeaponTuple> inventory = new List<WeaponTuple>();
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
	public List<WeaponTuple> inventory = new List<WeaponTuple>();
	public CharacterSkill[] skills;
	public AggroType aggroType;
}

public enum InteractType { NONE, BLOCK, DIALOGUE, VILLAGE, CHEST }

[System.Serializable]
public class InteractPosition {
	public int x;
	public int y;
	public InteractType interactType;
	public int health;
	public DialogueEntry dialogue;
	public WeaponItem gift;
}
