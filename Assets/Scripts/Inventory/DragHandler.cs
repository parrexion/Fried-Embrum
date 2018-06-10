using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {

    public static GameObject itemBeingDragged;

    public CharacterStatsVariable clickCharacter;
    public Slot slot;
    public int startID;
    public UnityEvent prepUpdateEvent;
    
    private Image _image;
    private Transform _invParent;


    private void Start() {
        _image = GetComponent<Image>();
        _invParent = transform.parent.transform.parent.transform;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        itemBeingDragged = gameObject;
        startID = slot.slotID;
        _image.raycastTarget = false;
        transform.parent.transform.SetAsLastSibling();
        _invParent.SetAsLastSibling();
        clickCharacter.value = slot.item;
        prepUpdateEvent.Invoke();
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) {
        itemBeingDragged = null;
        _image.raycastTarget = true;
        transform.localPosition = Vector3.zero;
    }

    public void OnPointerClick(PointerEventData eventData) {
        clickCharacter.value = slot.item;
        prepUpdateEvent.Invoke();
    }
}
