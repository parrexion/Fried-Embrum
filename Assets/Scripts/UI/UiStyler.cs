using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UiStyler : ScriptableObject {

	[Header("Text styles")]
	public TextStyle hugeText;
	public TextStyle titleText;
	public TextStyle subTitleText;
	public TextStyle breadText;

	[Header("Button Styles")]
	public UIStyle mainStyle;
	public UIStyle baseStyle;
	public UIStyle actionStyle;

	[Header("Prompt Styles")]
	public PromptStyle selectPopup;
	public PromptStyle smallPopup;

}


[System.Serializable]
public class UIStyle {
	public Sprite baseImage;
	public Color baseColor = Color.white;
	public Sprite highImage;
	public Color highColor = Color.white;
	public Font font;
	public Color fontColor = Color.black;
	public int fontMaxSize = 40;
}

[System.Serializable]
public class TextStyle {
	public Color color = Color.black;
	public Font font;
	public int fontMaxSize = 40;
}

[System.Serializable]
public class PromptStyle {
	public Sprite backgroundImage;
	public Color backgroundColor = Color.white;
	public UIStyle buttonStyle;
}
