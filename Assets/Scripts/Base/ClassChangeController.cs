using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClassChangeController : MonoBehaviour {

	public PlayerData playerData;
	public GameObject listView;
	public GameObject changeView;
	public ClassWheel classWheel;
	public IconLibrary wpnIcons;
	private bool changeMode;
	private bool popupMode;

	[Header("Entry List")]
	public Transform listParent;
	public Transform entryPrefab;
	public int visibleSize;
	private EntryList<TrainingListEntry> entryList;

	[Header("Class List")]
	public Transform classParent;
	public Transform classPrefab;
	public int visibleClassSize;
	private EntryList<ClassListEntry> classList;
	private StatsContainer selectedChar;
	private List<LevelGain> gains = new List<LevelGain>();

	[Header("Class gain")]
	public Text[] statsBoosts;
	public Transform wpnSkillParent;
	public Transform wpnSkillTemplate;
	private List<Transform> wpnList = new List<Transform>();
	public Image skillGainIcon;
	public Text skillGainName;
	public Text skillGainDesc;

	[Header("Class Change Animation")]
	public LevelupScript levelupScript;
	public AudioQueueVariable sfxQueue;
	public SfxEntry levelupFanfare;
	public IntVariable slowGameSpeed;
	public IntVariable currentGameSpeed;
	public UnityEvent playSfxEvent;
	public UnityEvent stopSfxEvent;
	public BoolVariable lockControls;

	[Header("Information box")]
	public Text characterName;
	public Image portrait;
	public Text className;
	public Text level;
	public Text exp;
	public Image[] skillIcons;
	public Text hpText;
	public Text dmgText;
	public Text mndText;
	public Text sklText;
	public Text spdText;
	public Text defText;
	public Transform wpnParent;
	public Transform wpnTemplate;
	private List<Transform> wpnRanks = new List<Transform>();


	private void Start() {
		listView.SetActive(true);
		changeView.SetActive(false);
		entryList = new EntryList<TrainingListEntry>(visibleSize);
		classList = new EntryList<ClassListEntry>(visibleClassSize);
		CreateListEntries();
		MoveSelection(0);
	}

	private void CreateListEntries() {
		for (int i = listParent.childCount - 1; i > 1; i--) {
			Destroy(listParent.GetChild(i).gameObject);
		}

		entryList.ResetList();
		for (int i = 0; i < playerData.stats.Count; i++) {
			Transform t = Instantiate(entryPrefab, listParent);
			TrainingListEntry entry = entryList.CreateEntry(t);
			entry.FillData(playerData.stats[i]);
			entry.SetDark(!playerData.stats[i].CanLevelup());

			t.gameObject.SetActive(true);
		}
		entryPrefab.gameObject.SetActive(false);
	}

	public void MoveSelection(int dir) {
		if (!changeMode) {
			entryList.Move(dir);
		}
		else {
			classList.Move(dir);
			SetupClassGains();
		}
	}

	public void SelectCharacter() {
		if (popupMode) {
			levelupScript.Continue();
		}
		else if (!changeMode) {
			if (entryList.GetEntry().dark)
				return;
			changeMode = true;
			selectedChar = playerData.stats[entryList.GetPosition()];
			CreateClassList();
			SetupCharacterInfo();
			SetupClassGains();
			changeView.SetActive(true);
			listView.SetActive(false);
		}
		else {
			StartCoroutine(ChangeClass());
		}
	}

	public bool DeselectCharacter() {
		if (popupMode)
			return false;

		if (!changeMode) {
			return true;
		}
		else {
			changeMode = false;
			listView.SetActive(true);
			changeView.SetActive(false);
			return false;
		}
	}

	private void CreateClassList() {
		classList.ResetList();
		gains = classWheel.LevelupOptions(selectedChar.classLevels);
		for (int i = 0; i < gains.Count; i++) {
			Transform t = Instantiate(classPrefab, classParent);
			ClassListEntry entry = classList.CreateEntry(t);
			entry.FillData(gains[i]);
		}
		classPrefab.gameObject.SetActive(false);
		MoveSelection(0);
	}

	private void SetupCharacterInfo() {
		StatsContainer stats = playerData.stats[entryList.GetPosition()];
		InventoryContainer inventory = playerData.inventory[entryList.GetPosition()];
		SkillsContainer skills = playerData.skills[entryList.GetPosition()];

		characterName.text = stats.charData.entryName;
		portrait.sprite = stats.charData.portrait;
		className.text = stats.currentClass.entryName;
		level.text = "Level: " + stats.level;
		exp.text = "EXP: " + stats.currentExp;

		for (int i = 0; i < skillIcons.Length; i++) {
			if (skills.skills[i] == null) {
				skillIcons[i].enabled = false;
			}
			else {
				skillIcons[i].sprite = skills.skills[i].icon;
				skillIcons[i].enabled = true;
			}
		}
		
		hpText.text = "HP:  " + stats.hp.ToString();
		dmgText.text = "Dmg:  " + stats.dmg.ToString();
		mndText.text = "Mnd:  " + stats.mnd.ToString();
		sklText.text = "Skl:  " + stats.skl.ToString();
		spdText.text = "Spd:  " + stats.spd.ToString();
		defText.text = "Def:  " + stats.def.ToString();

		for (int i = 0; i < wpnRanks.Count; i++) {
			Destroy(wpnRanks[i].gameObject);
		}
		wpnRanks.Clear();
		for (int i = 0; i < inventory.wpnSkills.Length; i++) {
			if (inventory.wpnSkills[i] == WeaponRank.NONE)
				continue;

			Transform t = Instantiate(wpnTemplate, wpnParent);
			t.GetComponentInChildren<Image>().sprite = wpnIcons.icons[i];
			t.GetComponentInChildren<Text>().text = inventory.wpnSkills[i].ToString();
			t.gameObject.SetActive(true);
			wpnRanks.Add(t);
		}
		wpnTemplate.gameObject.SetActive(false);

	}

	private void SetupClassGains() {
		LevelGain level = gains[classList.GetPosition()];

		//Stats
		statsBoosts[0].transform.parent.gameObject.SetActive(level.bonusHp > 0);
		statsBoosts[0].text = "+" + level.bonusHp;
		statsBoosts[1].transform.parent.gameObject.SetActive(level.bonusDmg > 0);
		statsBoosts[1].text = "+" + level.bonusDmg;
		statsBoosts[2].transform.parent.gameObject.SetActive(level.bonusMnd > 0);
		statsBoosts[2].text = "+" + level.bonusMnd;
		statsBoosts[3].transform.parent.gameObject.SetActive(level.bonusSkl > 0);
		statsBoosts[3].text = "+" + level.bonusSkl;
		statsBoosts[4].transform.parent.gameObject.SetActive(level.bonusSpd > 0);
		statsBoosts[4].text = "+" + level.bonusSpd;
		statsBoosts[5].transform.parent.gameObject.SetActive(level.bonusDef > 0);
		statsBoosts[5].text = "+" + level.bonusDef;

		//Wpn skills
		for (int i = 0; i < wpnList.Count; i++) {
			Destroy(wpnList[i].gameObject);
		}
		wpnList.Clear();
		for (int i = 0; i < level.weaponSkills.Count; i++) {
			Transform t = Instantiate(wpnSkillTemplate, wpnSkillParent);
			t.GetComponentInChildren<Image>().sprite = wpnIcons.icons[(int)level.weaponSkills[i]];

			WeaponRank current = playerData.inventory[entryList.GetPosition()].wpnSkills[(int)level.weaponSkills[i]];
			WeaponRank next = (current + 1);
			if (current != WeaponRank.NONE) {
				t.GetComponentInChildren<Text>().text = current + " > " + next;
			}
			else {
				t.GetComponentInChildren<Text>().text = "+ " + next;
			}

			t.gameObject.SetActive(true);
			wpnList.Add(t);
		}
		wpnSkillTemplate.gameObject.SetActive(false);

		//Skill
		skillGainIcon.sprite = level.skill.icon;
		skillGainName.text = level.skill.entryName;
		skillGainDesc.text = level.skill.description;
	}

	private IEnumerator ChangeClass() {
		lockControls.value = true;
		LevelGain level = gains[classList.GetPosition()];

		yield return new WaitForSeconds(1f * slowGameSpeed.value / currentGameSpeed.value);

		StatsContainer stats = playerData.stats[entryList.GetPosition()];
		InventoryContainer inventory = playerData.inventory[entryList.GetPosition()];
		SkillsContainer skills = playerData.skills[entryList.GetPosition()];

		levelupScript.SetupStats(stats, false);
		Debug.Log("CLASS CHANGE!");
		stats.ClassGain(level, (int)level.playerClassName);
		inventory.IncreaseWpnSkill(level.weaponSkills);
		skills.GainSkill(level.skill);

		sfxQueue.Enqueue(levelupFanfare);
		playSfxEvent.Invoke();
		popupMode = true;
		yield return StartCoroutine(levelupScript.RunLevelup(stats));
		popupMode = false;

		CreateListEntries();
		DeselectCharacter();
		lockControls.value = false;
		yield break;
	}
}
// class change anim och vanlig levelup