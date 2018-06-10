using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCDragHandler : MonoBehaviour, IPointerDownHandler {

    public MapClicker mapClicker;


    public void OnPointerDown(PointerEventData eventData) {
        if (TurnController.busy)
            return;
        int x = Mathf.FloorToInt(0.5f + transform.position.x);
        int y = Mathf.FloorToInt(0.5f + transform.position.y);
        mapClicker.CharacterClicked(x,y);
    }
}
