using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/Trigger List")]
public class TriggerListVariable : ScriptableObject {

	public List<TriggerTuple> values = new List<TriggerTuple>();


	public bool IsTriggered(int index) {
		return values[index].triggered;
	}

	public bool IsTriggered(string id) {
		TriggerTuple tuple = values.Find((x) => x.id == id);
		return (tuple != null && tuple.triggered);
	}

	public void Trigger(string id) {
		TriggerTuple tuple = values.Find((x) => x.id == id);
		if (tuple != null) {
			tuple.triggered = true;
		}
	}
}


[System.Serializable]
public class TriggerTuple {
	public string id;
	public bool triggered;

	public TriggerTuple() { }

	public TriggerTuple(string id) {
		this.id = id;
	}
}