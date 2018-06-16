using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/SaveList")]
public class SaveListVariable : ScriptableObject {

	public StatsContainer[] values = new StatsContainer[0];

}
