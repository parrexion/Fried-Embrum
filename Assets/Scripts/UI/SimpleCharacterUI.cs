﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SimpleCharacterUI : MonoBehaviour {

	[Header("References")]
	public TacticsMoveVariable selectCharacter;
	public TacticsMoveVariable targetCharacter;
	public IntVariable inventoryIndex;
	public IntVariable currentMenuMode;
	
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
	public GameObject equipIndicator;
	public Image[] inventoryHighlight;
	public Text[] inventoryFields;
	public Text[] inventoryValues;
	public GameObject helpButtons;

	
	public void ShowBasicStats(TacticsMove tactics) {
		StatsContainer stats = tactics.stats;
//		colorBackground.color = (tactics.faction == Faction.PLAYER) ? 
//			new Color(0.2f,0.2f,0.5f) : new Color(0.5f,0.2f,0.2f);
		
		characterName.text = stats.charData.charName;
		portrait.enabled = true;
		portrait.sprite = stats.charData.portrait;
		currentHpText.text = tactics.currentHealth + " / " + stats.hp;
		healthBar.fillAmount = tactics.GetHealthPercent();
		weakIcon1.sprite = weaknessImages[(int)stats.classData.classType];
		weakIcon1.enabled = (weakIcon1.sprite != null);

		WeaponItem weapon = tactics.GetFirstUsableItem(ItemCategory.WEAPON);
		wpnIcon.sprite = (weapon != null) ? weapon.icon : null;
		wpnName.text = (weapon != null) ? weapon.itemName : "";
		
		for (int i = 0; i < skillImages.Length; i++) {
			if (i >= stats.skills.Length || stats.skills[i] == null) {
				skillImages[i].sprite = noSkillImage;
			}
			else {
				skillImages[i].sprite = stats.skills[i].icon;
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
		boostAvoid.enabled = (tactics.GetTerrain().avoid > 0);
		
		statsObject.SetActive(false);
		basicObject.SetActive(true);
		inventoryObject.SetActive(false);
	}

	public void ShowStatsStats(TacticsMove tactics) {
		StatsContainer stats = tactics.stats;
		statsObject.SetActive(true);
		basicObject.SetActive(false);
		inventoryObject.SetActive(false);
		characterName.text = stats.charData.charName;

		hpText.color = (stats.bHp != 0) ? Color.green : Color.black;
		atkText.color = (stats.bAtk != 0) ? Color.green : Color.black;
		spdText.color = (stats.bSpd != 0) ? Color.green : Color.black;
		sklText.color = (stats.bSkl != 0) ? Color.green : Color.black;
		lckText.color = (stats.bLck != 0) ? Color.green : Color.black;
		defText.color = (stats.bDef != 0) ? Color.green : Color.black;
		resText.color = (stats.bRes != 0) ? Color.green : Color.black;
		
		WeaponItem weapon = tactics.GetFirstUsableItem(ItemCategory.WEAPON);
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

		levelText.text = stats.level.ToString();
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
	public void ShowInventoryStats(TacticsMove tactics) {
		StatsContainer stats = tactics.stats;
		InventoryContainer inventory = tactics.inventory;
		statsObject.SetActive(false);
		basicObject.SetActive(false);
		inventoryObject.SetActive(true);
		characterName.text = stats.charData.charName;

		WeaponItem weapon = tactics.GetFirstUsableItem(ItemCategory.WEAPON);
		conText.text = stats.GetConstitution().ToString();
		int atkSpeed = BattleCalc.GetAttackSpeed(weapon, stats);
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
		
		InventoryTuple first = inventory.inventory[0];
		bool equipped = (first.item != null && first.item.itemCategory == ItemCategory.WEAPON);
		equipIndicator.SetActive(equipped);

		// Set up inventory list
		for (int i = 0; i < 5; i++) {
			if (i >= inventory.inventory.Length || inventory.inventory[i].item == null) {
				inventoryFields[i].color = Color.black;
				inventoryFields[i].text = "---";
				inventoryValues[i].text = " ";
			}
			else {
				InventoryTuple tuple = inventory.inventory[i];
				int skill = stats.GetWpnSkill(tuple.item);
				inventoryFields[i].color = (tuple.droppable) ? Color.green : 
							(tuple.item.CanUse(skill)) ? Color.black : Color.grey;
				inventoryFields[i].text = tuple.item.itemName;
				inventoryValues[i].text = (tuple.item.maxCharge >= 0) ? tuple.charge.ToString() : " ";
				if (tuple.item.itemCategory == ItemCategory.CONSUME && tuple.item.maxCharge == 1)
					inventoryValues[i].text = " ";
			}
		}

		helpButtons.SetActive(currentMenuMode.value != (int)MenuMode.STATS &&currentMenuMode.value != (int)MenuMode.INV);

		UpdateSelection();
	}

	/// <summary>
	/// Updates the highlight for each inventory slot and sets the appropriate color.
	/// </summary>
	public void UpdateSelection() {
		for (int i = 0; i < 5; i++) {
			inventoryHighlight[i].enabled = (i == inventoryIndex.value);
			inventoryHighlight[i].color = (currentMenuMode.value == (int)MenuMode.STATS) ? new Color(0.35f,0.7f,1f,0.6f) : new Color(0.35f,1f,1f,0.75f);
		}
	}
}
