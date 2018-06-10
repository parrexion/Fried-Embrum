using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/SaveList")]
public class SaveListVariable : ScriptableObject {

	public StatsContainer[] values = new StatsContainer[0];


	public bool AddNew(int stars, CharacterStats stats) {
		for (int i = 0; i < values.Length; i++) {
			if (values[i].id != -1)
				continue;
			CharacterSave save = new CharacterSave();
			save.GenerateDataFromStars(stars, stats);
			StatsContainer cont = new StatsContainer(save, stats) {
				id = ++SaveController.highestID,
				statsID = stats.statsID
			};
			values[i] = cont;
			return true;
		}

		return false;
	}
}
