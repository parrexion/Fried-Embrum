using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler {

    public BoolVariable lockControls;
    public MapClicker mapClicker;
    public PlayerMove tactics;


    public void OnBeginDrag(PointerEventData eventData) {
        if (lockControls.value || tactics.hasMoved)
            return;

        int x = Mathf.FloorToInt(0.5f + transform.position.x);
        int y = Mathf.FloorToInt(0.5f + transform.position.y);
        mapClicker.BeginDrag(x,y);
    }

    public void OnDrag(PointerEventData eventData) {
        if (lockControls.value || tactics.hasMoved)
            return;
        transform.position = new Vector3(eventData.pointerCurrentRaycast.worldPosition.x, 
                                    eventData.pointerCurrentRaycast.worldPosition.y, -0.1f);
        int x = Mathf.FloorToInt(0.5f + transform.position.x);
        int y = Mathf.FloorToInt(0.5f + transform.position.y);
        mapClicker.DuringDrag(x, y);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (lockControls.value || tactics.hasMoved)
            return;

        transform.localPosition = new Vector3(tactics.posx, tactics.posy, -0.1f);
        mapClicker.EndDrag();
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (lockControls.value)
            return;
        int x = Mathf.FloorToInt(0.5f + transform.position.x);
        int y = Mathf.FloorToInt(0.5f + transform.position.y);
        mapClicker.CharacterClicked(x,y);
    }
}
