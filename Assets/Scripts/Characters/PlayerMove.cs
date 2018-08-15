using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMove : TacticsMove {

	public IntVariable currentMenuMode;
	public UnityEvent menuModeChangedEvent;


	// Use this for initialization
	protected override void SetupLists() {
		playerList.values.Add(this);
		Debug.Log(stats.charData.charName);
	}
	
	protected override void EndMovement() {
		Debug.Log("Finished move");
		isMoving = false;
		lockControls.value = false;
		currentMenuMode.value = (int)MenuMode.UNIT;
		menuModeChangedEvent.Invoke();
	}
}
