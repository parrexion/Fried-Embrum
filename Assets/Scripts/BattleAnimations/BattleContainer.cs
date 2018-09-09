using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleContainer : MonoBehaviour {

	public static BattleContainer instance;

	private void Awake() {
		instance = this;
	}

	public ScrObjEntryReference currentMap;
	public FactionVariable currentTurn;
	public BoolVariable lockControls;
	public IntVariable cursorX;
	public IntVariable cursorY;

	[Header("Settings")]
	public BoolVariable useTrueHit;
	public IntVariable doublingSpeed;

	[Header("Experience")]
	public UIExpMeter expMeter;
	public LevelupScript levelupScript;
	
	[Header("Battle Actions")]
	public List<BattleAction> actions = new List<BattleAction>();
	public float speed = 1.5f;
	public BoolVariable useBattleAnimations;

	[Header("Battle Animations")]
	public GameObject battleAnimationObject;
	public GameObject uiCanvas;
	public ForecastUI forecastUI;
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

	[Header("Broken Tooltip")]
	public GameObject brokenTooltip;
	public Image brokenIcon;
	public Text brokenText;

	[Header("Sound")]
	public AudioVariable subMusic;
	public AudioQueueVariable sfxQueue;
	public BoolVariable musicFocus;
	public SfxEntry levelupFanfare;
	public SfxEntry levelupFill;
	public SfxEntry brokenItemFanfare;
	public SfxEntry droppedItemFanfare;
	public SfxEntry missedAttackSfx;
	public SfxEntry hitAttackSfx;
	public SfxEntry leathalAttackSfx;
	public SfxEntry critAttackSfx;
	public SfxEntry enemyDiedSfx;
	public SfxEntry healSfx;
	
	[Header("Events")]
	public UnityEvent updateHealthEvent;
	public UnityEvent battleFinishedEvent;
	public UnityEvent showDialogueEvent;
	public UnityEvent playSubMusicEvent;
	public UnityEvent playSfxEvent;
	public UnityEvent stopSfxEvent;
	
	private TacticsMove _currentCharacter;
	private bool _attackerDealtDamage;
	private bool _defenderDealtDamage;


	public void GenerateActions(TacticsMove attacker, MapTile target) {
		if (target.currentCharacter == null) {
			_currentCharacter = attacker;
			actions.Clear();
			actions.Add(new BattleAction(true, true, attacker, target.blockMove));
			Debug.Log("BLOCK FIGHT!!");
		}
		else {
			TacticsMove defender = target.currentCharacter;

			// Add battle init boosts
			attacker.ActivateSkills(Activation.INITCOMBAT, defender);
			attacker.ActivateSkills(Activation.PRECOMBAT, defender);
			defender.ActivateSkills(Activation.PRECOMBAT, attacker);
			
			_currentCharacter = attacker;
			actions.Clear();
			actions.Add(new BattleAction(true, true, attacker, defender));
			int range = Mathf.Abs(attacker.posx - defender.posx) + Mathf.Abs(attacker.posy - defender.posy);
			if (defender.GetEquippedWeapon(ItemCategory.WEAPON) != null && defender.GetEquippedWeapon(ItemCategory.WEAPON).InRange(range)) {
				actions.Add(new BattleAction(false, true, defender, attacker));
			}
			//Compare speeds
			int spdDiff = actions[0].GetSpeedDifference();
			if (spdDiff >= doublingSpeed.value) {
				actions.Add(new BattleAction(true, true, attacker, defender));
			}
			else if (spdDiff <= - doublingSpeed.value) {
				if (defender.GetEquippedWeapon(ItemCategory.WEAPON) != null && defender.GetEquippedWeapon(ItemCategory.WEAPON).InRange(range)) {
					actions.Add(new BattleAction(false, true, defender, attacker));
				}
			}
		}
	}

	public void GenerateHealAction(TacticsMove attacker, MapTile target) {
		if (target.currentCharacter == null) {

		}
		else {
			TacticsMove defender = target.currentCharacter;
			_currentCharacter = attacker;
			actions.Clear();
			actions.Add(new BattleAction(true, false, attacker, defender));
		}
	}

	public void PlayBattleAnimations() {
		SetupBattle();
		StartCoroutine(ActionLoop());
	}

	private void SetupBattle() {
		leftDamageObject.SetActive(false);
		rightDamageObject.SetActive(false);
		leftTransform.GetComponent<SpriteRenderer>().sprite = actions[0].attacker.stats.charData.battleSprite;
		rightTransform.GetComponent<SpriteRenderer>().sprite = actions[0].defender.stats.charData.battleSprite;
		leftTransform.GetComponent<SpriteRenderer>().color = Color.white;
		rightTransform.GetComponent<SpriteRenderer>().color = Color.white;
		_attackerDealtDamage = false;
		_defenderDealtDamage = false;

		//Music
		MapEntry map = (MapEntry)currentMap.value;
		subMusic.value = (actions[0].isDamage) ? map.battleMusic.clip : map.healMusic.clip;
		musicFocus.value = false;
		playSubMusicEvent.Invoke();
	}

	private IEnumerator ActionLoop() {
		bool useAnim = useBattleAnimations.value;
		if (actions[0].defender.faction == Faction.WORLD) {
			useAnim = false;
		}
		for (int i = 0; i < actions.Count; i++) {
			BattleAction act = actions[i];
			if (act.isDamage && act.attacker.GetFirstUsableInventoryTuple(ItemCategory.WEAPON).charge <= 0) {
				continue; //Broken weapon
			}
			if (!act.isDamage && act.attacker.GetFirstUsableInventoryTuple(ItemCategory.STAFF).charge <= 0) {
				continue; //Broken staff
			}
			
			Transform attackTransform = (!useAnim) ? act.attacker.transform : (act.leftSide) ? leftTransform : rightTransform;
			Transform defenseTransform = (!useAnim) ? act.defender.transform : (act.leftSide) ? rightTransform : leftTransform;
			Vector3 startPos = attackTransform.position;
			Vector3 enemyPos = defenseTransform.position;
			enemyPos = startPos + (enemyPos - startPos).normalized;
			
			battleAnimationObject.SetActive(useAnim);
			// uiCanvas.SetActive(!useAnim.value);
			forecastUI.UpdateUI(true);
			if (useAnim) {
				leftHealth.fillAmount = actions[0].attacker.GetHealthPercent();
				rightHealth.fillAmount = actions[0].defender.GetHealthPercent();
			}
			
			yield return new WaitForSeconds(1f);
			
			//Move forward
			float f = 0;
			// Debug.Log("Start moving");
			while(f < 0.5f) {
				f += Time.deltaTime * speed;
				attackTransform.position = Vector3.Lerp(startPos, enemyPos, f);
				yield return null;
			}
			// Deal damage
			bool isCrit = false;
			if (act.isDamage) {
				int damage = (GenerateHitNumber(act.GetHitRate())) ? act.GetDamage() : -1;
				if (damage != -1 && SingleNumberCheck(act.GetCritRate())) {
					damage *= 3;
					isCrit = true;
				}
				act.defender.TakeDamage(damage, isCrit);
				if (damage < 0) {
					sfxQueue.Enqueue(missedAttackSfx);
					playSfxEvent.Invoke();
				}
				else {
					SfxEntry hitSfx = (isCrit) ? critAttackSfx : 
									(!act.defender.IsAlive()) ? leathalAttackSfx : hitAttackSfx;
					sfxQueue.Enqueue(hitSfx);
					playSfxEvent.Invoke();
				}
				StartCoroutine(DamageDisplay(act.leftSide, damage, true, isCrit));
				
				if (damage > 0) {
					if (act.leftSide)
						_attackerDealtDamage = true;
					else
						_defenderDealtDamage = true;
				}
				
				if (!act.defender.IsAlive()) {
					if (act.leftSide)
						rightTransform.GetComponent<SpriteRenderer>().color = new Color(0.4f,0.4f,0.4f);
					else
						leftTransform.GetComponent<SpriteRenderer>().color = new Color(0.4f,0.4f,0.4f);
				}
				act.attacker.ReduceWeaponCharge(ItemCategory.WEAPON);
				act.attacker.stats.GiveWpnExp(act.attacker.GetEquippedWeapon(ItemCategory.WEAPON));
			}
			else {
				if (act.staffAtk.item.itemType == ItemType.HEAL) {
					int health = act.GetHeals();
					act.defender.TakeHeals(health);
					StartCoroutine(DamageDisplay(act.leftSide, health, false, false));
				}
				else if (act.staffAtk.item.itemType == ItemType.BUFF) {
					act.defender.ReceiveBuff(act.attacker.GetEquippedWeapon(ItemCategory.STAFF).boost, true, true);
				}
				act.attacker.ReduceWeaponCharge(ItemCategory.STAFF);
				act.attacker.stats.GiveWpnExp(act.attacker.GetEquippedWeapon(ItemCategory.STAFF));
				_attackerDealtDamage = true;
			}
			//Update health
			leftHealth.fillAmount = (act.leftSide) ? act.attacker.GetHealthPercent() : act.defender.GetHealthPercent();
			rightHealth.fillAmount = (act.leftSide) ? act.defender.GetHealthPercent() : act.attacker.GetHealthPercent();
			updateHealthEvent.Invoke();
			
			//Extra crit animation
			if (isCrit) {
				defenseTransform.GetComponent<ParticleSystem>().Play();
				yield return new WaitForSeconds(0.2f);
			}
			
			// Move back
			// Debug.Log("Moving back");
			while(f > 0f) {
				f -= Time.deltaTime * speed;
				attackTransform.position = Vector3.Lerp(startPos, enemyPos, f);
				yield return null;
			}

			//Check Death
			// Debug.Log("Check death");
			if (!act.defender.IsAlive()) {
				yield return new WaitForSeconds(1f);
				break;
			}
		}

		//Handle exp
		yield return StartCoroutine(ShowExpGain());
		
		//Broken weapons
		yield return StartCoroutine(HandleBrokenWeapons());
		
		//Drop Items
		if (!actions[0].attacker.IsAlive() && actions[0].attacker.faction == Faction.ENEMY) {
			yield return StartCoroutine(DropItems(actions[0].attacker, actions[0].defender));
		}
		else if (!actions[0].defender.IsAlive() && actions[0].defender.faction == Faction.ENEMY) {
			yield return StartCoroutine(DropItems(actions[0].defender, actions[0].attacker));
		}

		//Give debuffs
		if (actions[0].isDamage) {
			actions[0].attacker.ActivateSkills(Activation.POSTCOMBAT, actions[0].defender);
			actions[0].defender.ActivateSkills(Activation.POSTCOMBAT, actions[0].attacker);
		}

		//Clean up
		battleAnimationObject.SetActive(false);
		// uiCanvas.SetActive(true);
		leftDamageObject.SetActive(false);
		rightDamageObject.SetActive(false);
		actions[0].attacker.EndSkills(Activation.INITCOMBAT, actions[0].defender);
		actions[0].attacker.EndSkills(Activation.PRECOMBAT, actions[0].defender);
		actions[0].defender.EndSkills(Activation.PRECOMBAT, actions[0].attacker);
		actions.Clear();
		if (currentTurn.value == Faction.PLAYER)
			lockControls.value = false;
		_currentCharacter.End();
		cursorX.value = _currentCharacter.posx;
		cursorY.value = _currentCharacter.posy;
		_currentCharacter = null;
		
		yield return new WaitForSeconds(0.5f);

		//Music
		subMusic.value = null;
		musicFocus.value = true;
		playSubMusicEvent.Invoke();

		//Send game finished
		battleFinishedEvent.Invoke();
	}

	private IEnumerator DamageDisplay(bool leftSide, int damage, bool isDamage, bool isCrit) {
		GameObject damageObject = (leftSide) ? rightDamageObject : leftDamageObject;
		Text damageText = (leftSide) ? rightDamageText : leftDamageText;
		damageText.color = (isDamage) ? Color.black : new Color(0,0.5f,0);
		damageText.text = (damage != -1) ? damage.ToString() : "Miss";
		damageObject.transform.localScale = (isCrit) ? new Vector3(2, 2, 2) : new Vector3(1, 1, 1);
		damageObject.gameObject.SetActive(true);
		
		yield return new WaitForSeconds(1f);
		damageObject.gameObject.SetActive(false);
	}

	private IEnumerator ShowExpGain() {
		TacticsMove player = null;
		for (int i = 0; i < actions.Count; i++) {
			if (actions[i].attacker.faction == Faction.PLAYER && actions[i].defender.faction != Faction.WORLD) {
				if ((actions[i].leftSide && _attackerDealtDamage) || (!actions[i].leftSide && _defenderDealtDamage)) {
					player = actions[i].attacker;
					break;
				}
			}
		}
		
		if (player == null) {
			Debug.Log("Nothing to give exp for");
			yield return new WaitForSeconds(0.5f);
			yield break;
		}

		int exp = actions[0].GetExperience();
		exp = player.EditValueSkills(Activation.EXP, exp);
		if (exp > 0) {
			expMeter.gameObject.SetActive(true);
			expMeter.currentExp = player.stats.currentExp;
			Debug.Log("Exp is currently: " + player.stats.currentExp);
			yield return new WaitForSeconds(0.5f);
			sfxQueue.Enqueue(levelupFill);
			playSfxEvent.Invoke();
			while(exp > 0) {
				exp--;
				expMeter.currentExp++;
				if (expMeter.currentExp == 100) {
					expMeter.currentExp = 0;
					stopSfxEvent.Invoke();
					yield return new WaitForSeconds(1f);
					expMeter.gameObject.SetActive(false);
					levelupScript.SetupStats(player.stats.level,player.stats);
					Debug.Log("LEVELUP!");
					sfxQueue.value.Enqueue(levelupFanfare.clip);
					playSfxEvent.Invoke();
					yield return StartCoroutine(levelupScript.RunLevelup(player.stats));
					CharacterSkill skill = player.stats.classData.AwardSkills(player.stats.level);
					if (skill) {
						player.skills.GainSkill(skill);
						yield return StartCoroutine(ShowPopup(skill.icon,  "gained: " + skill.entryName, droppedItemFanfare));
					}
					expMeter.gameObject.SetActive(true);
					sfxQueue.Enqueue(levelupFill);
					playSfxEvent.Invoke();
				}
				yield return null;
			}
			stopSfxEvent.Invoke();
			yield return new WaitForSeconds(0.5f);
			expMeter.gameObject.SetActive(false);
			player.stats.currentExp = expMeter.currentExp;
			Debug.Log("Exp is now: " + player.stats.currentExp);
		}
	}

	private IEnumerator HandleBrokenWeapons() {
		//Broken weapons
		if (actions[0].isDamage) {
			InventoryTuple invTup = actions[0].weaponAtk;
			if (invTup.item != null && invTup.charge <= 0) {
				yield return StartCoroutine(ShowPopup(invTup.item.icon, invTup.item.entryName + " broke!", brokenItemFanfare));
				actions[0].attacker.inventory.CleanupInventory();
			}
			invTup = actions[0].weaponDef;
			if (invTup != null && invTup.charge <= 0) {
				yield return StartCoroutine(ShowPopup(invTup.item.icon, invTup.item.entryName + " broke!", brokenItemFanfare));
				actions[0].defender.inventory.CleanupInventory();
			}
		}
		else {
			InventoryTuple invTup = actions[0].staffAtk;
			if (invTup != null && invTup.charge <= 0) {
				yield return StartCoroutine(ShowPopup(invTup.item.icon, invTup.item.entryName + " broke!", brokenItemFanfare));
				actions[0].attacker.inventory.CleanupInventory();
			}
		}
	}

	private IEnumerator DropItems(TacticsMove dropper, TacticsMove receiver) {
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple itemTup = dropper.inventory.GetItem(i);
			if (itemTup == null || !itemTup.droppable)
				continue;
			
			Debug.Log("Dropped item:  " + itemTup.item.entryName);
			itemTup.droppable = false;
			receiver.inventory.GainItem(itemTup);

			yield return StartCoroutine(ShowPopup(itemTup.item.icon, "Gained " + itemTup.item.entryName, droppedItemFanfare));
		}
		yield break;
	}

	private IEnumerator ShowPopup(Sprite icon, string text, SfxEntry sfx) {
		brokenIcon.sprite = icon;
		brokenText.text = text;
		brokenTooltip.SetActive(true);
		if (sfx != null) {
			sfxQueue.Enqueue(sfx);
			playSfxEvent.Invoke();
		}
		yield return new WaitForSeconds(2f);
		brokenTooltip.SetActive(false);
		yield return new WaitForSeconds(0.5f);
	}

	private bool GenerateHitNumber(int hit) {
		int nr = Random.Range(0, 100);
		if (useTrueHit.value) {
			nr += Random.Range(0, 100);
			nr /= 2;
		}
		// Debug.Log("HIT:  " + nr + " -> " + hit);
		return (nr < hit);
	}

	private bool SingleNumberCheck(int hit) {
		int nr = Random.Range(0, 100);
		// Debug.Log("SINGLE:  " + nr + " -> " + hit);
		return (nr < hit);
	}
}
