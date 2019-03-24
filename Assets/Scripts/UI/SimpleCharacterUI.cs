using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatsPage { BASIC, STATS, INVENTORY }

public class SimpleCharacterUI : MonoBehaviour {
	
	[Header("General")]
	public GameObject menuView;
	public GameObject flipButton;

	[Header("References")]
	public IntVariable currentMenuMode;
	public ActionModeVariable actionMode;
	public TacticsMoveVariable selectedCharacter;
	public MapTileVariable selectedTile;
	public MapTileVariable targetTile;
	public IntVariable inventoryIndex;
	
	[Header("Icons")]
	public Sprite noSkillImage;
	public Sprite[] weaknessImages;
	public Sprite[] weaponSkillImages;

	[Header("Basic Stats")]
	public GameObject basicObject;
	public Image colorBackground;
	public Image portrait;
	public Text characterName;
	public Text currentHpText;
	public Image healthBar;
	public Image weakIcon1;
	public Image weakIcon2;
	public Image wpnIcon;
	public Text wpnName;
	public Image[] skillImages;
	public Text hitText;
	public Text pwrText;
	public Text critText;
	public Text avoidText;
	public Image boostAvoid;

	[Header("Stats Stats")]
	public GameObject statsObject;
	public Text levelText;
	public Text expText;
	public Text hpText;
	public Text atkText;
	public Text spdText;
	public Text sklText;
	public Text lckText;
	public Text defText;
	public Text resText;
	public Text movText;
	public Image weighDownSpdIcon;
	public Text weighDownSpdValue;
	public Image weighDownSklIcon;
	public Text weighDownSklValue;

	[Header("Inventory Stats")]
	public GameObject inventoryObject;
	public Text conText;
	public Text weighDownValue2;
	public Image[] weaponSkillIcons;
	public Text[] weaponSkillRating;
	public Image[] inventoryHighlight;
	public Text[] inventoryFields;
	public Text[] inventoryValues;

	[Header("Tooltip")]
	public GameObject tooltipObject;
	public Text tooltipText;

	[Header("Terrain Info")]
	public GameObject terrainObject;
	public Text terrainText;

	private StatsPage page;


	private void Start() {
		UpdateTooltip("");
		page = StatsPage.BASIC;
		HideStats();
	}

	/// <summary>
	/// Updates the information in the UI whenever the state or character changes.
	/// </summary>
	public void UpdateUI() {
		TacticsMove tactics = selectedCharacter.value;
		MapTile tile = selectedTile.value;
		bool active = true;

		//Set selected character to the targeted tile instead of the selected character
		if (actionMode.value == ActionMode.ATTACK || actionMode.value == ActionMode.HEAL || actionMode.value == ActionMode.TRADE) {
			tile = targetTile.value;
			if (tile)
				tactics = tile.currentCharacter;
		}

		if (currentMenuMode.value != (int)MenuMode.MAP && currentMenuMode.value != (int)MenuMode.PREP &&
			currentMenuMode.value != (int)MenuMode.INV) {
			HideStats();
			active = false;
		}
		else if (tactics == null && tile.interactType != InteractType.NONE) {
			ShowObjectStats(tile);
		}
		else if (tactics == null) {
			HideStats();
		}
		else if (page == StatsPage.INVENTORY || currentMenuMode.value == (int)MenuMode.INV) {
			ShowInventoryStats(tactics);
		}
		else if (page == StatsPage.STATS) {
			ShowStatsStats(tactics);
		}
		else if (page == StatsPage.BASIC) {
			ShowBasicStats(tactics);
		}

		ShowTerrainInfo(active);
		flipButton.SetActive(currentMenuMode.value != (int)MenuMode.INV);
	}
	
	/// <summary>
	/// Changes the stats screen to the next one.
	/// </summary>
	/// <param name="dir"></param>
	public void ChangeStatsScreen() {
		switch (page) {
		case StatsPage.BASIC:		page = StatsPage.STATS; break;
		case StatsPage.STATS:		page = StatsPage.INVENTORY; break;
		case StatsPage.INVENTORY:	page = StatsPage.BASIC; break;
		}
		UpdateUI();
	}

	/// <summary>
	/// Hides the stats page.
	/// </summary>
	private void HideStats() {
		menuView.SetActive(false);
		statsObject.SetActive(false);
		basicObject.SetActive(false);
		inventoryObject.SetActive(false);
	}
	
