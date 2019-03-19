using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionMode {NONE, MOVE, ACTION, ATTACK, HEAL, USE, TRADE, WEAPON, LOCK}

[CreateAssetMenu(menuName = "Enums/ActionMode")]
public class ActionModeVariable : ScriptableObject {

	public ActionMode value;
}
