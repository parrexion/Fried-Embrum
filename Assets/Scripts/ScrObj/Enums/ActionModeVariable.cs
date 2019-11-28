using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionMode {NONE, MOVE, ACTION, ATTACK, HEAL, USE, TRADE, TALK, DOOR, LOCK}

[CreateAssetMenu(menuName = "Enums/ActionMode")]
public class ActionModeVariable : ScriptableObject {

	public ActionMode value;

	public bool IsTargetMode() {
		return (value == ActionMode.ATTACK || value == ActionMode.HEAL || value == ActionMode.TRADE || value == ActionMode.TALK || value == ActionMode.DOOR);
	}
}
