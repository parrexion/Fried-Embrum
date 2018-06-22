using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class ForecastUI : MonoBehaviour {
	
	public TacticsMoveVariable selectCharacter;
	public MapTileVariable walkTile;
	public MapTileVariable attackTile;
	public ActionModeVariable currentMode;
	public FactionVariable currentTurn;

	public GameObject background;
	
	[Header("Attacker Stats")]
//	public Image colorBackground;
	public Image portrait;
	public Text characterName;
	public Image wpnIcon;
	public Text wpnName;
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
	public Text eHpText;
	public Text eDmgText;
	public GameObject eDoubleDamage;
	public Text eHitText;
	public Text eCritText;
	private TacticsMove _defenderTactics;
	

	[Header("Tooltip")]
	public GameObject tooltipObject;
	public RectTransform tooltipBackground;
	public Text tooltipText;
	public StringVariable tooltipMessage;
	private bool _tooltipActive;


	public void UpdateUI() {
		if (selectCharacter.value == null || currentMode.value == ActionMode.NONE || currentMode.value == ActionMode.MOVE) {
			background.SetActive(false);
		}
		else {
			CalculateShowForecast(selectCharacter.value, attackTile.value.currentCharacter);
			background.SetActive(true);
		}
	}

	public void UpdateHealthUI() {
		hpText.text = _attackerTactics.currentHealth.ToString();
		eHpText.text = _defenderTactics.currentHealth.ToString();
	}
	
	private void ShowAttackerStats(TacticsMove tactics, int damage, int speed, int hit, int crit) {
		_attackerTactics = tactics;
		StatsContainer stats = tactics.stats;
//		colorBackground.color = (tactics.faction == Faction.PLAYER) ? new Color(0.5f,0.8f,1f) : new Color(1f,0.5f,0.8f);
		
		characterName.text = stats.charData.charName;
		portrait.enabled = true;
		portrait.sprite = stats.charData.portrait;
		wpnIcon.sprite = (stats.GetItem(ItemCategory.WEAPON) != null) ? stats.GetItem(ItemCategory.WEAPON).icon : null;
		wpnName.text = (stats.GetItem(ItemCategory.WEAPON) != null) ? stats.GetItem(ItemCategory.WEAPON).itemName : "";

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
		ePortrait.enabled = true;
		ePortrait.sprite = stats.charData.portrait;
		eWpnIcon.sprite = (stats.GetItem(ItemCategory.WEAPON) != null) ? stats.GetItem(ItemCategory.WEAPON).icon : null;
		eWpnName.text = (stats.GetItem(ItemCategory.WEAPON) != null) ? stats.GetItem(ItemCategory.WEAPON).itemName : "";

		eHpText.text = tactics.currentHealth.ToString();
		eDmgText.text = (damage != -1) ? damage.ToString() : "--";
		eDoubleDamage.SetActive(speed <= -5);
		eHitText.text = hit.ToString();
		eCritText.text = crit.ToString();
	}

	private void CalculateShowForecast(TacticsMove attacker, TacticsMove defender) {
		bool isDamage = (currentMode.value == ActionMode.ATTACK);
		BattleAction act1 = new BattleAction(true, isDamage, attacker, defender);

		if (isDamage) {
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
		}
		else {
//			ShowHealForecast(attacker, defender, act1.GetHeals());
		}
	}
}
