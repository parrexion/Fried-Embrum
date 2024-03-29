﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleContainer : InputReceiverDelegate {

	private enum State { INIT, QUOTE, ACTION, DEATH, FINISH }

	public ActionModeVariable currentAction;
	public MapTileVariable attackerTile;
	public MapTileVariable defenderTile;
	public ScrObjEntryReference currentMap;
	public FactionVariable currentTurn;
	public BoolVariable lockControls;
	public FloatVariable cameraPosX;
	public FloatVariable cameraPosY;
	public MySpinner spinner;

	[Header("Dialogue")]
	public IntVariable dialogueMode;
	public ScrObjEntryReference currentDialogue;
	private DialogueEntry dialogue;

	[Header("Settings")]
	public FloatVariable currentGameSpeed;
	public BoolVariable useTrueHit;
	public IntVariable doublingSpeed;

	[Header("Experience")]
	public UIExpMeter expMeter;
	public LevelupScript levelupScript;

	[Header("Battle Actions")]
	public List<BattleAction> actions = new List<BattleAction>();
	public BoolVariable useBattleAnimations;
	private bool showBattleAnim;

	[Header("Battle Animations")]
	public BattleAnimator battleAnimator;
	public GameObject battleAnimationObject;
	public GameObject uiCanvas;

	[Header("Sound")]
	public AudioVariable subMusic;
	public AudioQueueVariable sfxQueue;
	public SfxEntry levelupFanfare;
	public SfxEntry levelupFill;
	public SfxEntry badStuffSfx;
	public SfxEntry rewardSfx;

	[Header("Events")]
	public UnityEvent battleFinishedEvent;
	public UnityEvent showDialogueEvent;
	public UnityEvent playTransitionMusicEvent;
	public UnityEvent stopTransitionMusicEvent;
	public UnityEvent playSfxEvent;
	public UnityEvent stopSfxEvent;

	private State state;
	private TacticsMove _currentCharacter;
	private bool _attackerDealtDamage;
	private bool _defenderDealtDamage;
	private bool waitForAnimation;


	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.BATTLE);
		if (!active)
			return;
		if (state == State.INIT) {
			if (currentAction.value == ActionMode.ATTACK)
				GenerateDamageActions();
			else
				GenerateHealAction();
			PlayBattleAnimations();
		}
		else if (state == State.QUOTE) {
			lockControls.value = true;
			StartCoroutine(ActionLoop());
		}
		else if (state == State.DEATH) {
			lockControls.value = true;
			state = State.FINISH;
		}
	}

	public void GenerateDamageActions() {
		TacticsMove attacker = attackerTile.value.currentCharacter;
		TacticsMove defender = defenderTile.value.currentCharacter;
		dialogue = null;
		if (defender == null) {
			showBattleAnim = false;
			_currentCharacter = attacker;
			actions.Clear();
			actions.Add(new BattleAction(AttackSide.LEFT, BattleAction.Type.DAMAGE, attacker, defenderTile.value.blockMove));
			Debug.Log("BLOCK FIGHT!!");
		}
		else {
			showBattleAnim = useBattleAnimations.value;
			// Add battle init boosts
			attacker.ActivateSkills(SkillActivation.INITCOMBAT, defender);
			attacker.ActivateSkills(SkillActivation.PRECOMBAT, defender);
			defender.ActivateSkills(SkillActivation.COUNTER, attacker);
			defender.ActivateSkills(SkillActivation.PRECOMBAT, attacker);

			_currentCharacter = attacker;
			InventoryTuple atkTup = attacker.GetEquippedWeapon(ItemCategory.WEAPON);
			InventoryTuple defTup = defender.GetEquippedWeapon(ItemCategory.WEAPON);
			actions.Clear();
			actions.Add(new BattleAction(AttackSide.LEFT, BattleAction.Type.DAMAGE, attacker, defender));
			int range = Mathf.Abs(attacker.posx - defender.posx) + Mathf.Abs(attacker.posy - defender.posy);
			if (!string.IsNullOrEmpty(defTup.uuid) && defTup.currentCharges > 0 && defender.GetEquippedWeapon(ItemCategory.WEAPON).InRange(range)) {
				actions.Add(new BattleAction(AttackSide.RIGHT, BattleAction.Type.DAMAGE, defender, attacker));
			}
			//Compare speeds
			int spdDiff = actions[0].GetSpeedDifference();
			if (spdDiff >= doublingSpeed.value) {
				if (atkTup.currentCharges > 1)
					actions.Add(new BattleAction(AttackSide.LEFT, BattleAction.Type.DAMAGE, attacker, defender));
			}
			else if (spdDiff <= -doublingSpeed.value) {
				if (!string.IsNullOrEmpty(defTup.uuid) && defTup.currentCharges > 0 && defender.GetEquippedWeapon(ItemCategory.WEAPON).InRange(range)) {
					actions.Add(new BattleAction(AttackSide.RIGHT, BattleAction.Type.DAMAGE, defender, attacker));
				}
			}

			TacticsMove quoter = (attacker.faction == Faction.ENEMY) ? attacker : defender;
			CharEntry triggerer = (attacker.faction == Faction.ENEMY) ? defender.stats.charData : attacker.stats.charData;
			FightQuote bestFind = null;
			for (int q = 0; q < quoter.fightQuotes.Count; q++) {
				if (quoter.fightQuotes[q].triggerer == null) {
					if (bestFind == null)
						bestFind = quoter.fightQuotes[q];
				}
				else if (quoter.fightQuotes[q].triggerer == triggerer) {
					bestFind = quoter.fightQuotes[q];
					break;
				}
			}
			if (bestFind != null && !bestFind.activated) {
				dialogue = bestFind.quote;
				bestFind.activated = true;
			}
		}
	}

	public void GenerateHealAction() {
		TacticsMove attacker = attackerTile.value.currentCharacter;
		TacticsMove defender = defenderTile.value.currentCharacter;
		if (defender == null) {
			Debug.LogError("There should be someone to heal, right!?");
		}
		else {
			_currentCharacter = attacker;
			showBattleAnim = useBattleAnimations.value;
			actions.Clear();
			actions.Add(new BattleAction(AttackSide.LEFT, BattleAction.Type.HEAL, attacker, defender));
		}
	}

	public void PlayBattleAnimations() {
		SetupBattle();
		if (dialogue != null) {
			uiCanvas.gameObject.SetActive(!showBattleAnim);
			uiCanvas.gameObject.SetActive(false);
			state = State.QUOTE;
			dialogueMode.value = (int)DialogueMode.QUOTE;
			currentDialogue.value = dialogue;
			showDialogueEvent.Invoke();
		}
		else {
			StartCoroutine(ActionLoop());
		}
	}

	private void SetupBattle() {
		battleAnimator.SetupScene(actions[0].attacker.GetComponent<SpriteRenderer>().sprite, actions[0].defender.GetComponent<SpriteRenderer>().sprite);
		_attackerDealtDamage = false;
		_defenderDealtDamage = false;

		battleAnimationObject.transform.localPosition = new Vector3(
			cameraPosX.value,
			cameraPosY.value,
			battleAnimationObject.transform.localPosition.z
		);
		TacticsCamera tacticsCamera = GameObject.FindObjectOfType<TacticsCamera>();
		battleAnimationObject.transform.SetParent(tacticsCamera.transform);
		battleAnimationObject.transform.localPosition = new Vector3(0, 0, 5);
		battleAnimationObject.SetActive(showBattleAnim);
		uiCanvas.gameObject.SetActive(!showBattleAnim);
		uiCanvas.gameObject.SetActive(false);

		//Music
		MapEntry map = (MapEntry)currentMap.value;
		subMusic.value = (actions[0].type == BattleAction.Type.DAMAGE) ? map.mapMusic.battleTheme.clip : map.mapMusic.healTheme.clip;
		playTransitionMusicEvent.Invoke();
	}

	private IEnumerator ActionLoop() {
		state = State.ACTION;

		for (int i = 0; i < actions.Count; i++) {
			BattleAction act = actions[i];
			if (act.type == BattleAction.Type.DAMAGE && act.attacker.inventory.GetFirstUsableItemTuple(ItemCategory.WEAPON).currentCharges <= 0) {
				continue; //Broken weapon
			}
			if (act.type != BattleAction.Type.DAMAGE && act.attacker.inventory.GetFirstUsableItemTuple(ItemCategory.SUPPORT).currentCharges <= 0) {
				continue; //Broken staff
			}

			yield return new WaitForSeconds(1f * currentGameSpeed.value);

			BattleAnimator.AnimationInfo animationInfo = new BattleAnimator.AnimationInfo();

			// Deal damage
			bool isCrit = false;
			if (act.type == BattleAction.Type.DAMAGE) {
				int damage = act.AttemptAttack(useTrueHit.value);
				if (damage != -1 && act.AttemptCrit()) {
					damage = (int)(damage * BattleCalc.CRIT_MODIFIER);
					isCrit = true;
				}
				act.defender.TakeDamage(damage, isCrit);
				BattleAnimator.HitType hitType = (damage < 0) ? BattleAnimator.HitType.MISS : (isCrit) ? BattleAnimator.HitType.CRIT : BattleAnimator.HitType.NORMAL;
				animationInfo.side = act.side;
				animationInfo.weaponType = act.weaponAtk.weaponType;
				animationInfo.hitType = hitType;
				animationInfo.leathal = !act.defender.IsAlive();
				animationInfo.damage = damage;

				if (damage > 0) {
					if (act.side == AttackSide.LEFT)
						_attackerDealtDamage = true;
					else
						_defenderDealtDamage = true;
				}
				
				act.attacker.inventory.ReduceItemCharge(ItemCategory.WEAPON);
			}
			else {
				// Heal or buff
				if (act.staffAtk.weaponType == WeaponType.MEDKIT) {
					int health = act.GetHeals();
					act.defender.TakeHeals(health);
					//StartCoroutine(DamageDisplay(act.leftSide, health, false, false));
				}
				else if (act.staffAtk.weaponType == WeaponType.BARRIER) {
					act.defender.ReceiveBuff(act.attacker.GetEquippedWeapon(ItemCategory.SUPPORT).boost, true, true);
				}
				act.attacker.inventory.ReduceItemCharge(ItemCategory.SUPPORT);
				_attackerDealtDamage = true;
			}

			//Animate!!
			waitForAnimation = true;
			battleAnimator.PlayAttack(animationInfo);
			while (waitForAnimation) {
				yield return null;
			}
			
			//Check Death
			// Debug.Log("Check death");
			if (!act.defender.IsAlive()) {
				yield return new WaitForSeconds(1f * currentGameSpeed.value);
				if (act.defender.stats.charData.deathQuote != null) {
					Debug.Log("Death quote");
					state = State.DEATH;
					dialogueMode.value = (int)DialogueMode.QUOTE;
					currentDialogue.value = dialogue = act.defender.stats.charData.deathQuote;
					showDialogueEvent.Invoke();
					lockControls.value = false;

					while (state == State.DEATH) {
						yield return null;
					}
				}
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
		if (actions[0].type == BattleAction.Type.DAMAGE) {
			actions[0].attacker.ActivateSkills(SkillActivation.POSTCOMBAT, actions[0].defender);
			actions[0].defender.ActivateSkills(SkillActivation.POSTCOMBAT, actions[0].attacker);
			actions[0].defender.stats.fatigueAmount = Mathf.Min(actions[0].defender.fatigueCap, actions[0].defender.stats.fatigueAmount + 1);
		}

		//Clean up
		state = State.INIT;
		currentAction.value = ActionMode.NONE;
		battleAnimator.CleanupScene();
		battleAnimationObject.SetActive(false);
		uiCanvas.SetActive(true);
		actions[0].attacker.EndSkills(SkillActivation.INITCOMBAT, actions[0].defender);
		actions[0].attacker.EndSkills(SkillActivation.PRECOMBAT, actions[0].defender);
		actions[0].defender.EndSkills(SkillActivation.PRECOMBAT, actions[0].attacker);
		actions.Clear();
		if (currentTurn.value == Faction.PLAYER)
			lockControls.value = false;
		_currentCharacter.End();
		_currentCharacter = null;

		yield return new WaitForSeconds(0.5f * currentGameSpeed.value);

		//Music
		stopTransitionMusicEvent.Invoke();

		//Send game finished
		battleFinishedEvent.Invoke();
	}

	private IEnumerator ShowExpGain() {
		TacticsMove player = null;
		for (int i = 0; i < actions.Count; i++) {
			if (actions[i].attacker.faction == Faction.PLAYER && actions[i].defender.faction != Faction.WORLD) {
				if ((actions[i].side == AttackSide.LEFT && _attackerDealtDamage) || (actions[i].side == AttackSide.RIGHT && _defenderDealtDamage)) {
					player = actions[i].attacker;
					break;
				}
			}
		}

		if (player == null) {
			//Debug.Log("Nothing to give exp for");
			yield return new WaitForSeconds(0.5f * currentGameSpeed.value);
			yield break;
		}

		int exp = actions[0].GetExperience();
		exp = player.EditValueSkills(SkillActivation.REWARD, exp);
		if (exp > 0) {
			expMeter.gameObject.SetActive(true);
			expMeter.currentExp = player.stats.currentExp;
			yield return new WaitForSeconds(0.5f * currentGameSpeed.value);
			sfxQueue.Enqueue(levelupFill);
			playSfxEvent.Invoke();
			while (exp > 0) {
				exp--;
				expMeter.currentExp++;
				if (expMeter.currentExp == 100) {
					expMeter.currentExp = 0;
					stopSfxEvent.Invoke();
					yield return new WaitForSeconds(1f * currentGameSpeed.value);
					expMeter.gameObject.SetActive(false);
					levelupScript.SetupStats(player.stats, true);
					player.stats.GainLevel();
					sfxQueue.Enqueue(levelupFanfare);
					playSfxEvent.Invoke();
					yield return StartCoroutine(levelupScript.RunLevelup(player.stats));
					expMeter.gameObject.SetActive(true);
					sfxQueue.Enqueue(levelupFill);
					playSfxEvent.Invoke();
				}
				yield return null;
			}
			stopSfxEvent.Invoke();
			yield return new WaitForSeconds(0.5f * currentGameSpeed.value);
			expMeter.gameObject.SetActive(false);
			player.stats.currentExp = expMeter.currentExp;
		}
	}

	private IEnumerator HandleBrokenWeapons() {
		//Broken weapons
		if (actions[0].type == BattleAction.Type.DAMAGE) {
			InventoryTuple invTup = actions[0].weaponAtk;
			if (!string.IsNullOrEmpty(invTup.uuid) && invTup.currentCharges <= 0) {
				yield return StartCoroutine(spinner.ShowSpinner(invTup.icon, invTup.entryName + " is out of ammo!", badStuffSfx));
				actions[0].attacker.inventory.CleanupInventory();
			}
			invTup = actions[0].weaponDef;
			if (!string.IsNullOrEmpty(invTup.uuid) && invTup.currentCharges <= 0) {
				yield return StartCoroutine(spinner.ShowSpinner(invTup.icon, invTup.entryName + " is out of ammo!", badStuffSfx));
				actions[0].defender.inventory.CleanupInventory();
			}
		}
		else {
			InventoryTuple invTup = actions[0].staffAtk;
			if (!string.IsNullOrEmpty(invTup.uuid) && invTup.currentCharges <= 0) {
				yield return StartCoroutine(spinner.ShowSpinner(invTup.icon, invTup.entryName + " is out of ammo!", badStuffSfx));
				actions[0].attacker.inventory.CleanupInventory();
			}
		}
	}

	private IEnumerator DropItems(TacticsMove dropper, TacticsMove receiver) {
		for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
			InventoryTuple itemTup = dropper.inventory.GetTuple(i);
			if (itemTup == null || !itemTup.droppable)
				continue;

			Debug.Log("Dropped item:  " + itemTup.entryName);
			itemTup.droppable = false;
			receiver.inventory.AddItem(itemTup);

			yield return StartCoroutine(spinner.ShowSpinner(itemTup.icon, "Gained " + itemTup.entryName, rewardSfx));
		}
		yield break;
	}

	public void AnimationFinished() {
		waitForAnimation = false;
	}

	public override void OnOkButton() {
		levelupScript.Continue();
		menuAcceptEvent.Invoke();
	}


	public override void OnUpArrow() { }
	public override void OnDownArrow() { }
	public override void OnLeftArrow() { }
	public override void OnRightArrow() { }
	public override void OnBackButton() { }
	public override void OnLButton() { }
	public override void OnRButton() { }
	public override void OnXButton() { }
	public override void OnYButton() { }
	public override void OnStartButton() { }
}
