using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class EditController : MonoBehaviour {

	public static int intindex = -1;
	
	public CharacterStatsVariable clickCharacter;
	public SaveListVariable availableUnits;
	public Transform characterListParent;
	public CharacterStats[] characters;
	
	[Header("UI")]
	public Image[] upgradeBackgrounds;
	public GameObject skillMenuObject;
	public Text spText;

	[Header("SkillList")]
	public Transform skillListParent;
	public Transform skillTemplate;
	public List<SkillRepresentation> buttonList = new List<SkillRepresentation>();

	public UnityEvent saveGameEvent;
	
	private EditDragHandler[] _slots;
	private bool _isSelecting;
	private int _lastSelected = -1;


	private void Start() {
		clickCharacter.value = null;
		_slots = characterListParent.GetComponentsInChildren<EditDragHandler>();
		Debug.Log("START");
		UpdateAvailableCharacters();
		ShowButtons();
		spText.text = "";
	}

	public void UpdateAvailableCharacters() {
		for (int i = 0; i < _slots.Length; i++) {
			_slots[i].AddItem(availableUnits.values[i]);
		}
	}

	public void PopulateSkillList() {
		ClearList();
		int id = 0;
		StatsContainer cont = clickCharacter.value;
		CharacterStats stats = characters[clickCharacter.value.statsID];
		spText.text = cont.currentSp.ToString();
		
		//Weapons
		for (int i = 0; i < stats.weapons.Length; i++) {
			WeaponSkill weapon = stats.weapons[i];
			Transform skill = Instantiate(skillTemplate);
			skill.SetParent(skillListParent);
			
			SkillRepresentation rep = skill.GetComponent<SkillRepresentation>();
			rep.skillID = id++;
			rep.type = SkillRepType.WEAPON;
			rep.typeIcon.color = Color.red;
			rep.skillName.text = weapon.skillName;
			rep.description.text = weapon.description;
			rep.cost = weapon.cost;
			rep.costText.text = (i <= cont.weaponLevel) ? "Unlocked" : "SP: " + weapon.cost;
			rep.costText.color = (cont.currentSp < weapon.cost && i == cont.weaponLevel+1) ? Color.red : Color.black;
			rep.buttonImage.color = (i <= cont.weaponLevel) ? new Color(0.4f, 0.8f, 0.4f) : 
				(i == cont.weaponLevel+1) ? Color.white : Color.gray;
			rep.clickButton.interactable = (i == cont.weaponLevel+1 && cont.currentSp >= weapon.cost);
			buttonList.Add(rep);
			skill.gameObject.SetActive(true);
		}

		//Supports
		for (int i = 0; i < stats.supports.Length; i++) {
			SupportSkill support = stats.supports[i];
			Transform skill = Instantiate(skillTemplate);
			skill.SetParent(skillListParent);
			
			SkillRepresentation rep = skill.GetComponent<SkillRepresentation>();
			rep.skillID = id++;
			rep.type = SkillRepType.SUPPORT;
			rep.typeIcon.color = Color.cyan;
			rep.skillName.text = support.skillName;
			rep.description.text = support.description;
			rep.cost = support.cost;
			rep.costText.text = (i <= cont.supportLevel) ? "Unlocked" : "SP: " + support.cost;
			rep.costText.color = (cont.currentSp < support.cost && i == cont.supportLevel+1) ? Color.red : Color.black;
			rep.buttonImage.color = (i <= cont.supportLevel) ? new Color(0.4f, 0.8f, 0.4f) : 
				(i == cont.supportLevel+1) ? Color.white : Color.gray;
			rep.clickButton.interactable = (i == cont.supportLevel+1 && cont.currentSp >= support.cost);
			buttonList.Add(rep);
			skill.gameObject.SetActive(true);
		}

		//Skills
		for (int i = 0; i < stats.skills.Length; i++) {
			SkillSkill skill = stats.skills[i];
			Transform skillTransform = Instantiate(skillTemplate);
			skillTransform.SetParent(skillListParent);
			
			SkillRepresentation rep = skillTransform.GetComponent<SkillRepresentation>();
			rep.skillID = id++;
			rep.type = SkillRepType.SKILL;
			rep.typeIcon.color = Color.magenta;
			rep.skillName.text = skill.skillName;
			rep.description.text = skill.description;
			rep.cost = skill.cost;
			rep.costText.text = (i <= cont.skillLevel) ? "Unlocked" : "SP: " + skill.cost;
			rep.costText.color = (cont.currentSp < skill.cost && i == cont.skillLevel+1) ? Color.red : Color.black;
			rep.buttonImage.color = (i <= cont.skillLevel) ? new Color(0.4f, 0.8f, 0.4f) : 
				(i == cont.skillLevel+1) ? Color.white : Color.gray;
			rep.clickButton.interactable = (i == cont.skillLevel+1 && cont.currentSp >= skill.cost);
			buttonList.Add(rep);
			skillTransform.gameObject.SetActive(true);
		}

		//Skill A
		for (int i = 0; i < stats.skillsA.Length; i++) {
			PassiveSkill passive = stats.skillsA[i];
			Transform skillTransform = Instantiate(skillTemplate);
			skillTransform.SetParent(skillListParent);
			
			SkillRepresentation rep = skillTransform.GetComponent<SkillRepresentation>();
			rep.skillID = id++;
			rep.type = SkillRepType.SKILLA;
			rep.typeIcon.color = new Color(1f,0.5f,0);
			rep.typeCharacter.text = "A";
			rep.skillName.text = passive.baseSkill.skillName;
			rep.description.text = passive.baseSkill.description;
			rep.cost = passive.baseSkill.cost;
			rep.costText.text = (i <= cont.skillALevel) ? "Unlocked" : "SP: " + passive.baseSkill.cost;
			rep.costText.color = (cont.currentSp < passive.baseSkill.cost && i == cont.skillALevel+1) ? Color.red : Color.black;
			rep.buttonImage.color = (i <= cont.skillALevel) ? new Color(0.4f, 0.8f, 0.4f) : 
				(i == cont.skillALevel+1) ? Color.white : Color.gray;
			rep.clickButton.interactable = (i == cont.skillALevel+1 && cont.currentSp >= passive.baseSkill.cost);
			buttonList.Add(rep);
			skillTransform.gameObject.SetActive(true);
		}

		//Skill B
		for (int i = 0; i < stats.skillsB.Length; i++) {
			PassiveSkill passive = stats.skillsB[i];
			Transform skillTransform = Instantiate(skillTemplate);
			skillTransform.SetParent(skillListParent);
			
			SkillRepresentation rep = skillTransform.GetComponent<SkillRepresentation>();
			rep.skillID = id++;
			rep.type = SkillRepType.SKILLB;
			rep.typeIcon.color = new Color(1f,0.5f,0);
			rep.typeCharacter.text = "B";
			rep.skillName.text = passive.baseSkill.skillName;
			rep.description.text = passive.baseSkill.description;
			rep.cost = passive.baseSkill.cost;
			rep.costText.text = (i <= cont.skillBLevel) ? "Unlocked" : "SP: " + passive.baseSkill.cost;
			rep.costText.color = (cont.currentSp < passive.baseSkill.cost && i == cont.skillBLevel+1) ? Color.red : Color.black;
			rep.buttonImage.color = (i <= cont.skillBLevel) ? new Color(0.4f, 0.8f, 0.4f) : 
				(i == cont.skillBLevel+1) ? Color.white : Color.gray;
			rep.clickButton.interactable = (i == cont.skillBLevel+1 && cont.currentSp >= passive.baseSkill.cost);
			buttonList.Add(rep);
			skillTransform.gameObject.SetActive(true);
		}

		//Skill C
		for (int i = 0; i < stats.skillsC.Length; i++) {
			PassiveSkill passive = stats.skillsC[i];
			Transform skillTransform = Instantiate(skillTemplate);
			skillTransform.SetParent(skillListParent);
			
			SkillRepresentation rep = skillTransform.GetComponent<SkillRepresentation>();
			rep.skillID = id++;
			rep.type = SkillRepType.SKILLC;
			rep.typeIcon.color = new Color(1f,0.5f,0);
			rep.typeCharacter.text = "C";
			rep.skillName.text = passive.baseSkill.skillName;
			rep.description.text = passive.baseSkill.description;
			rep.cost = passive.baseSkill.cost;
			rep.costText.text = (i <= cont.skillCLevel) ? "Unlocked" : "SP: " + passive.baseSkill.cost;
			rep.costText.color = (cont.currentSp < passive.baseSkill.cost && i == cont.skillCLevel+1) ? Color.red : Color.black;
			rep.buttonImage.color = (i <= cont.skillCLevel) ? new Color(0.4f, 0.8f, 0.4f) : 
				(i == cont.skillCLevel+1) ? Color.white : Color.gray;
			rep.clickButton.interactable = (i == cont.skillCLevel+1 && cont.currentSp >= passive.baseSkill.cost);
			buttonList.Add(rep);
			skillTransform.gameObject.SetActive(true);
		}

		Debug.Log("Finished populating");
	}

	private void ClearList() {
		for (int i = 0; i < buttonList.Count; i++) {
			Destroy(buttonList[i].gameObject);
		}
		buttonList.Clear();
	}

	public void SelectUpgrade() {
		if (_isSelecting)
			return;

		if (_lastSelected != -1) {
			buttonList[_lastSelected].buttonImage.color = Color.white;
		}

		_lastSelected = intindex;
		if (_lastSelected != -1) {
			buttonList[_lastSelected].buttonImage.color = Color.magenta;
		}

		if (intindex != -1) {
			_isSelecting = true;
			ShowButtons();
		}
	}

	public void UpgradeSkill() {
		clickCharacter.value.currentSp -= buttonList[intindex].cost;
		switch (buttonList[intindex].type)
		{
			case SkillRepType.WEAPON:
				clickCharacter.value.weaponLevel++;
				break;
			case SkillRepType.SUPPORT:
				clickCharacter.value.supportLevel++;
				break;
			case SkillRepType.SKILL:
				clickCharacter.value.skillLevel++;
				break;
			case SkillRepType.SKILLA:
				clickCharacter.value.skillALevel++;
				break;
			case SkillRepType.SKILLB:
				clickCharacter.value.skillBLevel++;
				break;
			case SkillRepType.SKILLC:
				clickCharacter.value.skillCLevel++;
				break;
		}
		saveGameEvent.Invoke();
		
		//Reset selection
		_isSelecting = false;
		ShowButtons();
		intindex = -1;
		SelectUpgrade();
		PopulateSkillList();
	}

	public void CancelSkill() {
		_isSelecting = false;
		ShowButtons();
		intindex = -1;
		SelectUpgrade();
	}

	private void ShowButtons() {
		skillMenuObject.SetActive(_isSelecting);
	}
}
