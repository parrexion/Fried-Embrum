using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplanationObject : MonoBehaviour {

	public Image highlight;
	public ScrObjEntryReference scrObject;
	public string fallbackString;

	[Header("Directions")]
	public ExplanationObject upObject;
	public ExplanationObject downObject;
	public ExplanationObject leftObject;
	public ExplanationObject rightObject;


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

