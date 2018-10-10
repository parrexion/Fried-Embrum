using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExplanationGroup : MonoBehaviour {

	public Text tooltip;
	public ExplanationObject[] explanations;

	private int currentIndex;


	private void Start () {
		UpdateSelection(false);
	}

	public void MoveUp() {
		do {
			currentIndex--;
			if (currentIndex < 0) {
				currentIndex = explanations.Length -1;
			}
		} while (!explanations[currentIndex].IsActive());
		UpdateSelection(true);
	}

	public void MoveDown() {
		do {
			currentIndex++;
			if (currentIndex >= explanations.Length) {
				currentIndex = 0;
			}
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
