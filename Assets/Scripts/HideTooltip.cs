using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HideTooltip : MonoBehaviour, IPointerDownHandler {
	
	public GameObject mainObject;
	
	
	public void OnPointerDown(PointerEventData eventData) {
		mainObject.SetActive(false);
	}
}
