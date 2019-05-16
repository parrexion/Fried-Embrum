using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UiStyler : ScriptableObject {
	
	public Font font;

	[Header("Text Styles")]
	public TextStyle hugeText;
	public TextStyle titleText;
	public TextStyle subTitleText;
	public TextStyle breadText;
	public TextStyle objectiveText;
	public TextStyle menuTitleText;
	public TextStyle statsBigText;
	public TextStyle statsMediumText;
	public TextStyle statsSmallText;
	public TextStyle statsPenaltyText;
	public TextStyle baseTitleText;
	public TextStyle baseBigText;
	public TextStyle baseMediumText;
	public TextStyle baseSmallText;
	public TextStyle levelBonusText;
	public TextStyle damageText;

	[Header("Button Styles")]
	public UIStyle mainStyle;
	public UIStyle baseStyle;
	public UIStyle actionStyle;
	public UIStyle noSelectStyle;
	public UIStyle iconStyle;
	public UIStyle optionsStyle;

	[Header("Bar Styles")]
	public BarStyle healthBar;
	public BarStyle expBar;
	public BarStyle fulfillBar;
	public BarStyle bigExpBar;

	[Header("List Styles")]
	public UIStyle optionsList;
	public UIStyle thinList;
	public UIStyle saveList;
	public UIStyle tradeList;
	public UIStyle prepList;

	[Header("Prompt Styles")]
	public PromptStyle selectPopup;
	public PromptStyle smallPopup;

	[Header("Spinner Styles")]
	public SpinnerStyle bigSpinner;
	public SpinnerStyle smallSpinner;

}

[System.Serializable]
public class UIStyle {
	public Sprite baseImage;
	public Color baseColor = Color.white;
	public Sprite highImage;
	public Color highColor = Color.white;
	public Color fontColor = Color.black;
	public int fontMaxSize = 40;
}

[System.Serializable]
public class TextStyle {
	public Color color = Color.black;
	public int fontMaxSize = 40;
}

[System.Serializable]
public class PromptStyle {
	public Sprite backgroundImage;
	public Color backgroundColor = Color.white;
	public UIStyle buttonStyle;
}

[System.Serializable]
public class SpinnerStyle {
	public Sprite backgroundImage;
	public Color backgroundColor = Color.white;
	public Color fontColor = Color.black;
	public int fontMaxSize = 40;
}

[System.Serializable]
public class UISingleImage {
	public Sprite image;
	public Color color = Color.white;
}

[System.Serializable]
public class BarStyle {
	public Sprite backgroundImage;
	public Sprite fillImage;
	public Color fillColor = Color.white;
	public Color textColor = Color.white;
	public int fontMaxSize = 40;
}

