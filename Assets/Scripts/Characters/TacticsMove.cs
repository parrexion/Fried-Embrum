using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public abstract class TacticsMove : MonoBehaviour {

	public BoolVariable lockControls;
	public MapCreator mapCreator;
	public ActionModeVariable currentMode;
	public MapTileVariable attackTarget;
	public CharacterListVariable playerList;
	public CharacterListVariable enemyList;

	[Header("Movement")]
	public IntVariable movementVelocity;
	public bool isMoving;
	public bool hasMoved;
	public int posx, posy;
	public Stack<MapTile> path = new Stack<MapTile>();
	protected MapTile currentTile;
	private Vector3 _velocity;
	private Vector3 _heading;

	[Header("Stats")]
	public Faction faction = Faction.PLAYER;
	public StatsContainer stats;
	public int currentHealth;

	[Header("Materials")]
	public Material deadMaterial;

	[Header("Health")]
	public Image healthImage;
	public GameObject damageObject;
	public Text damageNumber;
	public GameObject boostObject;
	public GameObject debuffObject;

	public UnityEvent characterClicked;
	public UnityEvent waitEvent;


	public void Setup() {
		if (stats == null || stats.level == -1) {
			currentHealth = 0;
			gameObject.SetActive(false);
			Debug.LogWarning("No stats");
			return;
		}
		stats.boosts.Clear();
		ActivateSkills(Activation.PASSIVE, null);
		stats.CalculateStats();
		gameObject.name = stats.charData.charName;
		currentHealth = stats.hp;
		GetComponent<SpriteRenderer>().sprite = stats.charData.battleSprite;
		currentTile = mapCreator.GetTile(posx, posy);
		currentTile.currentCharacter = this;
		transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, 0);
		SetupLists();
	}

	protected abstract void SetupLists();

	protected abstract void EndMovement();

	public virtual void CalculateMovement() { }

	public virtual void CalculateAttacks() { }

	private void Update() {
		if (healthImage != null)
			UpdateHealthbar();
	}

	private void UpdateHealthbar() {
		healthImage.fillAmount = GetHealthPercent();
	}

	public void FindAllMoveTiles(bool isDanger) {
		mapCreator.ResetMap();
		Queue<MapTile> process = new Queue<MapTile>();
		process.Enqueue(currentTile);
		currentTile.distance = 0;
		currentTile.parent = null;
		
		if (isDanger)
			currentTile.reachable = true;
		else
			currentTile.current = true;
		
		if (GetWeapon(ItemCategory.WEAPON) != null)
			mapCreator.ShowAttackTiles(currentTile, GetWeapon(ItemCategory.WEAPON), faction, isDanger);
		if (GetWeapon(ItemCategory.STAFF) != null)
			mapCreator.ShowSupportTiles(currentTile, GetWeapon(ItemCategory.STAFF), faction, isDanger);

		while(process.Count > 0) {
			MapTile tile = process.Dequeue();
			if (tile.distance >= (stats.GetMovespeed()))
				continue;

			tile.FindNeighbours(process, tile.distance, this, stats.GetMovespeed(), true, isDanger);
		}
	}

	public void ShowMove(MapTile endTile) {
		mapCreator.ClearMovement();
		endTile.target = true;
		path.Clear();
		MapTile cTile = endTile;
		while(cTile.parent != null) {
			path.Push(cTile);
			cTile.pathable = true;
			cTile = cTile.parent;
		}
	}

	public void Move() {
		Debug.Log("MOVE   " + path.Count);
		_heading = transform.position;
		isMoving = true;
	}

	public MapTile CalculateCorrectMoveTile(MapTile current, MapTile attackTile) {
		if (current == null)
			current = currentTile;
		int tempDist = MapCreator.DistanceTo(attackTile, current);
		if ((GetWeapon(ItemCategory.WEAPON) != null && GetWeapon(ItemCategory.WEAPON).InRange(tempDist)) ||
		    (GetWeapon(ItemCategory.STAFF) != null && GetWeapon(ItemCategory.STAFF).InRange(tempDist))) {
			return current;
		}

		for (int i = 0; i < mapCreator.tiles.Length; i++) {
			MapTile tempTile = mapCreator.tiles[i];
			if (tempTile.currentCharacter != null || !tempTile.selectable)
				continue;
			tempDist = MapCreator.DistanceTo(attackTile, tempTile);
			if ((GetWeapon(ItemCategory.WEAPON) != null && GetWeapon(ItemCategory.WEAPON).InRange(tempDist)) ||
				(GetWeapon(ItemCategory.STAFF) != null && GetWeapon(ItemCategory.STAFF).InRange(tempDist))) {
				return tempTile;
			}
		}

		Debug.Log("Something went wrong it seems :/");
		return null;
	}
	
