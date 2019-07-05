using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplanationObject : MonoBehaviour {

	public Image highlight;
	public ScrObjEntryReference scrObject;
	public string fallbackString;


	private void Start () {
		highlight.enabled = false;
	}

	public bool IsActive() {
		return true;
	}

	public virtual string GetTooltip() {
		return fallbackString;
	}
}

