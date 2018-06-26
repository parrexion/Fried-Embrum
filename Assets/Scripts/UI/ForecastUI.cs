using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class ForecastUI : MonoBehaviour {
	
	public bool inBattle;
	public TacticsMoveVariable selectCharacter;
	public MapTileVariable walkTile;
	public MapTileVariable attackTile;
	public ActionModeVariable currentMode;

	public GameObject backgroundFight;
	public GameObject backgroundHeal;
	public GameObject backgroundInBattle;
	
	[Header("Attacker Stats")]
//	public Image colorBackground;
	public Image portrait;
	public Text characterName;
	public Image wpnIcon;
	public Text wpnName;
	public Text wpnCharge;
	public Text hpText;
	public Text dmgText;
	public GameObject doubleDamage;
	public Text hitText;
	public Text critText;
	private TacticsMove _attackerTactics;
	
	[Header("Defender Stats")]
//	public Image colorBackground;
	public Image ePortrait;
	public Text eCharacterName;
	public Image eWpnIcon;
	public Text eWpnName;
	public Text eWpnCharge;
	public Text eHpText;
	public Text eDmgText;
	public GameObject eDoubleDamage;
	public Text eHitText;
	public Text eCritText;
	private TacticsMove _defenderTactics;
	
	[Header("Healing Stats")]
//	public Image colorBackground;
	public Image hPortrait;
	public Text hCharacterName;
	public Image hWpnIcon;
	public Text hWpnName;
	public Text hWpnCharge;
	public Text hHealText;
	public Image hPortrait2;
	public Text hCharacterName2;


	public void UpdateUI() {
		if (selectCharacter.value == null || currentMode.value == ActionMode.NONE || currentMode.value == ActionMode.MOVE) {
			if (!inBattle) {
				backgroundFight.SetActive(false);
				backgroundHeal.SetActive(false);
			}
		}
		else {
			CalculateShowForecast(selectCharacter.value, attackTile.value.currentCharacter);
		}
	}

	public void UpdateHealthUI() {
		hpText.text = _attackerTactics.currentHealth.ToString();
		eHpText.text = _defenderTactics.currentHealth.ToString();
	}

	private void CalculateShowForecast(TacticsMove attacker, TacticsMove defender) {
		bool isDamage = (currentMode.value == ActionMode.ATTACK);
		BattleAction act1 = new BattleAction(true, isDamage, attacker, defender);

		if (isDamage) {
			if (inBattle)
				backgroundInBattle.SetActive(true);
			BattleAction act2 = new BattleAction(false, true, defender, attacker);
			int distance = MapCreator.DistanceTo(defender, walkTile.value);
			int atk = (attacker.GetWeapon(ItemCategory.WEAPON).InRange(distance)) ? act1.GetDamage() : -1;
			int def = (defender.GetWeapon(ItemCategory.WEAPON) != null && defender.GetWeapon(ItemCategory.WEAPON).InRange(distance)) ? act2.GetDamage() : -1;
			int spd = attacker.stats.spd - defender.stats.spd;
			int hit = (atk != -1) ? act1.GetHitRate() : -1;
			int hit2 = (def != -1) ? act2.GetHitRate() : -1;
			int crit = (atk != -1) ? act1.GetCritRate() : -1;
			int crit2 = (def != -1) ? act2.GetCritRate() : -1;
//			bool atkWeak = attacker.stats.IsWeakAgainst(defender.stats.GetItem(ItemCategory.WEAPON));
//			bool defWeak = defender.stats.IsWeakAgainst(attacker.stats.GetItem(ItemCategory.WEAPON));
//			ShowForecast(attacker, defender, atk, def, spd, act1.GetAdvantage(), atkWeak, defWeak);
			ShowAttackerStats(attacker, atk, spd, hit, crit);
			ShowDefenderStats(defender, def, spd, hit2, crit2);
			if (!inBattle) {
				backgroundFight.SetActive(true);
				backgroundHeal.SetActive(false);
			}
		}
		else {
			ShowHealForecast(attacker, defender, act1.GetHeals());
			if (!inBattle) {
				backgroundFight.SetActive(false);
				backgroundHeal.SetActive(true);
			}
		}
	}
	
	private void ShowAttackerStats(TacticsMove tactics, int damage, int speed, int hit, int crit) {
		_attackerTactics = tactics;
		StatsContainer stats = tactics.stats;
//		colorBackground.color = (tactics.faction == Faction.PLAYER) ? new Color(0.5f,0.8f,1f) : new Color(1f,0.5f,0.8f);
		
		characterName.text = stats.charData.charName;
		portrait.sprite = stats.charData.portrait;
		InventoryTuple weapon = stats.GetItemTuple(ItemCategory.WEAPON);
		wpnIcon.sprite = (weapon != null) ? weapon.item.icon : null;
		wpnName.text = (weapon != null) ? weapon.item.itemName : "";
		if (wpnCharge)
			wpnCharge.text = (weapon != null) ? weapon.charge.ToString() : "";

		hpText.text = tactics.currentHealth.ToString();
		dmgText.text = (damage != -1) ? damage.ToString() : "--";
		doubleDamage.SetActive(speed >= 5);
		hitText.text = hit.ToString();
		critText.text = crit.ToString();
	}
	
	private void ShowDefenderStats(TacticsMove tactics, int damage, int speed, int hit, int crit) {
		_defenderTactics = tactics;
		StatsContainer stats = tactics.stats;
//		colorBackground.color = (tactics.faction == Faction.PLAYER) ? new Color(0.5f,0.8f,1f) : new Color(1f,0.5f,0.8f);
		
		eCharacterName.text = stats.charData.charName;
		ePortrait.sprite = stats.charData.portrait;
		InventoryTuple weapon = stats.GetItemTuple(ItemCategory.WEAPON);
		eWpnIcon.sprite = (weapon != null) ? weapon.item.icon : null;
		eWpnName.text = (weapon != null) ? weapon.item.itemName : "";
		if (eWpnCharge)
			eWpnCharge.text = (weapon != null) ? weapon.charge.ToString() : "";

		eHpText.text = tactics.currentHealth.ToString();
		eDmgText.text = (damage != -1) ? damage.ToString() : "--";
		eDoubleDamage.SetActive(speed <= -5);
		eHitText.text = hit.ToString();
		eCritText.text = crit.ToString();
	}
	
	private void ShowHealForecast(TacticsMove healer, TacticsMove receiver, int heal) {
		if (inBattle)
			backgroundInBattle.SetActive(false);
		_attackerTactics = healer;
		_defenderTactics = receiver;
		StatsContainer stats = healer.stats;
		
		hCharacterName.text = stats.charData.charName;
		hPortrait.sprite = stats.charData.portrait;
		InventoryTuple staff = stats.GetItemTuple(ItemCategory.STAFF);
		hWpnIcon.sprite = (staff != null) ? staff.item.icon : null;
		hWpnName.text = (staff != null) ? staff.item.itemName : "";
		if (!inBattle)
			hWpnCharge.text = (staff != null) ? staff.charge.ToString() : "";

		stats = receiver.stats;
		hCharacterName2.text = stats.charData.charName;
		hPortrait2.sprite = stats.charData.portrait;
		if (inBattle) {
			hpText.text = healer.currentHealth.ToString();
			eHpText.text = receiver.currentHealth.ToString();

			InventoryTuple weapon = stats.GetItemTuple(ItemCategory.WEAPON);
			eWpnIcon.sprite = (weapon != null) ? weapon.item.icon : null;
			eWpnName.text = (weapon != null) ? weapon.item.itemName : "";
		}
		else {
			hHealText.text = string.Format("{0} → {1} ({2})",
					receiver.currentHealth,
					Mathf.Min(receiver.currentHealth + heal, stats.hp),
					stats.hp);
		}
	}
}
