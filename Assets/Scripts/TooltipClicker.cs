using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum TooltipPosition { WEAPON, SUPPORT, SKILL, TYPEA, TYPEB, TYPEC }

public class TooltipClicker : MonoBehaviour, IPointerDownHandler {

	public UICharacterStats uiStats;
	public TooltipPosition pos;


	public void OnPointerDown(PointerEventData eventData) {
		uiStats.ShowTooltip(transform, pos);
	}
}
