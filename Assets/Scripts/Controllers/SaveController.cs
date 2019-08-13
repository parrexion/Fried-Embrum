using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour {

	public const int SAVE_FILES_COUNT = 3;

	#region Singleton
	private static SaveController _instance;

	private void Awake() {
		if (_instance != null) {
			Destroy(gameObject);
		}
		else {
			DontDestroyOnLoad(gameObject);
			_instance = this;
			Initialize();
		}
	}
	#endregion

	public PlayerData playerData;
	public IntVariable saveIndex;

	[Header("Simple Data")]
	public StringVariable[] simpleChapterName;
	public IntVariable[] simpleTotalDays;
	public IntVariable[] simplePlayTimes;

	[Header("Current Data")]
	public ScrObjEntryReference currentMission;
	public IntVariable mapIndex;
	public PrepListVariable squad1;
	public PrepListVariable squad2;
	public ScrObjEntryReference currentDialogue;
	public StringVariable loadMapID;
	public IntVariable currentTotalDays;
	public IntVariable currentPlayTime;
	public IntVariable currentMoney;
	public IntVariable currentScrap;

	[Header("Libraries")]
	public ScrObjLibraryVariable missionLibrary;
	public ScrObjLibraryVariable characterLibrary;
	public ScrObjLibraryVariable classLibrary;
	public ScrObjLibraryVariable itemLibrary;
	public ScrObjLibraryVariable skillLibrary;
	public ScrObjLibraryVariable upgradeLibrary;

	[Header("Options")]
	public IntVariable musicVolume;
	public IntVariable sfxVolume;
	public IntVariable gameSpeed;
	public BoolVariable useAnimations;
	public BoolVariable trueHit;
	public BoolVariable autoEnd;
	public BoolVariable autoSelectCharacter;
	public IntVariable controlScheme;

	public UnityEvent preLoadFinishedEvent;
	public UnityEvent loadFinishedEvent;

	private string _savePath = "";
	private SavePackage saveFileData;


	private void Initialize() {
		_savePath = Application.persistentDataPath + "/saveData2.xml";
		PreLoad();
		//EmptySave();
	}

	public void EmptySave() {
		// Setup save data
		saveFileData = new SavePackage();
		for (int i = 0; i < SAVE_FILES_COUNT; i++) {
			saveFileData.saveFiles[i] = new SaveData();
		}

		//Write to file
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
		XmlSerializer serializer = new XmlSerializer(typeof(SavePackage));
		using (XmlWriter xmlWriter = XmlWriter.Create(_savePath, xmlWriterSettings)) {
			serializer.Serialize(xmlWriter, saveFileData);
		}
		Debug.Log("Successfully created a new save file!\n" + _savePath);
	}

	public void ResetCurrentData() {
		currentMoney.value = 0;
		currentScrap.value = 0;
		playerData.ResetData();
	}

	public void Save(bool onlyOptions) {
		// Options
		saveFileData.options.musicVolume = musicVolume.value;
		saveFileData.options.sfxVolume = sfxVolume.value;
		saveFileData.options.gameSpeed = gameSpeed.value;
		saveFileData.options.controlScheme = controlScheme.value;
		saveFileData.options.useAnimations = useAnimations.value;
		saveFileData.options.trueHit = trueHit.value;
		saveFileData.options.autoEnd = autoEnd.value;
		saveFileData.options.autoSelectCharacter = autoSelectCharacter.value;

		if (!onlyOptions) {
			saveFileData.lastSaveFileIndex = saveIndex.value;
			// Update simple data
			simpleChapterName[saveIndex.value].value = loadMapID.value;
			simpleTotalDays[saveIndex.value].value = currentTotalDays.value;
			simplePlayTimes[saveIndex.value].value = currentPlayTime.value;

			// Setup save file data
			SaveData data = new SaveData {
				chapterName = simpleChapterName[saveIndex.value].value,
				totalDays = simpleTotalDays[saveIndex.value].value,
				playTime = simplePlayTimes[saveIndex.value].value,
				money = currentMoney.value,
				scrap = currentScrap.value
			};

			//Map data
			data.mapData = new MapSavePackage() {
				missionString = currentMission.value.uuid,
				mapIndex = mapIndex.value
			};
			for (int i = 0; i < squad1.Count; i++) {
				data.mapData.squad1.Add(squad1.values[i].index);
			}
			for (int i = 0; i < squad2.Count; i++) {
				data.mapData.squad2.Add(squad2.values[i].index);
			}

			//Player data
			for (int i = 0; i < playerData.stats.Count; i++) {
				if (playerData.stats[i].charData == null)
					continue;
				CharacterSaveData c = new CharacterSaveData();
				c.StoreData(playerData.stats[i], playerData.inventory[i], playerData.skills[i], playerData.baseInfo[i]);
				data.characters.Add(c);
			}
			for (int i = 0; i < playerData.items.Count; i++) {
				ItemSaveData item = new ItemSaveData();
				item.StoreData(playerData.items[i]);
				data.items.Add(item);
			}
			for (int i = 0; i < playerData.upgrader.listSize; i++) {
				UpgradeSaveData upgrade = new UpgradeSaveData();
				upgrade.StoreData(playerData.upgrader.upgrades[i]);
				data.upgrade.Add(upgrade);
			}
			for (int i = 0; i < playerData.missions.Count; i++) {
				MissionSaveData mission = new MissionSaveData();
				mission.StoreData(playerData.missions[i]);
				data.missions.Add(mission);
			}
			saveFileData.saveFiles[saveIndex.value] = data;
		}

		//Write to file
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
		XmlSerializer serializer = new XmlSerializer(typeof(SavePackage));
		using (XmlWriter xmlWriter = XmlWriter.Create(_savePath, xmlWriterSettings)) {
			serializer.Serialize(xmlWriter, saveFileData);
		}
		if (onlyOptions) {
			Debug.Log("Successfully saved the options data!\n" + _savePath);
		}
		else {
			Debug.Log("Successfully saved the save data!\n" + _savePath);
		}
	}

	/// <summary>
	/// Does a pre load which is used to show which save files are used to
	/// give the player a sense of which save file to load.
	/// </summary>
	public void PreLoad() {
		string path = _savePath;
		if (!File.Exists(_savePath)) {
			Debug.LogWarning("No save file found: " + path);
			EmptySave();
		}
		else {
			XmlSerializer serializer = new XmlSerializer(typeof(SavePackage));
			FileStream file = File.Open(path, FileMode.Open);
			saveFileData = serializer.Deserialize(file) as SavePackage;
			file.Close();
		}

		if (saveFileData == null) {
			Debug.LogError("Could not open the file: " + path);
			return;
		}

		saveIndex.value = saveFileData.lastSaveFileIndex;

		// Options
		musicVolume.value = saveFileData.options.musicVolume;
		sfxVolume.value = saveFileData.options.sfxVolume;
		gameSpeed.value = saveFileData.options.gameSpeed;
		controlScheme.value = saveFileData.options.controlScheme;
		useAnimations.value = saveFileData.options.useAnimations;
		trueHit.value = saveFileData.options.trueHit;
		autoEnd.value = saveFileData.options.autoEnd;
		autoSelectCharacter.value = saveFileData.options.autoSelectCharacter;

		//Load save files info
		for (int i = 0; i < saveFileData.saveFiles.Length; i++) {
			if (saveFileData.saveFiles[i] == null)
				continue;
			simpleChapterName[i].value = saveFileData.saveFiles[i].chapterName;
			simpleTotalDays[i].value = saveFileData.saveFiles[i].totalDays;
			simplePlayTimes[i].value = saveFileData.saveFiles[i].playTime;
		}

		Debug.Log("Successfully pre-loaded the save data!");

		if (SceneManager.GetActiveScene().name == "BattleScene" || SceneManager.GetActiveScene().name == "BaseScene") {
			Load();
		}
		else {
			preLoadFinishedEvent.Invoke();
		}
	}

	/// <summary>
	/// Does the actual loading of all the data and puts it into the game.
	/// </summary>
	public void Load() {
		if (saveFileData == null) {
			Debug.LogError("There's no file to read!");
			return;
		}

		// Read simple data
		currentTotalDays.value = simpleTotalDays[saveIndex.value].value;
		currentPlayTime.value = simplePlayTimes[saveIndex.value].value;

		// Read data in save file
		SaveData loadedData = saveFileData.saveFiles[saveIndex.value];
		currentMoney.value = loadedData.money;
		currentScrap.value = loadedData.scrap;

		// Read map data
		currentMission.value = missionLibrary.GetEntry(loadedData.mapData.missionString);
		mapIndex.value = loadedData.mapData.mapIndex;
		loadMapID.value = ((MissionEntry)currentMission.value).maps[mapIndex.value].uuid;
		squad1.values.Clear();
		for (int i = 0; i < loadedData.mapData.squad1.Count; i++) {
			squad1.values.Add(new PrepCharacter(loadedData.mapData.squad1[i]));
		}
		squad2.values.Clear();
		for (int i = 0; i < loadedData.mapData.squad2.Count; i++) {
			squad2.values.Add(new PrepCharacter(loadedData.mapData.squad2[i]));
		}

		// Read player data
		playerData.ResetData();
		for (int i = 0; i < loadedData.upgrade.Count; i++) {
			UpgradeEntry upgrade = (UpgradeEntry)upgradeLibrary.GetEntry(loadedData.upgrade[i].id);
			playerData.upgrader.upgrades.Add(new UpgradeItem(upgrade, loadedData.upgrade[i].researched));
		}
		for (int i = 0; i < loadedData.characters.Count; i++) {
			CharData cStats = (CharData)characterLibrary.GetEntry(loadedData.characters[i].id);
			CharClass cClass = (CharClass)classLibrary.GetEntry(loadedData.characters[i].currentClass);
			playerData.stats.Add(new StatsContainer(loadedData.characters[i], cStats, cClass));
			playerData.inventory.Add(new InventoryContainer(itemLibrary, loadedData.characters[i], playerData.upgrader));
			playerData.skills.Add(new SkillsContainer(skillLibrary, loadedData.characters[i]));
			playerData.baseInfo.Add(new SupportContainer(loadedData.characters[i]));
		}
		for (int i = 0; i < loadedData.items.Count; i++) {
			ItemEntry item = (ItemEntry)itemLibrary.GetEntry(loadedData.items[i].id);
			playerData.items.Add(new InventoryItem(item, loadedData.items[i].charges));
		}
		for (int i = 0; i < loadedData.missions.Count; i++) {
			playerData.missions.Add(new MissionProgress(loadedData.missions[i].id, loadedData.missions[i].cleared));
		}
		playerData.upgrader.CalculateResearch();
		Debug.Log("Successfully loaded the save data!");
		loadFinishedEvent.Invoke();
	}
}

[System.Serializable]
public class SavePackage {
	public int lastSaveFileIndex;
	public OptionPackage options = new OptionPackage();
	public SaveData[] saveFiles = new SaveData[SaveController.SAVE_FILES_COUNT];
}

public class OptionPackage {
	public int musicVolume = 30;
	public int sfxVolume = 10;
	public int gameSpeed = 5;
	public int controlScheme = 0;
	public bool useAnimations = true;
	public bool trueHit = true;
	public bool autoEnd = true;
	public bool autoSelectCharacter = true;
}

public class MapSavePackage {
	public string missionString = "";
	public int mapIndex = 0;
	public List<int> squad1 = new List<int>();
	public List<int> squad2 = new List<int>();
}

[System.Serializable]
public class SaveData {
	public string chapterName;
	public int totalDays;
	public int playTime;

	public int money;
	public int scrap;
	public MapSavePackage mapData;
	public List<CharacterSaveData> characters = new List<CharacterSaveData>();
	public List<ItemSaveData> items = new List<ItemSaveData>();
	public List<UpgradeSaveData> upgrade = new List<UpgradeSaveData>();
	public List<MissionSaveData> missions = new List<MissionSaveData>();
}
