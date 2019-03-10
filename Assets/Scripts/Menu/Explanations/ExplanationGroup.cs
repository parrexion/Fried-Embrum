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
	public ExplanationObject[] explanations;

	private int currentIndex;


	private void Start () {
		UpdateSelection(false);
	}

	public void Move(int dir) {
		do {
			currentIndex = OPMath.FullLoop(0, explanations.Length, currentIndex + dir);
		} while (!explanations[currentIndex].IsActive());
		UpdateSelection(true);
	}

	public void UpdateSelection(bool active) {
		for (int i = 0; i < explanations.Length; i++) {
			explanations[i].highlight.enabled = (currentIndex == i && active);
			if (currentIndex == i)
				tooltip.text = explanations[i].GetTooltip();
		}
	}
}
