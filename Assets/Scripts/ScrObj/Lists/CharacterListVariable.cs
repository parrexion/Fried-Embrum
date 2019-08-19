using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/Character List")]
public class CharacterListVariable : ScriptableObject {

	public int Count { get { return values.Count; } }

	public List<TacticsMove> values = new List<TacticsMove>();


	public bool IsAnyoneAlive() {
		for (int i = 0; i < values.Count; i++) {
			if (values[i].IsAlive())
				return true;
		}
		return false;
	}
}
