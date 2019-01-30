using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/Prep List")]
public class PrepListVariable : ScriptableObject {

	// Characters
	public List<PrepCharacter> preps = new List<PrepCharacter>();


	public void ResetData() {
		preps = new List<PrepCharacter>();
	}

	public void SortListIndex() {
		Debug.Log("SORT index");
		preps.Sort((x,y) => x.index - y.index);
	}

	private const int XIsBetter = -1;
	private const int YIsBetter = 1;
	public void SortListPicked() {
		Debug.Log("SORT picked");
		preps.Sort((x, y) => {
			if (x.forced != y.forced) {
				return (x.forced) ? XIsBetter : YIsBetter;
			}
			else if (x.selected != y.selected) {
				return (x.selected) ? XIsBetter : YIsBetter;
			}
			else if (x.locked != y.locked) {
				return (!x.locked) ? XIsBetter : YIsBetter;
			}
			return x.index - y.index;
		});
	}
} 
