using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditDragHandler : MonoBehaviour, IPointerDownHandler {

    public CharacterStatsVariable clickCharacter;
    public StatsContainer stats;
    public UnityEvent prepUpdateEvent;


    public void AddItem(StatsContainer charItem) {
        stats = charItem;

        GetComponent<Image>().sprite = stats.portrait;
    }
    
    public void OnPointerDown(PointerEventData eventData) {
        if (stats.id == -1)
            return;
        
        clickCharacter.value = stats;
        prepUpdateEvent.Invoke();
    }
}
