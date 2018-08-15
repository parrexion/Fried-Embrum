using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationUIHandler : MonoBehaviour {

	
	public Transform equipItemsParent;
	public Transform bagItemsParent;

	private Slot[] _equipSlots;
	private Slot[] _bagSlots;

	public SaveListVariable invItemEquip;
	public SaveListVariable invItemBag;
	
	
	// Use this for initialization
	private void Start () {

		//Slot initialization
		_equipSlots = equipItemsParent.GetComponentsInChildren<Slot>();
		for (int i = 0; i < _equipSlots.Length; i++) {
			_equipSlots[i].slotID = -(i+1);
		}
		_bagSlots = bagItemsParent.GetComponentsInChildren<Slot>();
		for (int i = 0; i < _bagSlots.Length; i++) {
			_bagSlots[i].slotID = i;
		}

		Debug.Log("Initiated the slot ids");
		UpdateUI();
	}

	/// <summary>
	/// Update function for the UI. Uses the inventory to update the images of all the inventory slots.
	/// </summary>
	public void UpdateUI() {

		//Update the equipment
		for (int i = 0; i < _equipSlots.Length; i++) {
			if (invItemEquip.stats[i] != null) {
				_equipSlots[i].AddItem(invItemEquip.stats[i], invItemEquip.inventory[i]);
			}
			else {
				_equipSlots[i].ClearSlot();
			}
		}

		for (int i = 0; i < _bagSlots.Length; i++) {
			if (invItemBag.stats[i] != null) {
				_bagSlots[i].AddItem(invItemBag.stats[i], invItemBag.inventory[i]);
			}
			else {
				_bagSlots[i].ClearSlot();
			}
		}
	}
}
