using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetController : MonoBehaviour {

	[Header("Target")]
	public MapTileListVariable targetList;
	public MapTileVariable target;

	public UnityEvent targetChangedEvent;

	private int targetIndex;


	public void Clear() {
		target.value = null;
		targetChangedEvent.Invoke();
	}

    public void UpdateSelection() {
		targetIndex = 0;
        target.value = targetList.values[0];
		targetChangedEvent.Invoke();
    }

	public void Move(int dir) {
		targetIndex = OPMath.FullLoop(0, targetList.values.Count, targetIndex + dir);
        target.value = targetList.values[targetIndex];
		targetChangedEvent.Invoke();
	}
}
