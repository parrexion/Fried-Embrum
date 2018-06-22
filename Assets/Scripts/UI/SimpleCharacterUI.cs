using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SimpleCharacterUI : MonoBehaviour {

	public enum StatsType { BASIC, STATS, INVENTORY }
	
	public TacticsMoveVariable selectCharacter;
	public ActionModeVariable currentMode;
	public InventoryController invController;
	
	public StatsType currentStats;
	public GameObject background;
	
	[Header("Basic Stats")]
	public GameObject basicObject;
	public Image colorBackground;
	public Image portrait;
	public Text characterName;
	public Text currentHpText;
	public Image healthBar;
	public Image wpnIcon;
	public Text wpnName;
	public Image[] skillImages;

	[Header("Stats Stats")]
	public GameObject statsObject;
	public Text levelText;
	public Text hpText;
	public Text atkText;
	public Text spdText;
	public Text sklText;
	public Text lckText;
	public Text defText;
	public Text resText;

	[Header("Inventory Stats")]
	public GameObject inventoryObject;
	public Text[] inventoryFields;
	public Text[] inventoryValues;


	public void UpdateUI() {
		if (selectCharacter.value == null || (currentMode.value != ActionMode.NONE && currentMode.value != ActionMode.MOVE) ) {
			background.SetActive(false);
			invController.HideTooltip();
		}
		else {
			characterName.text = selectCharacter.value.stats.charData.charName;	
			if (currentStats == StatsType.BASIC)
				ShowBasicStats(selectCharacter.value);
			else if (currentStats == StatsType.STATS) {
				ShowStatsStats(selectCharacter.value);
			}
			else if (currentStats == StatsType.INVENTORY) {
				ShowInventoryStats(selectCharacter.value);
			}
			background.SetActive(true);
		}
	}
	
	private void ShowBasicStats(TacticsMove tactics) {
		StatsContainer stats = tactics.stats;
//		colorBackground.color = (tactics.faction == Faction.PLAYER) ? 
//			new Color(0.2f,0.2f,0.5f) : new Color(0.5f,0.2f,0.2f);
		
		portrait.enabled = true;
		portrait.sprite = stats.charData.portrait;
		currentHpText.text = tactics.currentHealth + " / " + stats.hp;
		healthBar.fillAmount = tactics.GetHealthPercent();

		for (int i = 0; i < skillImages.Length; i++) {
			if (i >= stats.skills.Length) {
				skillImages[i].enabled = false;
			}
			else {
				skillImages[i].sprite = stats.skills[i].icon;
				skillImages[i].enabled = true;
			}
		}

		wpnIcon.sprite = (stats.GetItem(ItemCategory.WEAPON) != null) ? stats.GetItem(ItemCategory.WEAPON).icon : null;
		wpnName.text = (stats.GetItem(ItemCategory.WEAPON) != null) ? stats.GetItem(ItemCategory.WEAPON).itemName : "";
		
		statsObject.SetActive(false);
		basicObject.SetActive(true);
		inventoryObject.SetActive(false);
	}

	private void ShowStatsStats(TacticsMove tactics) {
		StatsContainer stats = tactics.stats;
		statsObject.SetActive(true);
		basicObject.SetActive(false);
		inventoryObject.SetActive(false);

		hpText.color = (stats.bHp != 0) ? Color.green : Color.black;
		atkText.color = (stats.bAtk != 0) ? Color.green : Color.black;
		spdText.color = (stats.bSpd != 0) ? Color.green : Color.black;
		sklText.color = (stats.bSkl != 0) ? Color.green : Color.black;
		lckText.color = (stats.bLck != 0) ? Color.green : Color.black;
		defText.color = (stats.bDef != 0) ? Color.green : Color.black;
		resText.color = (stats.bRes != 0) ? Color.green : Color.black;
		
		levelText.text = stats.level.ToString();
		hpText.text = stats.hp.ToString();
		atkText.text = stats.atk.ToString();
		spdText.text = stats.spd.ToString();
		sklText.text = stats.skl.ToString();
		lckText.text = stats.lck.ToString();
		defText.text = stats.def.ToString();
		resText.text = stats.res.ToString();
	}

	private void ShowInventoryStats(TacticsMove tactics) {
		StatsContainer stats = tactics.stats;
		statsObject.SetActive(false);
		basicObject.SetActive(false);
		inventoryObject.SetActive(true);
		
		for (int i = 0; i < 5; i++) {
			if (i >= stats.inventory.Length || stats.inventory[i] == null) {
				inventoryFields[i].color = Color.black;
				inventoryFields[i].text = "---";
				inventoryValues[i].text = "";
			}
			else {
				inventoryFields[i].color = (stats.inventory[i].droppable) ? Color.green : Color.black;
				inventoryFields[i].text = stats.inventory[i].itemName;
				inventoryValues[i].text = stats.inventory[i].maxCharge.ToString();
			}
		}
	}

	public void ChangeStatsScreen(int dir) {
		int nextPage = (int) currentStats + dir;
		if (nextPage < 0)
			nextPage = nextPage + 3;
		
		currentStats = (StatsType) (nextPage % 3);
		UpdateUI();
	}
}
