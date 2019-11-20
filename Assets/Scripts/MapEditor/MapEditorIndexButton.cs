using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class MapEditorIndexButton : MonoBehaviour {

	public delegate void ButtonClick();

	public Image buttonImage;


	public void Setup(ButtonClick func) {
		GetComponent<Toggle>().onValueChanged.AddListener((x) => {
			if (x) {
				func();
			}
		});
	}
}
