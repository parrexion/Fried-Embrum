using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/Character List")]
public class CharacterListVariable : ScriptableObject {

	public int Count { get { return values.Count; } }

	public List<TacticsMove> values = new List<TacticsMove>();
}
