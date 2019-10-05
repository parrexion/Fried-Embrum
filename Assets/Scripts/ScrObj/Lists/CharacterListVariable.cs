using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/Character List")]
public class CharacterListVariable : ScriptableObject {

	public int Count { get { return values.Count; } }

	public List<TacticsMove> values = new List<TacticsMove>();


	public int AliveCount() {
		int alive = 0;
		for (int i = 0; i < values.Count; i++) {
			if (values[i].IsAlive())
				alive++;
		}
		return alive;
	}
}
