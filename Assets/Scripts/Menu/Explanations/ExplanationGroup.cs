using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI Group which contains explanation objects and shows 
/// them to the provided tooltip.
/// </summary>
public class ExplanationGroup : MonoBehaviour {

	public Text tooltip;
	public ExplanationObject selectedObject;

	private void Start () {
		UpdateSelection(false);
	}

	public void MoveLeft() {
		if(selectedObject.leftObject != null) {
			selectedObject.highlight.enabled = false;
			selectedObject = selectedObject.leftObject;
			selectedObject.highlight.enabled = true;
		}
		UpdateSelection(true);
	}

	public void MoveRight() {
		if(selectedObject.rightObject != null) {
			selectedObject.highlight.enabled = false;
			selectedObject = selectedObject.rightObject;
			selectedObject.highlight.enabled = true;
		}
		UpdateSelection(true);
	}

	public void MoveUp() {
		if(selectedObject.upObject != null) {
			selectedObject.highlight.enabled = false;
			selectedObject = selectedObject.upObject;
			selectedObject.highlight.enabled = true;
		}
		UpdateSelection(true);
	}

	public void MoveDown() {
		if(selectedObject.downObject != null) {
			selectedObject.highlight.enabled = false;
			selectedObject = selectedObject.downObject;
			selectedObject.highlight.enabled = true;
		}
		UpdateSelection(true);
	}

	public void UpdateSelection(bool active) {
		selectedObject.highlight.enabled = active;
		if (active)
			tooltip.text = selectedObject.GetTooltip();
	}
}
