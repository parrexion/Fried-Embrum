using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour {

	[Tooltip("Event to register with.")]
	public GameEvent evnt;

	[Tooltip("Response function")]
	public UnityEvent response;
	

	private void OnEnable() {
		evnt.RegisterListener(this);
	}

	private void OnDisable() {
		evnt.UnregisterListener(this);
	}

	public void OnEventRaised() {
		response.Invoke();
	}
}
