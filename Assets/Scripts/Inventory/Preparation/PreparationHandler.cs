using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PreparationHandler : MonoBehaviour {

	public SaveListVariable equippedUnits;
	public SaveListVariable availableUnits;
	public UnityEvent itemsChanged;
	

	public void Swap(int slotA, int slotB) {

//		Debug.Log(string.Format("Swappy: {0} <> {1}", slotA, slotB));
		StatsContainer temp = GetItem(slotA);
		SetItem(slotA,GetItem(slotB));
		SetItem(slotB,temp);

		itemsChanged.Invoke();
	}

	/// <summary>
	/// Retrieves the item at the current index.
	/// Equipped items use negative indexing starting at -1
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	private StatsContainer GetItem(int index) {
		if (index < 0) {
			index = -(index+1);
			return (index < equippedUnits.stats.Count) ? equippedUnits.stats[index] : null;
		}
		else {
			return (index < availableUnits.stats.Count) ? availableUnits.stats[index] : null;
		}
	}

	/// <summary>
	/// Sets the given item in the slot at the index.
	/// Equipped items use negative indexing starting at -1
	/// </summary>
	/// <param name="index"></param>
	/// <param name="item"></param>
	private void SetItem(int index, StatsContainer item) {

		if (index < 0) {
			index = -(index+1);
			if (index < equippedUnits.stats.Count) {
				equippedUnits.stats[index] = item;
			}
		}
		else {
			if (index < availableUnits.stats.Count) {
				availableUnits.stats[index] = item;
			}
		}
	}
}
