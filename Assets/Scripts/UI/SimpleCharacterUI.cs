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
	public Image[] weaponSkillIcons;
	public Text[] weaponSkillRating;
	public Text[] inventoryFields;
	public Text[] inventoryValues;
	public Text conText;
	public Text weighDownValue2;


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
		weakIcon1.sprite = weaknessImages[(int)stats.classData.classType];
		weakIcon1.enabled = (weakIcon1.sprite != null);

		wpnIcon.sprite = (stats.GetItem(ItemCategory.WEAPON) != null) ? stats.GetItem(ItemCategory.WEAPON).icon : null;
		wpnName.text = (stats.GetItem(ItemCategory.WEAPON) != null) ? stats.GetItem(ItemCategory.WEAPON).itemName : "";
		
		for (int i = 0; i < skillImages.Length; i++) {
			if (i >= stats.skills.Length || stats.skills[i] == null) {
				skillImages[i].sprite = noSkillImage;
			}
			else {
				skillImages[i].sprite = stats.skills[i].icon;
			}
		}

		int hitrate = stats.GetHitRate();
		pwrText.text = (hitrate != -1) ? "Hit:  " + hitrate : "Hit:  --";
		int pwer = stats.GetAttackPower();
		hitText.text = (pwer != -1) ? "Pwr:  " + pwer : "Pwr:  --";
		int critrate = stats.GetCriticalRate();
		critText.text = (critrate != -1) ? "Crit:   " + stats.GetCriticalRate() : "Crit:   --";
		avoidText.text = "Avo:  " + stats.GetAvoid();
		
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
		
		int penalty = stats.GetConPenalty();
		if (penalty > 0) {
			spdText.text = stats.GetAttackSpeed().ToString();
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

		levelText.text = stats.level.ToString();
		expText.text = stats.currentExp.ToString();
		hpText.text = stats.hp.ToString();
		atkText.text = stats.atk.ToString();
		lckText.text = stats.lck.ToString();
		defText.text = stats.def.ToString();
		resText.text = stats.res.ToString();
		movText.text = stats.GetMovespeed().ToString();
	}

	private void ShowInventoryStats(TacticsMove tactics) {
		StatsContainer stats = tactics.stats;
		statsObject.SetActive(false);
		basicObject.SetActive(false);
		inventoryObject.SetActive(true);

		conText.text = stats.GetConstitution().ToString();
		int atkSpeed = stats.GetAttackSpeed();
		if (atkSpeed < stats.spd) {
			weighDownValue2.text = "Penalty:  " + (atkSpeed - stats.spd).ToString();
		}
		else {
			weighDownValue2.text = "";
		}

		for (int i = 0; i < weaponSkillIcons.Length; i++) {
			if (i >= stats.classData.weaponSkills.Length){
				weaponSkillIcons[i].transform.parent.gameObject.SetActive(false);
			}
			else {
				weaponSkillIcons[i].transform.parent.gameObject.SetActive(true);
				weaponSkillIcons[i].sprite = weaponSkillImages[(int)stats.classData.weaponSkills[i]];
				weaponSkillRating[i].text = WeaponItem.GetRankLetter(stats.wpnSkills[(int)stats.classData.weaponSkills[i]]);
			}
		}
		
		for (int i = 0; i < 5; i++) {
			if (i >= stats.inventory.Length || stats.inventory[i] == null) {
				inventoryFields[i].color = Color.black;
				inventoryFields[i].text = "---";
				inventoryValues[i].text = " ";
			}
			else {
				InventoryTuple tuple = stats.inventory[i];
				inventoryFields[i].color = (tuple.droppable) ? Color.green : 
							(tuple.item.CanUse(stats)) ? Color.black : Color.grey;
				inventoryFields[i].text = tuple.item.itemName;
				inventoryValues[i].text = (tuple.item.maxCharge >= 0) ? tuple.charge.ToString() : " ";
				if (tuple.item.itemCategory == ItemCategory.CONSUME && tuple.item.maxCharge == 1)
					inventoryValues[i].text = " ";
			}
		}
	}

	public void ChangeStatsScreen(int dir) {
		int nextPage = (int) currentStats + dir;
		if (nextPage < 0)
			nextPage = nextPage + 3;
		
		currentStats = (StatsType) (nextPage % 3);
		invController.HideTooltip();
		UpdateUI();
	}
}
