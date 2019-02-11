using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyButton : MonoBehaviour {

	public Image buttonImage;
	public Text buttonText;
	public Image highlight;


	public void SetSelected(bool selected) {
		highlight.enabled = selected;
	}
}
