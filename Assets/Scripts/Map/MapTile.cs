using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour {

	[System.Serializable]
	private class TriggerID {
		public string id;
		public Faction faction;
	}

	public BattleMap battlemap;
	public TacticsMove currentCharacter;
	public GameObject highlight;
	public GameObject dangerZone;
	public GameObject spawnPoint;
	public ActionModeVariable actionMode;

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
	public KeyType doorKeyType;
	private List<TriggerID> triggers = new List<TriggerID>();

	private SpriteRenderer _rendHighlight;
	private float colorWarping;


	private void Start() {
		_rendHighlight = highlight.GetComponent<SpriteRenderer>();
	}

	private void Update() {
		colorWarping += Time.deltaTime;
		if (colorWarping >= 1f) {
			colorWarping -= 1f;
		}
		SetHighlightColor();
	}

	public void PrintPos() {
		Debug.Log("Pos - x: " + posx + " , posy: " + posy);
	}

	public bool IsEmpty(TacticsMove me = null) {
		return (currentCharacter == null || currentCharacter == me);
	}

	private void SetHighlightColor() {
		Color tileColor = Color.white;
		tileColor.a = 0.35f;

		if (current) {
			tileColor = (colorWarping >= 0.5f && actionMode.value == ActionMode.MOVE) ? Color.yellow : Color.magenta;
			tileColor.a = 0.35f;
		}
		else if (target) {
			tileColor = (colorWarping >= 0.5f && actionMode.value != ActionMode.NONE && actionMode.value != ActionMode.MOVE) ? Color.magenta : Color.cyan;
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
		MapTile tile = battlemap.GetTile(posx - 1, posy);
		if (CheckTile(tile, currentDistance, info))
			progress.Enqueue(tile);
		tile = battlemap.GetTile(posx + 1, posy);
		if (CheckTile(tile, currentDistance, info))
			progress.Enqueue(tile);
		tile = battlemap.GetTile(posx, posy - 1);
		if (CheckTile(tile, currentDistance, info))
			progress.Enqueue(tile);
		tile = battlemap.GetTile(posx, posy + 1);
		if (CheckTile(tile, currentDistance, info))
			progress.Enqueue(tile);
	}

	public bool CheckTile(MapTile checkTile, int currentDistance, SearchInfo info) {
		if (checkTile == null)
			return false;
		if (checkTile.currentCharacter != null && checkTile.currentCharacter.faction != info.tactics.faction) {
			//return false;
			currentDistance = 500;
		}

		MovementType moveType = info.tactics.stats.currentClass.classType;
		if (checkTile.GetRoughness(moveType) == -1 || checkTile.GetRoughness(moveType) > info.oneTurnSpeed)
			return false;
		currentDistance += checkTile.GetRoughness(moveType);
		if (currentDistance >= checkTile.distance)
			return false;

		checkTile.distance = currentDistance;
		if (currentDistance > info.maxMoveSpeed)
			return false;

		if (info.isDanger) {
			checkTile.dangerous = true;
		}
		else if (checkTile.currentCharacter == null || checkTile.currentCharacter == info.tactics) {
			checkTile.selectable = true;
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

	/// <summary>
	/// Adds a trigger for the event to this tile.
	/// </summary>
	/// <param name="trigger"></param>
	public void AddTrigger(TriggerArea trigger) {
		triggers.Add(new TriggerID() {
			id = battlemap.triggerList.values[trigger.idIndex].id,
			faction = trigger.faction
		});
	}

	/// <summary>
	/// Can be triggered whenever someone ends their turn on the tile.
	/// </summary>
	public void EndOn(Faction faction) {
		for (int i = 0; i < triggers.Count; i++) {
			if (faction == triggers[i].faction) {
				battlemap.triggerList.Trigger(triggers[i].id);
			}
		}
	}
}


