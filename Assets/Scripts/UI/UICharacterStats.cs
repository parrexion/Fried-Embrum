using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterStats : MonoBehaviour {

	public TacticsMoveVariable selectCharacter;
	public CharacterStatsVariable clickCharacter;
	public MapTileVariable walkTile;
	public MapTileVariable attackTile;
	public ActionModeVariable currentMode;
	public FactionVariable currentTurn;

	public Sprite advIconGood;
	public Sprite advIconBad;

	[Header("Backgrounds")]
	public GameObject fullBackground;
	public GameObject attackBackground;
	
	[Header("Player Character Stats")]
	public GameObject playerStatsObject;
	public GameObject playerNormalObject;
	public GameObject playerForecastObject;
	public Image colorBackground;
	public Text characterName;
	public Image portrait;
	public Text hpText;
	public Text atkText;
	public Text spdText;
	public Text defText;
	public Text resText;

	[Header("Secondary Stats")]
	public Text levelText;
	public Image[] skillImages;
	public GameObject damageTextObj;
	public Text wpnName;

	public Text wpnText;
	public Image advIcon;

	[Header("Enemy Character Stats")]
	public GameObject targetStatsObject;
	public Text eCharacterName;
	public Image ePortrait;
	public Text eHpText;

	public Text eWpnText;
	public GameObject eDamageTextObj;
	public Image eAdvIcon;

	[Header("Tooltip")]
	public GameObject tooltipObject;
	public RectTransform tooltipBackground;
	public Text tooltipText;
	public StringVariable tooltipMessage;
	private bool _tooltipActive;
	
	
	private void Start() {
		UpdateUI();
	}

	// Update is called once per frame
	public void UpdateUI() {
		playerStatsObject.SetActive(false);
		targetStatsObject.SetActive(false);

		if (currentTurn.value == Faction.NONE) {
			if (clickCharacter.value != null) {
				ShowPrep(clickCharacter.value);
			}
		}
		else if (selectCharacter != null) {
			switch (currentMode.value)
			{
				case ActionMode.NONE:
				case ActionMode.MOVE:
					ShowNone();
					break;
				case ActionMode.ATTACK:
				case ActionMode.HEAL:
					ShowAttack();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	private void ShowNone() {
		if (selectCharacter.value == null) 
			return;
		
		ShowStats(selectCharacter.value);
		if (attackTile.value != null && attackTile.value.currentCharacter != null)
			ShowStats(attackTile.value.currentCharacter);
	}

	private void ShowAttack() {
		if (attackTile.value != null && attackTile.value.currentCharacter != null) {
			CalculateShowForecast(selectCharacter.value, attackTile.value.currentCharacter);
		}
		else {
			ShowNone();
		}
	}

	private void ShowPrep(StatsContainer stats) {
		if (stats.level == -1)
			return;
		
		fullBackground.SetActive(true);
		attackBackground.SetActive(false);
		playerNormalObject.SetActive(true);
		playerForecastObject.SetActive(false);
		colorBackground.color = new Color(0.2f,0.2f,0.5f);

		characterName.text = stats.charData.charName;	
		portrait.enabled = true;
		portrait.sprite = stats.charData.portrait;
		hpText.color = (stats.bHp != 0) ? Color.green : Color.white;
		atkText.color = (stats.bAtk != 0) ? Color.green : Color.white;
		spdText.color = (stats.bSpd != 0) ? Color.green : Color.white;
		defText.color = (stats.bDef != 0) ? Color.green : Color.white;
		resText.color = (stats.bRes != 0) ? Color.green : Color.white;
		hpText.text = stats.hp.ToString();
		atkText.text = stats.atk.ToString();
		spdText.text = stats.spd.ToString();
		defText.text = stats.def.ToString();
		resText.text = stats.res.ToString();

		levelText.text = "Lv:  " + stats.level;
		for (int i = 0; i < stats.skills.Length; i++) {
			if (stats.skills[i] == null) {
				skillImages[i].enabled = false;
			}
			else {
				skillImages[i].sprite = stats.skills[i].icon;
				skillImages[i].enabled = true;
			}
		}
		
		wpnName.text = (stats.GetItem(ItemCategory.WEAPON) != null) ? stats.GetItem(ItemCategory.WEAPON).itemName : "";

		playerStatsObject.SetActive(true);
	}

	private void ShowStats(TacticsMove tactics) {
		StatsContainer stats = tactics.stats;
		fullBackground.SetActive(true);
		attackBackground.SetActive(false);
		playerNormalObject.SetActive(true);
		playerForecastObject.SetActive(false);
		colorBackground.color = (tactics.faction == Faction.PLAYER) ? 
			new Color(0.2f,0.2f,0.5f) : new Color(0.5f,0.2f,0.2f);

		characterName.text = stats.charData.charName;	
		portrait.enabled = true;
		portrait.sprite = stats.charData.portrait;
		hpText.color = (stats.bHp > 0) ? Color.green : (stats.bHp < 0) ? Color.red : Color.white;
		atkText.color = (stats.bAtk > 0) ? Color.green : (stats.bAtk < 0) ? Color.red : Color.white;
		spdText.color = (stats.bSpd > 0) ? Color.green : (stats.bSpd < 0) ? Color.red : Color.white;
		defText.color = (stats.bDef > 0) ? Color.green : (stats.bDef < 0) ? Color.red : Color.white;
		resText.color = (stats.bRes > 0) ? Color.green : (stats.bRes < 0) ? Color.red : Color.white;
		hpText.text = tactics.currentHealth + " / " + stats.hp;
		atkText.text = stats.atk.ToString();
		spdText.text = stats.spd.ToString();
		defText.text = stats.def.ToString();
		resText.text = stats.res.ToString();

		levelText.text = "Lv:  " + stats.level;
		for (int i = 0; i < stats.skills.Length; i++) {
			if (stats.skills[i] == null) {
				skillImages[i].enabled = false;
			}
			else {
				skillImages[i].sprite = stats.skills[i].icon;
				skillImages[i].enabled = true;
			}
		}

		wpnName.text = (stats.GetItem(ItemCategory.WEAPON) != null) ? stats.GetItem(ItemCategory.WEAPON).itemName : "";
		
		playerStatsObject.SetActive(true);
	}

	private void ShowForecast(TacticsMove lefter, TacticsMove righter, int damage, int taken, int speed, int adv, bool atkWeak, bool defWeak) {
		StatsContainer stats = lefter.stats;
		int currentHp = lefter.currentHealth;
		int receive = (speed <= -5) ? taken * 2 : taken;
		
		fullBackground.SetActive(false);
		attackBackground.SetActive(true);
		playerNormalObject.SetActive(false);
		playerForecastObject.SetActive(true);
		
		characterName.text = stats.charData.charName;
		portrait.enabled = true;
		portrait.sprite = stats.charData.portrait;
		hpText.text = currentHp + " -> " + Mathf.Max(0,(currentHp - Mathf.Max(0,receive)));
		damageTextObj.SetActive(true);
		wpnText.text = (damage < 0) ? "-" : damage.ToString();
		wpnText.color = (defWeak) ? Color.green : Color.white;
		if (speed >= 5)
			wpnText.text += " x2";
		if (adv == 0) {
			advIcon.enabled = false;
		}
		else {
			advIcon.enabled = true;
			advIcon.sprite = (adv == 1) ? advIconGood : advIconBad;
		}
		playerStatsObject.SetActive(true);

		stats = righter.stats;
		currentHp = righter.currentHealth;
		receive = (speed >= 5) ? damage * 2 : damage;
		eCharacterName.text = stats.charData.charName;
		ePortrait.enabled = true;
		ePortrait.sprite = stats.charData.portrait;
		eHpText.text = currentHp + " -> " + Mathf.Max(0,(currentHp - Mathf.Max(0,receive)));
		eDamageTextObj.SetActive(true);
		eWpnText.text = (taken < 0) ? "-" : taken.ToString();
		eWpnText.color = (atkWeak) ? Color.green : Color.white;
		if (speed <= -5)
			eWpnText.text += " x2";
		if (adv == 0) {
			eAdvIcon.enabled = false;
		}
		else {
			eAdvIcon.enabled = true;
			eAdvIcon.sprite = (adv == -1) ? advIconGood : advIconBad;
		}
		targetStatsObject.SetActive(true);
	}

	private void ShowHealForecast(TacticsMove lefter, TacticsMove righter, int heal) {
		StatsContainer stats = lefter.stats;
		int currentHp = lefter.currentHealth;

		fullBackground.SetActive(true);
		attackBackground.SetActive(false);
		playerNormalObject.SetActive(false);
		playerForecastObject.SetActive(true);
		
		characterName.text = stats.charData.charName;
		portrait.enabled = true;
		portrait.sprite = stats.charData.portrait;
		hpText.text = currentHp + " -> " + currentHp;
		damageTextObj.SetActive(false);
		advIcon.enabled = false;
		playerStatsObject.SetActive(true);

		stats = righter.stats;
		currentHp = righter.currentHealth;
		eCharacterName.text = stats.charData.charName;
		ePortrait.enabled = true;
		ePortrait.sprite = stats.charData.portrait;
		eHpText.text = currentHp + " -> " + Mathf.Min(currentHp + heal, stats.hp);
		eDamageTextObj.SetActive(false);
		eAdvIcon.enabled = false;
		targetStatsObject.SetActive(true);
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
			bool atkWeak = attacker.stats.IsWeakAgainst(defender.stats.GetItem(ItemCategory.WEAPON));
			bool defWeak = defender.stats.IsWeakAgainst(attacker.stats.GetItem(ItemCategory.WEAPON));
			ShowForecast(attacker, defender, atk, def, spd, act1.GetAdvantage(), atkWeak, defWeak);
		}
		else {
			ShowHealForecast(attacker, defender, act1.GetHeals());
		}
	}

	public void ShowTooltip(Transform skillTransform, Item item) {
		if (item == null) {
			tooltipObject.SetActive(false);
			return;
		}
		tooltipObject.SetActive(true);
		tooltipText.text = tooltipMessage.value;
		tooltipObject.transform.position = skillTransform.position;
		tooltipBackground.sizeDelta = new Vector2(Mathf.Min(tooltipText.preferredWidth + 16, 216), tooltipText.preferredHeight + 8);
	}

	public void HideTooltip() {
		tooltipObject.SetActive(false);
	}
}