	/// <summary>
	/// Shows a basic overview of the character with some stats and combat stats.
	/// </summary>
	/// <param name="tactics"></param>
	private void ShowBasicStats(TacticsMove tactics) {
		if (tactics == null)
			return;
		StatsContainer stats = tactics.stats;
		SkillsContainer skills = tactics.skills;
		menuView.SetActive(true);
		statsObject.SetActive(false);
		basicObject.SetActive(true);
		inventoryObject.SetActive(false);

//		colorBackground.color = (tactics.faction == Faction.PLAYER) ? 
//			new Color(0.2f,0.2f,0.5f) : new Color(0.5f,0.2f,0.2f);
		
		characterName.text = stats.charData.entryName;
		portrait.enabled = true;
		portrait.sprite = stats.charData.portrait;
		currentHpText.text = tactics.currentHealth + " / " + stats.hp;
		healthBar.fillAmount = tactics.GetHealthPercent();
		weakIcon1.sprite = weaknessImages[(int)stats.classData.classType];
		weakIcon1.enabled = (weakIcon1.sprite != null);

		ItemEntry weapon = tactics.GetEquippedWeapon(ItemCategory.WEAPON).item;
		wpnIcon.sprite = (weapon != null) ? weapon.icon : null;
		wpnName.text = (weapon != null) ? weapon.entryName : "";
		
		for (int i = 0; i < skillImages.Length; i++) {
			if (i >= skills.skills.Length || skills.skills[i] == null) {
				skillImages[i].sprite = noSkillImage;
			}
			else {
				skillImages[i].sprite = skills.skills[i].icon;
			}
		}

		int hitrate = BattleCalc.GetHitRate(weapon, stats);
		pwrText.text = (hitrate != -1) ? "Hit:  " + hitrate : "Hit:  --";
		int pwer = BattleCalc.CalculateDamage(weapon, stats);
		hitText.text = (pwer != -1) ? "Pwr:  " + pwer : "Pwr:  --";
		int critrate = BattleCalc.GetCritRate(weapon, stats);
		critText.text = (critrate != -1) ? "Crit:   " + critrate : "Crit:   --";
		avoidText.text = "Avo:  " + BattleCalc.GetAvoid(stats);

		//Terrain
		boostAvoid.enabled = (tactics.currentTile.terrain.avoid > 0);
	}

	/// <summary>
	/// Shows the stats page. Contains information about the current stats of the character.
	/// </summary>
	/// <param name="tactics"></param>
	private void ShowStatsStats(TacticsMove tactics) {
		if (tactics == null)
			return;
		StatsContainer stats = tactics.stats;
		menuView.SetActive(true);
		statsObject.SetActive(true);
		basicObject.SetActive(false);
		inventoryObject.SetActive(false);
		characterName.text = stats.charData.entryName;

		hpText.color = (stats.bHp != 0) ? Color.green : Color.black;
		atkText.color = (stats.bAtk != 0) ? Color.green : Color.black;
		spdText.color = (stats.bSpd != 0) ? Color.green : Color.black;
		sklText.color = (stats.bSkl != 0) ? Color.green : Color.black;
		lckText.color = (stats.bLck != 0) ? Color.green : Color.black;
		defText.color = (stats.bDef != 0) ? Color.green : Color.black;
		resText.color = (stats.bRes != 0) ? Color.green : Color.black;
		
		ItemEntry weapon = tactics.GetEquippedWeapon(ItemCategory.WEAPON).item;
		int penalty = stats.GetConPenalty(weapon);
		if (penalty > 0) {
			spdText.text = BattleCalc.GetAttackSpeed(weapon, stats).ToString();
			weighDownSpdIcon.enabled = true;
			weighDownSpdValue.text = (-penalty).ToString();
			sklText.text = (stats.skl - penalty).ToString();
			weighDownSklIcon.enabled = true;
			weighDownSklValue.text = (-penalty).ToString();
		}
		else {
			spdText.text = stats.spd.ToString();
			sklText.text = stats.skl.ToString();
			weighDownSpdIcon.enabled = false;
			weighDownSpdValue.text = "";
			weighDownSklIcon.enabled = false;
			weighDownSklValue.text = "";
		}

		levelText.text = stats.currentLevel.ToString();
		expText.text = stats.currentExp.ToString();
		hpText.text = stats.hp.ToString();
		atkText.text = stats.atk.ToString();
		lckText.text = stats.lck.ToString();
		defText.text = stats.def.ToString();
		resText.text = stats.res.ToString();
		movText.text = stats.GetMovespeed().ToString();
	}

