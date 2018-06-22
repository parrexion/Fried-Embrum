using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum TooltipType { NONE, NAME, CLASS, INV1, INV2, INV3, INV4, INV5, SKL1, SKL2, SKL3, SKL4, SKL5 };

public class TooltipClicker : MonoBehaviour, IPointerDownHandler {

	public IntVariable selectedItem;
	public TooltipType position;
	public Image highlight;

	public UnityEvent showTooltipEvent;


	private void OnEnable() {
		highlight.enabled = false;
	}

	public void OnPointerDown(PointerEventData eventData) {
		selectedItem.value = (int)position;
		showTooltipEvent.Invoke();
	}

	public void ShowHighlight() {
		highlight.enabled = ((int) position == selectedItem.value);
	}
}
