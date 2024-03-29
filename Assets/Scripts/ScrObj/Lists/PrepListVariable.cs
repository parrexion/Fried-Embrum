﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/Prep List")]
public class PrepListVariable : ScriptableObject {

	public int Count { get { return values.Count; } }

	// Characters
	public List<PrepCharacter> values = new List<PrepCharacter>();


	public void ResetData() {
		values = new List<PrepCharacter>();
	}

	public void SortListIndex() {
		Debug.Log("SORT index");
		values.Sort((x, y) => x.index - y.index);
	}

	private const int XIsBetter = -1;
	private const int YIsBetter = 1;
	public void SortListPicked() {
		values.Sort((x, y) => {
			if (x.forced != y.forced) {
				return (x.forced) ? XIsBetter : YIsBetter;
			}
			else if (x.locked != y.locked) {
				return (!x.locked) ? XIsBetter : YIsBetter;
			}
			return x.index - y.index;
		});
	}
}
