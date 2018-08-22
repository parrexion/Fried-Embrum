using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HideTooltip : MonoBehaviour {
	
	public GameObject mainObject;
	
	
	public void Hide(PointerEventData eventData) {
		mainObject.SetActive(false);
	}
}
