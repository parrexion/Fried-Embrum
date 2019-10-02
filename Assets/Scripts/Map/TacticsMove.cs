using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


/// <summary>
/// Basic class which contains the general functionality all movable 
/// characters on the map should have.
/// </summary>
public abstract class TacticsMove : MonoBehaviour {

	public static bool infiniteMove = false;

	public BoolVariable lockControls;
	public BattleMap battleMap;
	public MapTileVariable targetTile;
	public CharacterListVariable playerList;
	public CharacterListVariable enemyList;
	public CharacterListVariable allyList;
	public CharacterListVariable interactList;
	public IntVariable battleWeaponIndex;

	[Header("Movement")]
	public IntVariable movementVelocity;
	public bool isMoving;
	public bool canUndoMove;
	public bool hasMoved;
	public int posx, posy;
	public Stack<MapTile> path = new Stack<MapTile>();
	public MapTile currentTile;
	public bool hasEscaped;
	private Vector3 _velocity;
	private Vector3 _heading;
	private MapTile startTile;

	[Header("Stats")]
	public Faction faction;
	public int currentHealth;
	public int fatigueCap = 3;
	public StatsContainer stats;
	public InventoryContainer inventory;
	public SkillsContainer skills;

	[Header("Dialogue")]
	public List<FightQuote> fightQuotes = new List<FightQuote>();
	public List<FightQuote> talkQuotes = new List<FightQuote>();

	[Header("Health")]
	public MyBar healthBar;
	public GameObject damageObject;
	public Image damageNumberBkg;
	public Text damageNumber;
	public ParticleSystem critEffect;
	public GameObject boostObject;
	public GameObject debuffObject;

	[Header("Sound")]
	public AudioQueueVariable sfxQueue;
	public SfxEntry deathSfx;

	[Header("Events")]
	public UnityEvent characterClicked;
	public UnityEvent cameraFollowEvent;
	public UnityEvent waitEvent;
	public UnityEvent playSfxEvent;


	/// <summary>
	/// Sets up the character with default stats and resets values.
	/// </summary>
	public void Setup() {
		if (stats == null || stats.level == -1) {
			currentHealth = 0;
			gameObject.SetActive(false);
			Debug.LogWarning("No stats");
			return;
		}
		currentHealth = stats.hp;
		stats.boosts.Clear();
		ActivateSkills(SkillActivation.PASSIVE, null);
		stats.CalculateStats();
		gameObject.name = stats.charData.entryName;
		currentHealth = stats.hp;
		UpdateHealth();
		inventory.CleanupInventory();
		currentTile = battleMap.GetTile(posx, posy);
		currentTile.currentCharacter = this;
		transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, 0);

