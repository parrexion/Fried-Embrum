using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/CharacterList")]
public class CharacterListVariable : ScriptableObject {

	public List<TacticsMove> values = new List<TacticsMove>();
}
