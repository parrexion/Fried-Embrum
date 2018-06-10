using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionMode {NONE, MOVE, ATTACK, HEAL}

[CreateAssetMenu(menuName = "Variables/ActionMode")]
public class ActionModeVariable : ScriptableObject {

	public ActionMode value;
}
