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
	public BoolVariable lockControls;
	public MapCreator mapCreator;
	public MapTileVariable targetTile;
	public CharacterListVariable playerList;
	public CharacterListVariable enemyList;
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
	private Vector3 _velocity;
	private Vector3 _heading;

	[Header("Stats")]
	public Faction faction;
	public StatsContainer stats;
	public InventoryContainer inventory;
	public SkillsContainer skills;
	public int currentHealth;

	[Header("Dialogue")]
	public List<FightQuote> fightQuotes = new List<FightQuote>();

	[Header("Health")]
	public Image healthImage;
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
		stats.boosts.Clear();
		ActivateSkills(Activation.PASSIVE, null);
		stats.CalculateStats();
		gameObject.name = stats.charData.entryName;
		currentHealth = stats.hp;
		GetComponent<SpriteRenderer>().sprite = stats.charData.battleSprite;
		currentTile = mapCreator.GetTile(posx, posy);
		currentTile.currentCharacter = this;
		transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, 0);
		gameObject.SetActive(true);
		SetupLists();
	}

	/// <summary>
	/// Additional functions which should be run by children.
	/// </summary>
	protected abstract void SetupLists();

	/// <summary>
	/// Additional functions which should be run when movement ends.
	/// </summary>
	protected abstract void EndMovement();


	/// <summary>
	/// Update loop.
	/// Updates the healthbar if they are visible.
	/// </summary>
	private void Update() {
		if (healthImage != null)
			healthImage.fillAmount = GetHealthPercent();
	}

	/// <summary>
	/// Runs the physics loop if the character isMoving and follows the path until the end.
	/// </summary>
	private void FixedUpdate() {
		if (!isMoving)
			return;

		if (Vector3.Distance(transform.position, _heading) >= 0.05f) {
			_velocity = Vector3.MoveTowards(transform.position, _heading, movementVelocity.value * Time.deltaTime);
			_velocity = new Vector3(_velocity.x,_velocity.y,0);
			transform.position = _velocity;
		}
		else if (path.Count > 0) {
			MapTile tile = path.Pop();
			_heading = new Vector3(tile.transform.position.x,tile.transform.position.y,0);
			posx = currentTile.posx;
			posy = currentTile.posy;
			cameraFollowEvent.Invoke();
		}
		else {
			currentTile.currentCharacter = null;
			currentTile = mapCreator.GetTile(Mathf.RoundToInt(transform.localPosition.x),Mathf.RoundToInt(transform.localPosition.y));
			currentTile.currentCharacter = this;
			posx = currentTile.posx;
			posy = currentTile.posy;
			cameraFollowEvent.Invoke();
			EndMovement();
		}
	}

	/// <summary>
	/// Shows all the possible moves for the character.
	/// </summary>
	/// <param name="isDanger"></param>
	public void FindAllMoveTiles(bool isDanger) {
		mapCreator.ResetMap();
		Queue<MapTile> process = new Queue<MapTile>();
		process.Enqueue(currentTile);
		currentTile.distance = 0;
		currentTile.parent = null;
		currentTile.selectable = true;
		currentTile.target = true;
		
		WeaponRange weapon = inventory.GetReach(ItemCategory.WEAPON);
		WeaponRange staff = inventory.GetReach(ItemCategory.STAFF);

		bool isBuff = false;
		if (weapon.max > 0)
			mapCreator.ShowAttackTiles(currentTile, weapon, faction, isDanger);
		if (staff.max > 0) {
			isBuff = (inventory.GetFirstUsableItem(ItemType.BUFF, stats) != null);
			mapCreator.ShowSupportTiles(currentTile, staff, faction, isDanger, isBuff);
		}

		while(process.Count > 0) {
			MapTile tile = process.Dequeue();
			if (tile.distance >= (stats.GetMovespeed()))
				continue;

			tile.FindNeighbours(process, tile.distance, this, stats.GetMovespeed(), weapon, staff, true, isDanger, isBuff);
		}
	}

	/// <summary>
	/// Show the path the character will take when moving to the endTile.
	/// </summary>
	/// <param name="endTile"></param>
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

	/// <summary>
	/// Starts the movement for the character.
	/// Locks the controls until the target is reached.
	/// </summary>
	public void StartMove() {
		Debug.Log("MOVE   " + path.Count);
		lockControls.value = true;
		_heading = transform.position;
		isMoving = true;
	}

	/// <summary>
	/// Undos the movement and returns the character to the starting tile again and updates all references.
	/// </summary>
	/// <param name="startTile"></param>
	public void UndoMove(MapTile startTile) {
		mapCreator.ClearMovement();
		currentTile.currentCharacter = null;
		currentTile = startTile;
		currentTile.currentCharacter = this;
		posx = currentTile.posx;
		posy = currentTile.posy;
		transform.position = new Vector3(startTile.transform.position.x,startTile.transform.position.y,0);
	}
	
	/// <summary>
	/// Takes the current position and makes a list of all enemies which can be reached from 
	/// there using the character's weapons.
	/// </summary>
	/// <returns></returns>
	public List<MapTile> GetAttackablesInRange() {
		List<InventoryTuple> weaponList = inventory.GetAllUsableItemTuple(ItemCategory.WEAPON, stats);
		List<MapTile> enemies = new List<MapTile>();
		// currentTile.current = true;
		for (int i = 0; i < enemyList.values.Count; i++) {
			int tempDist = MapCreator.DistanceTo(this, enemyList.values[i]);
			for (int w = 0; w < weaponList.Count; w++) {
				if (weaponList[w].item == null)
					continue;
				if (weaponList[w].item.InRange(tempDist) && faction != enemyList.values[i].faction) {
					enemies.Add(enemyList.values[i].currentTile);
					break;
				}
			}
		}
		for (int i = 0; i < mapCreator.breakables.Count; i++) {
			int tempDist = MapCreator.DistanceTo(this, mapCreator.breakables[i]);
			for (int w = 0; w < weaponList.Count; w++) {
				if (weaponList[w].item == null)
					continue;
				if (weaponList[w].item.InRange(tempDist)) {
					enemies.Add(mapCreator.breakables[i]);
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
	public List<MapTile> FindSupportablesInRange() {
		WeaponItem staff = inventory.GetFirstUsableItem(ItemCategory.STAFF, stats);
		List<MapTile> supportables = new List<MapTile>();
		for (int i = 0; i < playerList.values.Count; i++) {
			if (this == playerList.values[i] || !playerList.values[i].IsInjured())
				continue;

			int tempDist = MapCreator.DistanceTo(this, playerList.values[i]);
			if (staff.InRange(tempDist) && faction == playerList.values[i].faction)
				supportables.Add(playerList.values[i].currentTile);
		}
		return supportables;
	}

	/// <summary>
	/// Finds all the allies around the character which can be supported with the staff.
	/// </summary>
	/// <returns></returns>
	public List<MapTile> FindAdjacentCharacters(Faction faction) {
		List<MapTile> supportables = new List<MapTile>();
		if (faction == Faction.NONE || faction == Faction.PLAYER) {
			for (int i = 0; i < playerList.values.Count; i++) {
				if (this == playerList.values[i] || !playerList.values[i].IsAlive())
					continue;

				if (MapCreator.DistanceTo(this, playerList.values[i]) == 1)
					supportables.Add(playerList.values[i].currentTile);
			}
		}
		else if (faction == Faction.NONE || faction == Faction.ENEMY) {
			for (int i = 0; i < enemyList.values.Count; i++) {
				if (!enemyList.values[i].IsAlive())
					continue;

				if (MapCreator.DistanceTo(this, enemyList.values[i]) == 1)
					supportables.Add(enemyList.values[i].currentTile);
			}
		}
		return supportables;
	}

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
		BattleContainer.instance.GenerateActions(this, target);
		BattleContainer.instance.PlayBattleAnimations();
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
		BattleContainer.instance.GenerateHealAction(this, ally);
		BattleContainer.instance.PlayBattleAnimations();
	}

	/// <summary>
	/// Ends the turn for the character and darkens it.null Also cleans up the map.
	/// </summary>
	public void End() {
		hasMoved = true;
		GetComponent<SpriteRenderer>().color = new Color(0.66f,0.66f,0.66f);
		mapCreator.ResetMap();
		currentTile.current = true;
		waitEvent.Invoke();
	}

	/// <summary>
	/// Reduces the current health by the damage taken and displays the damage in a popup.
	/// </summary>
	/// <param name="damage"></param>
	public void TakeDamage(int damage, bool isCrit) {
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
		damageNumber.color = new Color(0,0.5f,0);
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
	public void ReceiveBuff(Boost boost, bool isBuff, bool useAnim) {
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
	/// Returns how many percent health are left for UI purposes.
	/// </summary>
	/// <returns></returns>
	public float GetHealthPercent() {
		return Mathf.Clamp01(currentHealth / (float)stats.hp);
	}

	/// <summary>
	/// Returns the tile the character is standing on.
	/// </summary>
	/// <returns></returns>
	public TerrainTile GetTerrain() {
		return currentTile.terrain;
	}

	/// <summary>
	/// Checks if the character is injured and can be healed.
	/// </summary>
	/// <returns></returns>
	public bool IsInjured() {
		return currentHealth != stats.hp;
	}

	/// <summary>
	/// Checks if the character is still alive.
	/// </summary>
	/// <returns></returns>
	public bool IsAlive() {
		return currentHealth > 0;
	}

	/// <summary>
	/// Shows the death animation of the character when they die.
	/// </summary>
	/// <returns></returns>
	protected virtual IEnumerator OnDeath() {
		GetComponent<SpriteRenderer>().color = new Color(0.4f,0.4f,0.4f);
		currentTile.currentCharacter = null;
		yield return new WaitForSeconds(0.4f);
		sfxQueue.Enqueue(deathSfx);
		playSfxEvent.Invoke();
		yield return new WaitForSeconds(1f);
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Refreshes the character and removes expired buffs.
	/// </summary>
	public void OnStartTurn() {
		if (!IsAlive())
			return;
			
		stats.ClearBoosts(true);
		ForEachSkills(Activation.STARTTURN, playerList);
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
	}

	/// <summary>
	/// Ends the turn for the character and restores the color to normal during the enemies' turn.
	/// </summary>
	public void OnEndTurn() {
		hasMoved = false;
		canUndoMove = true;
		stats.ClearBoosts(false);
		GetComponent<SpriteRenderer>().color = Color.white;
	}

	/// <summary>
	/// Takes the range of the character's weapons and checks if any enemy is in range.
	/// </summary>
	/// <returns></returns>
	public bool CanAttack() {
		WeaponRange range = inventory.GetReach(ItemCategory.WEAPON);
		if (range.max == 0)
			return false;
		for (int i = 0; i < enemyList.values.Count; i++) {
			if (!enemyList.values[i].IsAlive())
				continue;
			int distance = MapCreator.DistanceTo(this, enemyList.values[i]);
			if (range.InRange(distance)) {
				return true;
			}
		}
		for (int i = 0; i < mapCreator.breakables.Count; i++) {
			if (!mapCreator.breakables[i].blockMove.IsAlive())
				continue;
			int distance = MapCreator.DistanceTo(this, mapCreator.breakables[i]);
			if (range.InRange(distance)) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Takes the range of the character's staffs and checks if any supportable is in range.
	/// </summary>
	/// <returns></returns>
	public bool CanSupport() {
		WeaponRange range = inventory.GetReach(ItemCategory.STAFF);
		if (range.max == 0)
			return false;
		
		List<InventoryTuple> staffs = inventory.GetAllUsableItemTuple(ItemCategory.STAFF, stats);
		bool isBuff = false;
		for (int i = 0; i < staffs.Count; i++) {
			if (staffs[i].item.itemType == ItemType.BUFF){
				isBuff = true;
				break;
			}
		}

		for (int i = 0; i < playerList.values.Count; i++) {
			bool usable = (playerList.values[i].IsAlive() && playerList.values[i].IsInjured() || isBuff);
			if (!usable || playerList.values[i] == this)
				continue;
			int distance = MapCreator.DistanceTo(this, playerList.values[i]);
			if (range.InRange(distance)) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Checks if the character can visit a village or other buildings.
	/// </summary>
	/// <returns></returns>
	public bool CanVisit() {
		return (currentTile.interactType == InteractType.VILLAGE);
	}

	/// <summary>
	/// Checks if the character can visit a village or other buildings.
	/// </summary>
	/// <returns></returns>
	public bool CanSeize() {
		return (currentTile.interactType == InteractType.SEIZE);
	}

	/// <summary>
	/// Checks if the character can trade with anyone.
	/// </summary>
	/// <returns></returns>
	public bool CanTrade() {
		if (!canUndoMove)
			return false;
		List<MapTile> traders = FindAdjacentCharacters(Faction.PLAYER);
		return (traders.Count > 0);
	}

	//Inventory

	/// <summary>
	/// Returns the first equippable weapon of the given item type.
	/// </summary>
	/// <param name="category"></param>
	/// <returns></returns>
	public WeaponItem GetEquippedWeapon(ItemCategory category) {
		return inventory.GetFirstUsableItem(category, stats);
	}
	
	/// <summary>
	/// Returns the first usable item tuple from the inventory of the given category.
	/// </summary>
	/// <param name="category"></param>
	/// <returns></returns>
	public InventoryTuple GetFirstUsableInventoryTuple(ItemCategory category) {
		return inventory.GetFirstUsableItemTuple(category, stats);
	}
	
	/// <summary>
	/// Reduces the weapon charge of the first weapon with the given category
	/// </summary>
	/// <param name="category"></param>
	public void ReduceWeaponCharge(ItemCategory category) {
		inventory.ReduceItemCharge(category);
	}

	//SKills

	public void ActivateSkills(Activation activation, TacticsMove enemy) {
		skills.ActivateSkills(activation, this, enemy);
	}

	public void EndSkills(Activation activation, TacticsMove enemy) {
		skills.EndSkills(activation, this, enemy);
	}

	public int EditValueSkills(Activation activation, int value) {
		return skills.EditValueSkills(activation, this, value);
	}

	public void ForEachSkills(Activation activation, CharacterListVariable list) {
		skills.ForEachSkills(activation, this, list);
	}
}
