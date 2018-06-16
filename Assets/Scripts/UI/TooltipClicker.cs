using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TooltipClicker : MonoBehaviour, IPointerDownHandler {

	public Item linkedItem;
	public StringVariable tooltipMessage;
	public UnityEvent showTooltipEvent;


	public void OnPointerDown(PointerEventData eventData) {
		tooltipMessage.value = linkedItem.itemName + "\n" + linkedItem.description;
		showTooltipEvent.Invoke();
	}
}
