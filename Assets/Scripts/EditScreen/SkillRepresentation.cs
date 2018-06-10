using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum SkillRepType { WEAPON, SUPPORT, SKILL, SKILLA, SKILLB, SKILLC }

public class SkillRepresentation : MonoBehaviour {

	public int skillID;
	public SkillRepType type;
	public int cost;
	public Button clickButton;
	public Image buttonImage;
	public Image typeIcon;
	public Text typeCharacter;
	public Text skillName;
	public Text description;
	public Text costText;

	public UnityEvent skillClickedEvent;


	public void WhenClicked() {
		EditController.intindex = skillID;
		skillClickedEvent.Invoke();
	}
}
