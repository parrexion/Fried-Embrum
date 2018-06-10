using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHandler : MonoBehaviour, IDropHandler {
	
	/// <summary>
	/// Get the child object representing the item in the slot.
	/// </summary>
	public GameObject Item {
		get {
			return (transform.childCount > 0) ? transform.GetChild(0).gameObject : null;
		}
	}
	
	public PreparationHandler invContainer;

	private DragHandler _dragHandler;
	private Slot _slot;


	private void Start() {
		_slot = GetComponent<Slot>();
	}
	
	public void OnDrop(PointerEventData eventData) {
		if (DragHandler.itemBeingDragged == null)
			return;
		int startID = DragHandler.itemBeingDragged.GetComponent<DragHandler>().startID;
		invContainer.Swap(startID, _slot.slotID);
	}
}