		gameObject.SetActive(true);
		ExtraSetup();
	}

	public void CopyData(TacticsMove other) {
		battleMap = other.battleMap;
		posx = other.posx;
		posy = other.posy;
		stats = other.stats;
		inventory = other.inventory;
		skills = other.skills;
	}

	/// <summary>
	/// Additional functions which should be run by children.
	/// </summary>
	protected abstract void ExtraSetup();

	/// <summary>
	/// Additional functions which should be run when movement ends.
	/// </summary>
	public abstract void EndMovement();

	/// <summary>
	/// Removes the character from their corresponding list.
	/// </summary>
	public abstract void RemoveFromList();

	/// <summary>
	/// Runs the physics loop if the character isMoving and follows the path until the end.
	/// </summary>
	private void FixedUpdate() {
		if (!isMoving)
			return;

		if (Vector3.Distance(transform.position, _heading) >= 0.05f) {
			_velocity = Vector3.MoveTowards(transform.position, _heading, movementVelocity.value * Time.deltaTime);
			_velocity = new Vector3(_velocity.x, _velocity.y, 0);
			transform.position = _velocity;
		}
		else if (path.Count > 0) {
			MapTile tile = path.Pop();
			_heading = new Vector3(tile.transform.position.x, tile.transform.position.y, 0);
			posx = tile.posx;
			posy = tile.posy;
			cameraFollowEvent.Invoke();
		}
		else {
			//currentTile.currentCharacter = null;
			currentTile = battleMap.GetTile(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y));
			currentTile.currentCharacter = this;
			posx = currentTile.posx;
			posy = currentTile.posy;
			cameraFollowEvent.Invoke();
			EndMovement();
		}
	}

	/// <summary>
	/// Update loop.
	/// Updates the healthbar if they are visible.
	/// </summary>
	private void UpdateHealth() {
		healthBar?.SetAmount(currentHealth, stats.hp);
	}


	//Functions for movement and the display of it
	#region Movement

	/// <summary>
	/// Shows all the possible moves for the character.
	/// </summary>
	/// <param name="attacking"></param>
	public void FindAllMoveTiles(bool attacking) {
		battleMap.ResetMap();
		Queue<MapTile> process = new Queue<MapTile>();
		process.Enqueue(currentTile);
		currentTile.distance = 0;
		currentTile.parent = null;
		currentTile.selectable = true;
		currentTile.target = true;

		SearchInfo info = new SearchInfo() {
			tactics = this,
			moveSpeed = GetMoveSpeed(),

			wpnRange = inventory.GetReach(ItemCategory.WEAPON),
			staff = inventory.GetReach(ItemCategory.SUPPORT),

			showAttack = true,
			isDanger = attacking,
			//isBuff = false
		};

		if (info.wpnRange.max > 0)
			battleMap.ShowAttackTiles(currentTile, info.wpnRange, faction, info.isDanger);
		if (info.staff.max > 0) {
			info.isBuff = (inventory.GetFirstUsableItemTuple(ItemCategory.SUPPORT) != null);
			battleMap.ShowSupportTiles(currentTile, info.staff, faction, info.isDanger, info.isBuff);
		}

		while (process.Count > 0) {
			MapTile tile = process.Dequeue();
			if (tile.distance >= (GetMoveSpeed()))
				continue;

			tile.FindNeighbours(process, tile.distance, info);
		}
	}

	/// <summary>
	/// Show the path the character will take when moving to the endTile.
	/// </summary>
	/// <param name="endTile"></param>
	public void ShowMove(MapTile endTile) {
		battleMap.ClearMovement();
		endTile.target = true;
		path.Clear();
		MapTile cTile = endTile;
		while (cTile.parent != null) {
			path.Push(cTile);
			cTile.pathable = true;
			cTile = cTile.parent;
		}
	}

	/// <summary>
	/// Starts the movement for the character.
	/// Locks the controls until the target is reached.
	/// </summary>
	public void StartMove() {
		lockControls.value = true;
		_heading = transform.position;
		currentTile.currentCharacter = null;
		startTile = currentTile;
		isMoving = true;
	}

	/// <summary>
	/// Starts the movement for the character and moves it in a straight
	/// line towards the target map tile.
	/// </summary>
	/// <param name="tile"></param>
	public void MoveDirectSwap(MapTile tile) {
		lockControls.value = true;
		path = new Stack<MapTile>();
		path.Push(tile);
		_heading = transform.position;
		isMoving = true;
	}

	/// <summary>
	/// Undos the movement and returns the character to the starting tile again and updates all references.
	/// </summary>
	/// <param name="startTile"></param>
	public void UndoMove() {
		battleMap.ClearMovement();
		currentTile.currentCharacter = null;
		currentTile = startTile;
		currentTile.currentCharacter = this;
		posx = currentTile.posx;
		posy = currentTile.posy;
		transform.position = new Vector3(startTile.transform.position.x, startTile.transform.position.y, 0);
	}

	#endregion

	/// <summary>
	/// Adds attackable to all tiles surrounding the character depending on range.
	/// </summary>
	/// <param name="range1"></param>
	/// <param name="range2"></param>
	public void ShowAttackTiles(WeaponRange range, int damage) {
		if (!IsAlive())
			return;

		for (int i = 0; i < battleMap.tiles.Length; i++) {
			if (range.InRange(BattleMap.DistanceTo(this, battleMap.tiles[i]))) {
				battleMap.tiles[i].attackable = true;
				if (damage >= currentHealth) {
					damage += 100;
				}
				battleMap.tiles[i].value = damage;
			}
		}
	}

	/// <summary>
	/// Adds supportable to all tiles surrounding the character depending on range.
	/// </summary>
	/// <param name="range1"></param>
	/// <param name="range2"></param>
	public void ShowSupportTiles(WeaponRange range, bool isBuff) {
		if (!IsAlive())
			return;

		for (int i = 0; i < battleMap.tiles.Length; i++) {
			if (range.InRange(BattleMap.DistanceTo(this, battleMap.tiles[i]))) {
				battleMap.tiles[i].supportable = true;
				if (isBuff) {
					battleMap.tiles[i].value = 5;
				}
				else {
					battleMap.tiles[i].value = stats.hp - currentHealth;
				}
			}
		}
	}

	/// <summary>
	/// Takes the current position and makes a list of all enemies which can be reached from 
	/// there using the character's weapons.
	/// </summary>
	/// <returns></returns>
	public List<MapTile> GetAttackablesInRange() {
		List<InventoryTuple> weaponList = inventory.GetAllUsableItemTuple(ItemCategory.WEAPON);
		List<MapTile> enemies = new List<MapTile>();
		// currentTile.current = true;
		for (int i = 0; i < enemyList.values.Count; i++) {
			if (!enemyList.values[i].IsAlive())
				continue;
			int tempDist = BattleMap.DistanceTo(this, enemyList.values[i]);
			for (int w = 0; w < weaponList.Count; w++) {
				if (string.IsNullOrEmpty(weaponList[w].uuid))
					continue;
				if (weaponList[w].InRange(tempDist) && faction != enemyList.values[i].faction) {
					enemies.Add(enemyList.values[i].currentTile);
					break;
				}
			}
		}
		for (int i = 0; i < battleMap.breakables.Count; i++) {
			int tempDist = BattleMap.DistanceTo(this, battleMap.breakables[i]);
			for (int w = 0; w < weaponList.Count; w++) {
				if (string.IsNullOrEmpty(weaponList[w].uuid))
					continue;
				if (weaponList[w].InRange(tempDist)) {
					enemies.Add(battleMap.breakables[i]);
					break;
				}
			}
		}
		return enemies;
	}

	/// <summary>
	/// Finds all the allies around the character which can be supported with the staff.
	/// </summary>
	/// <returns></returns>
	public List<MapTile> FindAdjacentCharacters(bool players, bool enemies, bool allies) {
		List<MapTile> supportables = new List<MapTile>();
		if (players) {
			for (int i = 0; i < playerList.values.Count; i++) {
				if (this == playerList.values[i] || !playerList.values[i].IsAvailable())
					continue;

				if (BattleMap.DistanceTo(this, playerList.values[i]) == 1)
					supportables.Add(playerList.values[i].currentTile);
			}
		}
		if (enemies) {
			for (int i = 0; i < enemyList.values.Count; i++) {
				if (this == enemyList.values[i] || !enemyList.values[i].IsAvailable())
					continue;

				if (BattleMap.DistanceTo(this, enemyList.values[i]) == 1)
					supportables.Add(enemyList.values[i].currentTile);
			}
		}
		if (allies) {
			for (int i = 0; i < allyList.values.Count; i++) {
				if (this == allyList.values[i] || !allyList.values[i].IsAvailable())
					continue;

				if (BattleMap.DistanceTo(this, allyList.values[i]) == 1)
					supportables.Add(allyList.values[i].currentTile);
			}
		}
		return supportables;
	}

	//Different functions regarding actions the character can take.
	#region Actions

	/// <summary>
	/// Makes the character attack with the currently selected weapon.
	/// </summary>
	/// <param name="target"></param>
	public void Attack(MapTile target) {
		lockControls.value = true;
		inventory.EquipItem(battleWeaponIndex.value);
		battleWeaponIndex.value = 0;
		characterClicked.Invoke();
		targetTile.value.target = true;
	}

	/// <summary>
	/// Makes the character use the currently selected staff.
	/// </summary>
	/// <param name="ally"></param>
	public void Heal(MapTile ally) {
		lockControls.value = true;
		inventory.EquipItem(battleWeaponIndex.value);
		battleWeaponIndex.value = 0;
		characterClicked.Invoke();
		targetTile.value.target = true;
	}

	/// <summary>
	/// Makes the character escape from the map.
	/// </summary>
	public void Escape() {
		hasEscaped = true;
		currentTile.currentCharacter = null;
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Ends the turn for the character and darkens it. Also cleans up the map.
	/// </summary>
	public void End() {
		hasMoved = true;
		GetComponent<SpriteRenderer>().color = new Color(0.66f, 0.66f, 0.66f);
		battleMap.ResetMap();
		currentTile.current = true;
		currentTile.EndOn(faction);
		waitEvent.Invoke();
	}

	#endregion

	//Functions for taking damage, healing and receiving buffs.
	#region Damage and Status

	/// <summary>
	/// Reduces the current health by the damage taken and displays the damage in a popup.
	/// </summary>
	/// <param name="damage"></param>
	public void TakeDamage(int damage, bool isCrit) {
		if (!IsAlive() || hasEscaped)
			return;

		if (damage > 0) {
			currentHealth -= damage;
			currentHealth = Mathf.Max(0, currentHealth);
		}
		damageNumber.text = (damage == -1) ? "Miss" : damage.ToString();
		if (isCrit) {
			damageNumberBkg.color = Color.black;
			damageNumber.color = Color.white;
			critEffect.Play();
		}
		else {
			damageNumberBkg.color = Color.white;
			damageNumber.color = Color.black;
		}
		UpdateHealth();
		StartCoroutine(DamageDisplay());
		if (currentHealth == 0)
			StartCoroutine(OnDeath());
	}

	/// <summary>
	/// Heals the character the given amount up to the maximum health.
	/// </summary>
	/// <param name="health"></param>
	public void TakeHeals(int health) {
		currentHealth = Mathf.Min(stats.hp, currentHealth + health);
		damageNumber.text = health.ToString();
		damageNumber.color = new Color(0, 0.5f, 0);
		UpdateHealth();
		StartCoroutine(DamageDisplay());
	}

	/// <summary>
	/// Shows the damage animation by showing the damage number for a short moment.
	/// </summary>
	/// <returns></returns>
	protected IEnumerator DamageDisplay() {
		damageObject.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.65f);
		damageObject.gameObject.SetActive(false);
	}

	/// <summary>
	/// Receives the given buff and adds it to the current stats.
	/// isBuff indicates if it's a buff or debuff.
	/// useAnim indicates if the buff gaining should also play an animation.
	/// </summary>
	/// <param name="boost"></param>
	/// <param name="isBuff"></param>
	/// <param name="useAnim"></param>
	public virtual void ReceiveBuff(Boost boost, bool isBuff, bool useAnim) {
		if (!IsAlive())
			return;

		boost.ActivateBoost();
		stats.boosts.Add(boost);
		stats.CalculateStats();
		if (useAnim)
			StartCoroutine(BoostDisplay(isBuff));
	}

	/// <summary>
	/// Shows the buff animation by showing the buff icon for a short moment.
	/// </summary>
	/// <param name="isBuff"></param>
	/// <returns></returns>
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

	/// <summary>
	/// Checks if the character is injured and can be healed.
	/// </summary>
	/// <returns></returns>
	public bool IsInjured() {
		return currentHealth != stats.hp && !hasEscaped;
	}

	/// <summary>
	/// Checks if the character is still alive.
	/// </summary>
	/// <returns></returns>
	public bool IsAlive() {
		return currentHealth > 0;
	}

	/// <summary>
	/// Checks if the character is still on the map.
	/// </summary>
	/// <returns></returns>
	public bool IsAvailable() {
		return IsAlive() && !hasEscaped;
	}

	/// <summary>
	/// Shows the death animation of the character when they die.
	/// </summary>
	/// <returns></returns>
	protected virtual IEnumerator OnDeath() {
		GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
		currentTile.currentCharacter = null;
		yield return new WaitForSeconds(0.4f);
		sfxQueue.Enqueue(deathSfx);
		playSfxEvent.Invoke();
		yield return new WaitForSeconds(1f);
		gameObject.SetActive(false);
	}

	#endregion

	/// <summary>
	/// Returns the current movement speed. Virtual so that other classes
	/// can change the movement speed.
	/// </summary>
	/// <returns></returns>
	protected virtual int GetMoveSpeed() {
		return (infiniteMove) ? 999 : stats.GetMovespeed();
	}

	/// <summary>
	/// Refreshes the character and removes expired buffs.
	/// </summary>
	public bool OnStartTurn() {
		if (!IsAlive())
			return false;

		stats.fatigueAmount = 0;
		stats.ClearBoosts(true);
		ForEachSkills(SkillActivation.STARTTURN);
		hasMoved = false;
		canUndoMove = true;

		// Map tiles
		int diff = (int)(stats.hp * 0.01f * currentTile.terrain.healPercent);
		if (diff > 0 && currentHealth != stats.hp) {
			TakeHeals(diff);
			Debug.Log("Heal");
		}
		else if (diff < 0) {
			TakeDamage(-diff, false);
			Debug.Log("Damage");
		}
		return (diff != 0);
	}

	/// <summary>
	/// Ends the turn for the character and restores the color to normal during the enemies' turn.
	/// </summary>
	public void OnEndTurn() {
		hasMoved = false;
		canUndoMove = true;
		stats.fatigueAmount = 0;
		stats.ClearBoosts(false);
		GetComponent<SpriteRenderer>().color = Color.white;
	}


	//Inventory

	/// <summary>
	/// Returns the first equippable weapon of the given item type.
	/// </summary>
	/// <param name="category"></param>
	/// <returns></returns>
	public InventoryTuple GetEquippedWeapon(ItemCategory category) {
		if (category == ItemCategory.WEAPON) {
			return inventory.GetTuple(0);
		}
		return inventory.GetFirstUsableItemTuple(category);
	}


	//SKills

	public void ActivateSkills(SkillActivation activation, TacticsMove enemy) {
		skills.ActivateSkills(activation, this, enemy);
	}

	public void EndSkills(SkillActivation activation, TacticsMove enemy) {
		skills.EndSkills(activation, this, enemy);
	}

	public int EditValueSkills(SkillActivation activation, int value) {
		return skills.EditValueSkills(activation, this, value);
	}

	public void ForEachSkills(SkillActivation activation) {
		if (faction == Faction.PLAYER) {
			skills.ForEachSkills(activation, this, playerList);
		}
		else if (faction == Faction.ENEMY) {
			skills.ForEachSkills(activation, this, enemyList);
		}
		else if (faction == Faction.ALLY) {
			skills.ForEachSkills(activation, this, allyList);
		}
	}
}