	/// <summary>
	/// Displays the inventory page. Contains information on the inventory, weaponskills and constitution.
	/// </summary>
	/// <param name="tactics"></param>
	private void ShowInventoryStats(TacticsMove tactics) {
		if (tactics == null)
			return;
		StatsContainer stats = tactics.stats;
		InventoryContainer inventory = tactics.inventory;
		menuView.SetActive(true);
		statsObject.SetActive(false);
		basicObject.SetActive(false);
		inventoryObject.SetActive(true);
		characterName.text = stats.charData.entryName;

		ItemEntry weapon = tactics.GetEquippedWeapon(ItemCategory.WEAPON).item;
		conText.text = stats.GetConstitution().ToString();
		int atkSpeed = BattleCalc.GetAttackSpeed(weapon, stats);
		if (atkSpeed < stats.spd) {
			weighDownValue2.color = new Color(0.5f,0.2f,0.2f);
			weighDownValue2.text = "Penalty:  " + (atkSpeed - stats.spd).ToString();
		}
		else {
			weighDownValue2.color = Color.grey;
			weighDownValue2.text = "No penalty";
		}

		for (int i = 0; i < weaponSkillIcons.Length; i++) {
			if (i >= stats.classData.weaponSkills.Count){
				weaponSkillIcons[i].transform.parent.gameObject.SetActive(false);
			}
			else {
				weaponSkillIcons[i].transform.parent.gameObject.SetActive(true);
				weaponSkillIcons[i].sprite = weaponSkillImages[(int)stats.classData.weaponSkills[i]];
				weaponSkillRating[i].text = ItemEntry.GetRankLetter(stats.wpnSkills[(int)stats.classData.weaponSkills[i]]);
			}
		}

		// Set up inventory list
		for (int i = 0; i < 5; i++) {
			if (i >= InventoryContainer.INVENTORY_SIZE || inventory.GetTuple(i).item == null) {
				inventoryFields[i].color = Color.black;
				inventoryFields[i].text = "---";
				inventoryValues[i].text = " ";
			}
			else {
				InventoryTuple tuple = inventory.GetTuple(i);
				int skill = stats.GetWpnSkill(tuple.item);
				inventoryFields[i].color = (tuple.droppable) ? Color.green : 
							(tuple.item.CanUse(skill)) ? Color.black : Color.grey;
				inventoryFields[i].text = tuple.item.entryName;
				inventoryValues[i].text = (tuple.item.maxCharge >= 0) ? tuple.charge.ToString() : " ";
				if (tuple.item.itemCategory == ItemCategory.CONSUME && tuple.item.maxCharge == 1)
					inventoryValues[i].text = " ";
			}
		}

		UpdateSelection();
	}
	
	/// <summary>
	/// Shows the stats for a non character tile.
	/// </summary>
	/// <param name="tile"></param>
	private void ShowObjectStats(MapTile tile) {
		if (tile.interactType == InteractType.BLOCK) {
			StatsContainer stats = tile.blockMove.stats;
	//		colorBackground.color = (tactics.faction == Faction.PLAYER) ? 
	//			new Color(0.2f,0.2f,0.5f) : new Color(0.5f,0.2f,0.2f);
			
			characterName.text = stats.charData.entryName;
			portrait.enabled = true;
			portrait.sprite = stats.charData.portrait;
			currentHpText.text = tile.blockMove.currentHealth + " / " + stats.hp;
			healthBar.fillAmount = tile.blockMove.GetHealthPercent();
		}
		else if (tile.interactType == InteractType.VILLAGE) {
			characterName.text = "Village";
			portrait.enabled = true;
			portrait.sprite = tile.terrain.sprite;
			currentHpText.text = (tile.interacted) ? "Visited" : "Not Visited";
			healthBar.fillAmount = (tile.interacted) ? 1 : 0;
		}

		weakIcon1.sprite = null;
		weakIcon1.enabled = false;
		wpnIcon.sprite = null;
		wpnName.text = "";
		pwrText.text = "Hit:  --";
		hitText.text = "Pwr:  --";
		critText.text = "Crit:   --";
		avoidText.text = "Avo:  --";
		boostAvoid.enabled = false;
		
		menuView.SetActive(true);
		statsObject.SetActive(false);
		basicObject.SetActive(true);
		inventoryObject.SetActive(false);
	}

	/// <summary>
	/// Shows some terrain information of the currently selected tile.
	/// </summary>
	/// <param name="terrain"></param>
	/// <param name="active"></param>
	private void ShowTerrainInfo(bool active) {
		TerrainTile terrain = selectedTile.value.terrain;
		terrainObject.SetActive(active);
		if (active) {
			if (terrain.healPercent != 0)
				terrainText.text = string.Format("{0}    Def: {1}\nAvo: {2}    Heal: {3}", terrain.name, terrain.defense, terrain.avoid, terrain.healPercent);
			else
				terrainText.text = string.Format("{0}\nDef: {1}    Avo: {2}", terrain.name, terrain.defense, terrain.avoid);
		}
	}

	/// <summary>
	/// Updates the highlight for each inventory slot and sets the appropriate color.
	/// </summary>
	public void UpdateSelection() {
		for (int i = 0; i < 5; i++) {
			inventoryHighlight[i].enabled = (i == inventoryIndex.value);
			inventoryHighlight[i].color = (currentMenuMode.value == (int)MenuMode.INV) ? new Color(0.35f,0.7f,1f,0.6f) : new Color(0.35f,1f,1f,0.75f);
		}
	}

	/// <summary>
	/// Updates the tooltip text. Disable the tooltip with empty message.
	/// </summary>
	/// <param name="message"></param>
	public void UpdateTooltip(string message) {
		tooltipText.text = message;
		tooltipObject.SetActive(!string.IsNullOrEmpty(message));
	}
}