//
//	public void FindAllAttackTiles() {
//		currentTile.current = true;
//		for (int i = 0; i < ConstValues.MAP_SIZE_X; i++) {
//			for (int j = 0; j < ConstValues.MAP_SIZE_Y; j++) {
//				MapTile tempTile = mapCreator.GetTile(i,j);
//				int tempDist = MapCreator.DistanceTo(this, tempTile);
//				if (!GetWeapon().InRange(tempDist)) 
//					continue;
//				if (!SameFaction(this, tempTile.currentCharacter))
//					tempTile.attackable = true;
//			}
//		}
//	}
//
//	public void FindAllHealTiles() {
//		currentTile.current = true;
//		for (int i = 0; i < ConstValues.MAP_SIZE_X; i++) {
//			for (int j = 0; j < ConstValues.MAP_SIZE_Y; j++) {
//				MapTile tempTile = mapCreator.GetTile(i,j);
//				if (tempTile.currentCharacter == null)
//					continue;
//				int tempDist = MapCreator.DistanceTo(this, tempTile);
//				if (!GetSupport().InRange(tempDist)) 
//					continue;
//				bool usable = tempTile.currentCharacter.currentHealth != tempTile.currentCharacter.stats.hp;
//				if (SameFaction(this, tempTile.currentCharacter) && (usable || GetSupport().supportType == SupportType.BUFF))
//					tempTile.pathable = true;
//			}
//		}
//	}

	protected void Attack(TacticsMove enemy) {
		lockControls.value = true;
		characterClicked.Invoke();
		BattleContainer.instance.GenerateActions(this, enemy);
		BattleContainer.instance.PlayBattleAnimations();
	}

	protected void Heal(TacticsMove ally) {
		lockControls.value = true;
		characterClicked.Invoke();
		BattleContainer.instance.GenerateHealAction(this, ally);
		BattleContainer.instance.PlayBattleAnimations();
	}

	public void End() {
		hasMoved = true;
		GetComponent<SpriteRenderer>().color = new Color(0.66f,0.66f,0.66f);
		mapCreator.ResetMap();
		currentTile.current = true;
		waitEvent.Invoke();
	}

	private void FixedUpdate() {
		if (!isMoving)
			return;

		if (Vector3.Distance(transform.position, _heading) >= 0.05f) {
			CalculateVelocity();
			transform.position = _velocity;
		}
		else if (path.Count > 0) {
			MapTile tile = path.Pop();
			_heading = new Vector3(tile.transform.position.x,tile.transform.position.y,0);
			posx = currentTile.posx;
			posy = currentTile.posy;
		}
		else {
			currentTile.currentCharacter = null;
			currentTile = mapCreator.GetTile(Mathf.RoundToInt(transform.localPosition.x),Mathf.RoundToInt(transform.localPosition.y));
			currentTile.currentCharacter = this;
			posx = currentTile.posx;
			posy = currentTile.posy;
			EndMovement();
		}
	}

	private void CalculateVelocity() {
		_velocity = Vector3.MoveTowards(transform.position, _heading, movementVelocity.value * Time.deltaTime);
		_velocity = new Vector3(_velocity.x,_velocity.y,0);
	}

	public void TakeDamage(int damage) {
		if (damage > 0)
			currentHealth -= damage;
		damageNumber.text = (damage == -1) ? "Miss" : damage.ToString();
		damageNumber.color = Color.black;
		StartCoroutine(DamageDisplay());
		if (currentHealth <= 0)
			StartCoroutine(OnDeath());
	}

	public void TakeHeals(int health) {
		currentHealth = Mathf.Min(stats.hp, currentHealth + health);
		damageNumber.text = health.ToString();
		damageNumber.color = new Color(0,0.5f,0);
		StartCoroutine(DamageDisplay());
	}

	public void ReceiveBuff(Boost boost, bool isBuff, bool useAnim) {
		if (!IsAlive())
			return;
		
		boost.ActivateBoost();
		stats.boosts.Add(boost);
		stats.CalculateStats();
		if (useAnim)
			StartCoroutine(BoostDisplay(isBuff));
	}

	protected IEnumerator DamageDisplay() {
		damageObject.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.65f);
		damageObject.gameObject.SetActive(false);
	}

	protected IEnumerator BoostDisplay(bool isBuff) {
		if (isBuff) {
			boostObject.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.65f);
			boostObject.gameObject.SetActive(false);
		}
		else {
			debuffObject.gameObject.SetActive(true);
			yield return new WaitForSeconds(0.65f);
			debuffObject.gameObject.SetActive(false);
		}
	}

	public float GetHealthPercent() {
		return Mathf.Clamp01(currentHealth / (float)stats.hp);
	}

	public bool IsInjured() {
		return currentHealth != stats.hp;
	}

	public bool IsAlive() {
		return currentHealth > 0;
	}

	protected IEnumerator OnDeath() {
		GetComponent<SpriteRenderer>().color = new Color(0.4f,0.4f,0.4f);
		currentTile.currentCharacter = null;
		yield return new WaitForSeconds(1f);
		gameObject.SetActive(false);
	}

	public void OnStartTurn() {
		stats.ClearBoosts(true);
		ForEachSkills(Activation.STARTTURN, playerList);
		hasMoved = false;
	}

	public void OnEndTurn() {
		hasMoved = false;
		stats.ClearBoosts(false);
		GetComponent<SpriteRenderer>().color = Color.white;
	}

	public bool CanAttack() {
		WeaponItem invItem = GetWeapon(ItemCategory.WEAPON);
		if (invItem == null)
			return false;
		for (int i = 0; i < enemyList.values.Count; i++) {
			int distance = MapCreator.DistanceTo(this, enemyList.values[i]);
			if (invItem.InRange(distance)) {
				return true;
			}
		}
		return false;
	}

	public bool CanSupport() {
		WeaponItem invItem = GetWeapon(ItemCategory.STAFF);
		if (invItem == null)
			return false;
		for (int i = 0; i < playerList.values.Count; i++) {
			bool usable = (playerList.values[i].IsInjured() || invItem.itemType == ItemType.BUFF);
			if (playerList.values[i] == this || !usable)
				continue;
			int distance = MapCreator.DistanceTo(this, playerList.values[i]);
			if (invItem.InRange(distance)) {
				return true;
			}
		}
		return false;
	}
	
	public void ShowAttackTiles(bool range1, bool range2) {
		if (!IsAlive())
			return;
			
		if (range1) {
			MapTile tile = mapCreator.GetTile(posx+1,posy);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx,posy+1);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx-1,posy);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx,posy-1);
			if (tile != null) tile.attackable = true;
		}
		if (range2) {
			MapTile tile = mapCreator.GetTile(posx+2,posy);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx,posy+2);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx-2,posy);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx,posy-2);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx+1,posy+1);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx-1,posy+1);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx-1,posy-1);
			if (tile != null) tile.attackable = true;
			tile = mapCreator.GetTile(posx+1,posy-1);
			if (tile != null) tile.attackable = true;
		}
	}
	
	public void ShowSupportTiles(bool range1, bool range2) {
		if (!IsAlive())
			return;
			
		if (range1) {
			MapTile tile = mapCreator.GetTile(posx+1,posy);
			if (tile != null) tile.supportable = true;
			tile = mapCreator.GetTile(posx,posy+1);
			if (tile != null) tile.supportable = true;
			tile = mapCreator.GetTile(posx-1,posy);
			if (tile != null) tile.supportable = true;
			tile = mapCreator.GetTile(posx,posy-1);
			if (tile != null) tile.supportable = true;
		}
		if (range2) {
			MapTile tile = mapCreator.GetTile(posx+2,posy);
			if (tile != null) tile.supportable = true;
			tile = mapCreator.GetTile(posx,posy+2);
			if (tile != null) tile.supportable = true;
			tile = mapCreator.GetTile(posx-2,posy);
			if (tile != null) tile.supportable = true;
			tile = mapCreator.GetTile(posx,posy-2);
			if (tile != null) tile.supportable = true;
			tile = mapCreator.GetTile(posx+1,posy+1);
			if (tile != null) tile.supportable = true;
			tile = mapCreator.GetTile(posx-1,posy+1);
			if (tile != null) tile.supportable = true;
			tile = mapCreator.GetTile(posx-1,posy-1);
			if (tile != null) tile.supportable = true;
			tile = mapCreator.GetTile(posx+1,posy-1);
			if (tile != null) tile.supportable = true;
		}
	}

	public WeaponItem GetWeapon(ItemCategory category) {
		return stats.GetItem(category);
	}
	
	public InventoryTuple GetInventoryTuple(ItemCategory category) {
		return stats.GetItemTuple(category);
	}
	
	public void ReduceWeaponCharge(ItemCategory category) {
		stats.ReduceItemCharge(category);
	}

	public void ActivateSkills(Activation activation, TacticsMove enemy) {
		stats.ActivateSkills(activation, this, enemy);
	}

	public void EndSkills(Activation activation, TacticsMove enemy) {
		stats.EndSkills(activation, this, enemy);
	}

	public int EditValueSkills(Activation activation, int value) {
		return stats.EditValueSkills(activation, this, value);
	}

	public void ForEachSkills(Activation activation, CharacterListVariable list) {
		stats.ForEachSkills(activation, this, list);
	}
}
