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
	public MapTileVariable defendCharacter;
	public ActionModeVariable currentMode;
	public IntVariable doublingSpeed;

	[Header("Arrows")]
	public Sprite advArrow;
	public Sprite disArrow;

	[Header("Objects")]
	public GameObject backgroundFight;
	public GameObject backgroundHeal;
	public GameObject backgroundInBattle;
	
	[Header("Attacker Stats")]
//	public Image colorBackground;
	public Image portrait;
	public Text characterName;
	public Image wpnAdvantage;
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
	public Image eWpnAdvantage;
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

	public IntVariable battleWeaponIndex;


	public void UpdateUI(bool active) {
		if (!active) {
			if (!inBattle) {
				backgroundFight.SetActive(false);
				backgroundHeal.SetActive(false);
			}
		}
		else {
			CalculateShowForecast(selectCharacter.value, defendCharacter.value);
		}
	}

	public void UpdateHealthUI() {
		hpText.text = _attackerTactics.currentHealth.ToString();
		eHpText.text = _defenderTactics.currentHealth.ToString();
	}

	private void CalculateShowForecast(TacticsMove attacker, MapTile target) {
		if (target.currentCharacter == null) {
			TacticsMove defender = target.blockMove;
			bool attackmode = (currentMode.value == ActionMode.ATTACK);
			BattleAction act1 = new BattleAction(true, attackmode, attacker, defender);
			_attackerTactics = attacker;
			_defenderTactics = defender;
			act1.weaponAtk = attacker.inventory.GetTuple(battleWeaponIndex.value);

			if (attackmode) {
				Debug.Log("ATTACK1!");
				if (inBattle)
					backgroundInBattle.SetActive(true);

				int distance = BattleMap.DistanceTo(defender, walkTile.value);
				int atk = (act1.weaponAtk.item.InRange(distance)) ? act1.GetDamage() : -1;
				int ret = -1;
				int spd = 0;
				int hit = (atk != -1) ? act1.GetHitRate() : -1;
				int hit2 = -1;
				int crit = -1;
				int crit2 = -1;
				bool atkWeak = false;
				bool defWeak = false;
				int atkAdv = 0;
				int defAdv = 0;
				ShowAttackerStats(attacker, act1.weaponAtk, atk, spd, hit, crit, atkAdv, atkWeak);
				ShowDefenderStats(defender, null, ret, spd, hit2, crit2, defAdv, defWeak);
				if (!inBattle) {
					backgroundFight.SetActive(true);
					backgroundHeal.SetActive(false);
				}
			}
		}
		else {
			TacticsMove defender = target.currentCharacter;
			bool attackmode = (currentMode.value == ActionMode.ATTACK);
			BattleAction act1 = new BattleAction(true, attackmode, attacker, defender);
			_attackerTactics = attacker;
			_defenderTactics = defender;
			act1.weaponAtk = attacker.inventory.GetTuple(battleWeaponIndex.value);

			if (attackmode) {
				if (inBattle)
					backgroundInBattle.SetActive(true);

				BattleAction act2 = new BattleAction(false, true, defender, attacker);
				act2.weaponDef = attacker.inventory.GetTuple(battleWeaponIndex.value);
				int distance = BattleMap.DistanceTo(defender, walkTile.value);
				int atk = (act1.weaponAtk.item.InRange(distance)) ? act1.GetDamage() : -1;
				int ret = (act1.DefenderInRange(distance)) ? act2.GetDamage() : -1;
				int spd = act1.GetSpeedDifference();
				int hit = (atk != -1) ? act1.GetHitRate() : -1;
				int hit2 = (ret != -1) ? act2.GetHitRate() : -1;
				int crit = (atk != -1) ? act1.GetCritRate() : -1;
				int crit2 = (ret != -1) ? act2.GetCritRate() : -1;
				bool atkWeak = act1.CheckWeaponWeakness();
				bool defWeak = act2.CheckWeaponWeakness();
				int atkAdv = act1.GetAdvantage();
				int defAdv = act2.GetAdvantage();
				ShowAttackerStats(attacker, act1.weaponAtk, atk, spd, hit, crit, atkAdv, atkWeak);
				ShowDefenderStats(defender, act2.weaponAtk, ret, spd, hit2, crit2, defAdv, defWeak);
				if (!inBattle) {
					backgroundFight.SetActive(true);
					backgroundHeal.SetActive(false);
				}
			}
			else {
				Debug.Log("HEAL!");
				ShowHealForecast(attacker, defender, act1.weaponAtk);
				if (!inBattle) {
					backgroundFight.SetActive(false);
					backgroundHeal.SetActive(true);
				}
			}
		}
	}
	
	private void ShowAttackerStats(TacticsMove tactics, InventoryTuple InvTup, int damage, int speed, int hit, int crit, int atkAdv, bool defWeak) {
//		colorBackground.color = (tactics.faction == Faction.PLAYER) ? new Color(0.5f,0.8f,1f) : new Color(1f,0.5f,0.8f);
		
		characterName.text = tactics.stats.charData.entryName;
		portrait.sprite = tactics.stats.charData.portrait;
		wpnAdvantage.enabled = (atkAdv != 0);
		wpnAdvantage.sprite = (atkAdv == 1) ? advArrow : disArrow;
		wpnIcon.sprite = (InvTup.item != null) ? InvTup.item.icon : null;
		wpnName.text = (InvTup.item != null) ? InvTup.item.entryName : "";
		if (wpnCharge)
			wpnCharge.text = (InvTup.item != null) ? InvTup.charge.ToString() : "";

		hpText.text = tactics.currentHealth.ToString();
		dmgText.text = (damage != -1) ? damage.ToString() : "--";
		dmgText.color = (damage != -1 && defWeak) ? Color.green : Color.black;
		doubleDamage.SetActive(speed >= doublingSpeed.value);
		hitText.text = (hit != -1) ? hit.ToString() : "--";
		critText.text = (crit != -1) ? crit.ToString() : "--";
	}
	
	private void ShowDefenderStats(TacticsMove tactics, InventoryTuple invTup, int damage, int speed, int hit, int crit, int defAdv, bool atkWeak) {
//		colorBackground.color = (tactics.faction == Faction.PLAYER) ? new Color(0.5f,0.8f,1f) : new Color(1f,0.5f,0.8f);
		
		eCharacterName.text = tactics.stats.charData.entryName;
		ePortrait.sprite = tactics.stats.charData.portrait;
		eWpnAdvantage.enabled = (defAdv != 0);
		eWpnAdvantage.sprite = (defAdv == 1) ? advArrow : disArrow;
		eWpnIcon.sprite = (invTup.item != null) ? invTup.item.icon : null;
		eWpnName.text = (invTup.item != null) ? invTup.item.entryName : "";
		if (eWpnCharge)
			eWpnCharge.text = (invTup.item != null) ? invTup.charge.ToString() : "";

		eHpText.text = tactics.currentHealth.ToString();
		eDmgText.text = (damage != -1) ? damage.ToString() : "--";
		eDmgText.color = (damage != -1 && atkWeak) ? Color.green : Color.black;
		eDoubleDamage.SetActive(speed <= - doublingSpeed.value);
		eHitText.text = (hit != -1) ? hit.ToString() : "--";
		eCritText.text = (crit != -1) ? crit.ToString() : "--";
	}
	
	private void ShowHealForecast(TacticsMove healer, TacticsMove receiver, InventoryTuple staff) {
		if (inBattle)
			backgroundInBattle.SetActive(false);
		StatsContainer stats = healer.stats;
		
		hCharacterName.text = stats.charData.entryName;
		hPortrait.sprite = stats.charData.portrait;

		stats = receiver.stats;
		hCharacterName2.text = stats.charData.entryName;
		hPortrait2.sprite = stats.charData.portrait;
		if (inBattle) {
			hpText.text = healer.currentHealth.ToString();
			eHpText.text = receiver.currentHealth.ToString();

			eWpnIcon.sprite = null;
			eWpnName.text = "--";
		}

		hWpnIcon.sprite = (staff.item != null) ? staff.item.icon : null;
		hWpnName.text = (staff.item != null) ? staff.item.entryName : "";

		if (!inBattle) {
			hWpnCharge.text = (staff.item != null) ? staff.charge.ToString() : "";
			hHealText.text = string.Format("{0} → {1} ({2})",
					_defenderTactics.currentHealth,
					Mathf.Min(_defenderTactics.currentHealth + BattleCalc.CalculateHeals(staff.item, _attackerTactics.stats), _defenderTactics.stats.hp),
					_defenderTactics.stats.hp);
		}
	}

}
