using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gatcha : MonoBehaviour, IPointerDownHandler {

	public GatchaController controller;
	public Image icon;
	public Image isSelected;

	[Space(10)]
	
	public bool hasBeenOpened;
	public int index;
	public int stars;
	public CharacterStats character;
	
	
	public void OnPointerDown(PointerEventData eventData) {
		controller.GatchaClicked(index);
	}
}
