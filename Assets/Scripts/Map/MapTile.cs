using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MapTile : MonoBehaviour {

	public BattleMap battlemap;
	public TacticsMove currentCharacter;
	public GameObject highlight;
	public GameObject dangerZone;
	public GameObject spawnPoint;

	[Header("Selectable")]
	public bool current;
	public bool target;
	public bool pathable;
	public bool selectable;
	public bool attackable;
	public bool supportable;
	public bool dangerous;
	public int deployable;

	[Header("Map values")]
	public int posx;
	public int posy;
	public TerrainTile terrain;

	[Header("Distance Search")]
	public int distance = 1000;
	public int value = 0;
	public MapTile parent;

	[Header("Special Tiles")]
	public InteractType interactType;
	public TerrainTile alternativeTerrain;
	public BlockMove blockMove;
	public bool interacted;
	public DialogueEntry dialogue;
	public Reward gift;
	public TacticsMove ally;

	private SpriteRenderer _rendHighlight;


	private void Start () {
		_rendHighlight = highlight.GetComponent<SpriteRenderer>();
	}
	
	private void Update () {
		SetHighlightColor();
	}

	public void PrintPos() {
		Debug.Log("Pos - x: " + posx + " , posy: " + posy);
	}

	public bool IsEmpty() {
		return (currentCharacter == null);
	}

	private void SetHighlightColor() {
		Color tileColor = Color.white;
		tileColor.a = 0.35f;

		if (current) {
			tileColor = Color.magenta;
			tileColor.a = 0.35f;
		}
		else if (target) {
			tileColor = Color.cyan;
			tileColor.a = 0.35f;
		}
		else if (pathable || deployable > 0) {
			tileColor = Color.yellow;
			tileColor.a = 0.35f;
		}
		else if (selectable) {
			tileColor = Color.blue;
			tileColor.a = 0.35f;
		}
		else if (attackable) {
			tileColor = Color.red;
			tileColor.a = 0.35f;
		}
		else if (supportable) {
			tileColor = Color.green;
			tileColor.a = 0.35f;
		}
		else {
			tileColor.a = 0f;
		}

		_rendHighlight.color = tileColor;
		dangerZone.SetActive(dangerous);
		spawnPoint.SetActive(deployable > 0);
	}

	public void SetTerrain(TerrainTile terrainData) {
		terrain = terrainData;
		SpriteRenderer spr = GetComponent<SpriteRenderer>();
		spr.sprite = terrainData.sprite;
		spr.color = terrainData.tint;
	}

	public int GetRoughness(MovementType type) {
		for (int i = 0; i < terrain.canMoveTypes.Length; i++) {
			if (terrain.canMoveTypes[i].type == type) {
//				Debug.Log("Movespeed:  " + terrain.canMoveTypes[i].roughness);
				return terrain.canMoveTypes[i].roughness;
			}
		}
		Debug.LogError("Forgot to add type " + type);
		return 1;
	} 

	public void Reset() {
		current = false;
		target = false;
		pathable = false;
		selectable = false;
		attackable = false;
		supportable = false;

		distance = 1000;
		value = 0;
		parent = null;
	}

	public void FindNeighbours(Queue<MapTile> progress, int currentDistance, SearchInfo info) {
		MapTile tile = battlemap.GetTile(posx-1, posy);
		if (CheckTile(tile, currentDistance, info))
			progress.Enqueue(tile);
		tile = battlemap.GetTile(posx+1, posy);
		if (CheckTile(tile, currentDistance, info))
			progress.Enqueue(tile);
		tile = battlemap.GetTile(posx, posy-1);
		if (CheckTile(tile, currentDistance, info))
			progress.Enqueue(tile);
		tile = battlemap.GetTile(posx, posy+1);
		if (CheckTile(tile, currentDistance, info))
			progress.Enqueue(tile);
	}

	public bool CheckTile(MapTile checkTile, int currentDistance, SearchInfo info) {
		if (checkTile == null)
			return false;
		if (checkTile.currentCharacter != null && checkTile.currentCharacter.faction != info.tactics.faction) {
			return false;
		}

		MovementType moveType = info.tactics.stats.currentClass.classType;
		if (checkTile.GetRoughness(moveType) == -1)
			return false;
		currentDistance += checkTile.GetRoughness(moveType);
		if (currentDistance >= checkTile.distance)
			return false;

		checkTile.distance = currentDistance;
		if (currentDistance > info.moveSpeed)
			return false;
	
		if (info.isDanger) {
			checkTile.dangerous = true;
		}
		else if (checkTile.currentCharacter == null) {
			checkTile.selectable = (checkTile.currentCharacter == null);
		}
			
		if (info.wpnRange != null && info.showAttack) {
			battlemap.ShowAttackTiles(checkTile, info.wpnRange, info.tactics.faction, info.isDanger);
		}
			
		if (info.staff != null && info.showAttack) {
			battlemap.ShowSupportTiles(checkTile, info.staff, info.tactics.faction, info.isDanger, info.isBuff);
		}
		
		checkTile.parent = this;
		return true;
	}

}
