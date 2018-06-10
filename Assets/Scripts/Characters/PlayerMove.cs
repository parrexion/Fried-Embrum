using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove {

	// Use this for initialization
	protected override void SetupLists() {
		stats = playerChars.values[id];
		playerList.values.Add(this);
		Debug.Log(stats.charName);
	}
	
	protected override void EndMovement() {
		Debug.Log("Finished move");
		isMoving = false;
		TurnController.busy = false;
		characterClicked.Invoke();
		Debug.Log("CURREnt mode:  " + currentMode.value);
		if (currentMode.value == ActionMode.MOVE)
			End();
		else if (attackTarget.value != null && attackTarget.value.currentCharacter != null) {
			if (currentMode.value == ActionMode.ATTACK) {
				Attack(attackTarget.value.currentCharacter);
			}
			else if (currentMode.value == ActionMode.HEAL) {
				Heal(attackTarget.value.currentCharacter);
			}
			else {
				End();
			}
		}
		else {
			End();
		}
	}
}
