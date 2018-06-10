using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleContainer : MonoBehaviour {

	public static BattleContainer instance;

	private void Awake() {
		instance = this;
	}

	public UIExpMeter expMeter;
	public LevelupScript levelupScript;
	public List<BattleAction> actions = new List<BattleAction>();
	public float speed = 1.5f;
	public BoolVariable useBattleAnimations;

	[Header("Battle Animations")]
	public GameObject battleAnimationObject;
	[Space(5)]
	public Transform leftTransform;
	public Image leftHealth;
	public GameObject leftDamageObject;
	public Text leftDamageText;
	[Space(5)]
	public Transform rightTransform;
	public Image rightHealth;
	public GameObject rightDamageObject;
	public Text rightDamageText;

	public UnityEvent battleFinishedEvent;
	
	private TacticsMove _currentCharacter;


	public void GenerateActions(TacticsMove attacker, TacticsMove defender) {
		// Add battle init boosts
		attacker.ActivateSkills(Activation.INITCOMBAT, defender);
		attacker.ActivateSkills(Activation.PRECOMBAT, defender);
		defender.ActivateSkills(Activation.PRECOMBAT, attacker);
		
		_currentCharacter = attacker;
		actions.Clear();
		actions.Add(new BattleAction(true, true, attacker, defender));
		int range = Mathf.Abs(attacker.posx - defender.posx) + Mathf.Abs(attacker.posy - defender.posy);
		if (defender.GetWeapon() != null && defender.GetWeapon().InRange(range)) {
			actions.Add(new BattleAction(false, true, defender, attacker));
		}
		//Compare speeds
		int spdDiff = attacker.stats.spd - defender.stats.spd;
		if (spdDiff >= 5){
			actions.Add(new BattleAction(true, true, attacker, defender));
		}
		else if (spdDiff <= -5) {
			if (defender.GetWeapon() != null && defender.GetWeapon().InRange(range)) {
				actions.Add(new BattleAction(false, true, defender, attacker));
			}
		}
	}

	public void GenerateHealAction(TacticsMove attacker, TacticsMove defender) {
		_currentCharacter = attacker;
		actions.Clear();
		actions.Add(new BattleAction(true, false, attacker, defender));
	}

	public void PlayBattleAnimations() {
		StartCoroutine(ActionLoop());
	}

	private IEnumerator ActionLoop() {
		leftDamageObject.SetActive(false);
		rightDamageObject.SetActive(false);
		leftTransform.GetComponent<SpriteRenderer>().sprite = actions[0].attacker.stats.battleSprite;
		rightTransform.GetComponent<SpriteRenderer>().sprite = actions[0].defender.stats.battleSprite;
		leftTransform.GetComponent<SpriteRenderer>().color = Color.white;
		rightTransform.GetComponent<SpriteRenderer>().color = Color.white;
		
		for (int i = 0; i < actions.Count; i++) {
			BattleAction act = actions[i];
			Transform attackTransform = (!useBattleAnimations.value) ? act.attacker.transform : (act.leftSide) ? leftTransform : rightTransform;
			Transform defenseTransform = (!useBattleAnimations.value) ? act.defender.transform : (act.leftSide) ? rightTransform : leftTransform;
			Vector3 startPos = attackTransform.localPosition;
			Vector3 enemyPos = defenseTransform.localPosition;
			enemyPos = startPos + (enemyPos - startPos).normalized;
			
			battleAnimationObject.SetActive(useBattleAnimations.value);
			if (useBattleAnimations.value) {
				leftHealth.fillAmount = actions[0].attacker.GetHealthPercent();
				rightHealth.fillAmount = actions[0].defender.GetHealthPercent();
				yield return new WaitForSeconds(1f);
			}
			
			//Move forward
			float f = 0;
			Debug.Log("Start moving");
			while(f < 0.5f) {
				f += Time.deltaTime * speed;
				attackTransform.localPosition = Vector3.Lerp(startPos, enemyPos, f);
				yield return null;
			}
			// Deal damage
			if (act.isDamage) {
				int damage = act.GetDamage();
				if (act.attacker.SkillReady(SkillType.DAMAGE)) {
					damage = act.attacker.GetSkill().GenerateDamage(damage);
					act.attacker.skillCharge = -1;
					Debug.Log("Bonus damage!");
				}
				act.defender.TakeDamage(damage);
				StartCoroutine(DamageDisplay(act.leftSide, damage, true));
				
				Debug.Log(i + " Dealt damage :  " + damage);
				if (act.attacker.SkillReady(SkillType.HEAL)) {
					int health = act.attacker.GetSkill().GenerateHeal(damage);
					act.attacker.skillCharge = -1;
					act.attacker.TakeHeals(health);
					rightDamageText.text = damage.ToString();
					StartCoroutine(DamageDisplay(!act.leftSide, damage, false));
					Debug.Log(i + " Healt damage :  " + health);
				}
				
				if (!act.defender.IsAlive()) {
					if (act.leftSide)
						rightTransform.GetComponent<SpriteRenderer>().color = new Color(0.4f,0.4f,0.4f);
					else
						leftTransform.GetComponent<SpriteRenderer>().color = new Color(0.4f,0.4f,0.4f);
				}
			
				//Add skill charges
				act.attacker.IncreaseSkill();
				act.defender.IncreaseSkill();
			}
			else {
				if (act.attacker.GetSupport().supportType == SupportType.HEAL) {
					int health = act.GetHeals();
					act.defender.TakeHeals(health);
					StartCoroutine(DamageDisplay(act.leftSide, health, false));
					Debug.Log(i + " Healt damage :  " + health);
					if (act.attacker.faction == Faction.PLAYER)
						act.attacker.GainSP(1, true);
				}
				else if (act.attacker.GetSupport().supportType == SupportType.BUFF) {
					act.defender.ReceiveBuff(act.attacker.GetSupport().boost, true, true);
					Debug.Log("Boost them up!");
				}
			}
			//Update health
			leftHealth.fillAmount = (act.leftSide) ? act.attacker.GetHealthPercent() : act.defender.GetHealthPercent();
			rightHealth.fillAmount = (act.leftSide) ? act.defender.GetHealthPercent() : act.attacker.GetHealthPercent();

			// Move back
			Debug.Log("Moving back");
			while(f > 0f) {
				f -= Time.deltaTime * speed;
				attackTransform.localPosition = Vector3.Lerp(startPos, enemyPos, f);
				yield return null;
			}

			//Check Death
			Debug.Log("Check death");
			if (!act.defender.IsAlive()) {
				if (act.attacker.faction == Faction.PLAYER)
					act.attacker.GainSP(3, true);
				yield return new WaitForSeconds(1f);
				break;
			}
		}

		//Handle exp
		yield return StartCoroutine(ShowExpGain());
		
		//Give debuffs
		if (actions[0].isDamage) {
			actions[0].attacker.ActivateSkills(Activation.POSTCOMBAT, actions[0].defender);
			actions[0].defender.ActivateSkills(Activation.POSTCOMBAT, actions[0].attacker);
		}
		
		//Check game finished
		battleFinishedEvent.Invoke();

		//Clean up
		battleAnimationObject.SetActive(false);
		leftDamageObject.SetActive(false);
		rightDamageObject.SetActive(false);
		actions[0].attacker.EndSkills(Activation.INITCOMBAT, actions[0].defender);
		actions[0].attacker.EndSkills(Activation.PRECOMBAT, actions[0].defender);
		actions[0].defender.EndSkills(Activation.PRECOMBAT, actions[0].attacker);
		actions.Clear();
		TurnController.busy = false;
		_currentCharacter.End();
		_currentCharacter = null;
	}

	private IEnumerator DamageDisplay(bool leftSide, int damage, bool isDamage) {
		GameObject damageObject = (leftSide) ? rightDamageObject : leftDamageObject;
		Text damageText = (leftSide) ? rightDamageText : leftDamageText;
		damageText.color = (isDamage) ? Color.black : Color.white;
		damageText.text = damage.ToString();
		damageObject.gameObject.SetActive(true);
		
		yield return new WaitForSeconds(1f);
		damageObject.gameObject.SetActive(false);
	}

	private IEnumerator ShowExpGain() {
		TacticsMove player = null;
		for (int i = 0; i < actions.Count; i++) {
			if (actions[i].attacker.faction == Faction.PLAYER) {
				player = actions[i].attacker;
				break;
			}
		}
		if (player == null) {
			Debug.Log("Nothing to give exp for");
			yield break;
		}

		int exp = actions[0].GetExperience();
		exp = player.EditValueSkills(Activation.EXP, exp);
		if (exp > 0) {
			expMeter.gameObject.SetActive(true);
			expMeter.currentExp = player.stats.currentExp;
			Debug.Log("Exp is currently: " + player.stats.currentExp);
			yield return new WaitForSeconds(0.5f);
			while(exp > 0) {
				exp--;
				expMeter.currentExp++;
				if (expMeter.currentExp == 100) {
					expMeter.currentExp = 0;
					yield return new WaitForSeconds(1f);
					expMeter.gameObject.SetActive(false);
					levelupScript.SetupStats(player.stats.level,player.stats);
					levelupScript.gameObject.SetActive(true);
					Debug.Log("LEVELUP!");
					yield return StartCoroutine(levelupScript.RunLevelup(player.stats));
					levelupScript.gameObject.SetActive(false);
					expMeter.gameObject.SetActive(true);
				}
				yield return null;
			}
			yield return new WaitForSeconds(0.5f);
			expMeter.gameObject.SetActive(false);
			player.stats.currentExp = expMeter.currentExp;
			Debug.Log("Exp is now: " + player.stats.currentExp);
		}
	}
}
