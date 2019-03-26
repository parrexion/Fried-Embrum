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
	
	public ScrObjEntryReference currentMap;
	public SaveListVariable playerData;
	public IntVariable saveIndex;

	[Header("Simple Data")]
	public IntVariable[] simpleChapterIndex;
	public IntVariable[] simplePlayTimes;

	[Header("Current Data")]
	public IntVariable currentChapterIndex;
	public IntVariable currentPlayTime;
	public IntVariable currentMoney;
	public IntVariable currentScrap;
	public IntVariable currentBexp;

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

	public UnityEvent loadFinishedEvent;
	
	private string _savePath = "";
	private string _backupSavePath = "";
	private SavePackage saveFileData;


	private void Initialize() {
		_savePath = Application.persistentDataPath+"/saveData2.xml";
		_backupSavePath = Application.streamingAssetsPath+"/saveData2.xml";
		PreLoad();
		// EmptySave();
	}

	public void EmptySave() {
		// Setup save data
		saveFileData = new SavePackage {
			musicVolume = 10,
			sfxVolume = 10,
			gameSpeed = 5,
			useAnimations = true,
			trueHit = true,
			autoEnd = true
		};
		for (int i = 0; i < SAVE_FILES_COUNT; i++) {
			SaveData data = new SaveData(){chapterIndex = 1, playTime = 0};
			saveFileData.saveFiles[i] = data;
		}

		//Write to file
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
		XmlSerializer serializer = new XmlSerializer(typeof(SavePackage));
		using (XmlWriter xmlWriter = XmlWriter.Create(_savePath, xmlWriterSettings)) {
			serializer.Serialize(xmlWriter, saveFileData);
		}
		Debug.Log("Successfully created a new save file!\n" + _savePath);
	}

	public void Save(bool onlyOptions) {
		// Options
		saveFileData.musicVolume = musicVolume.value;
		saveFileData.sfxVolume = sfxVolume.value;
		saveFileData.gameSpeed = gameSpeed.value;
		saveFileData.useAnimations = useAnimations.value;
		saveFileData.trueHit = trueHit.value;
		saveFileData.autoEnd = autoEnd.value;

		if (!onlyOptions) {
			// Update data
			simpleChapterIndex[saveIndex.value].value = currentChapterIndex.value;
			simplePlayTimes[saveIndex.value].value = currentPlayTime.value;

			// Setup save data
			SaveData data = new SaveData {
				chapterIndex = simpleChapterIndex[saveIndex.value].value,
				playTime = simplePlayTimes[saveIndex.value].value,
				money = currentMoney.value,
				scrap = currentScrap.value,
				bexp = currentBexp.value
			};
			for (int i = 0; i < playerData.stats.Count; i++) {
				if (playerData.stats[i].charData == null)
					continue;
				CharacterSaveData c = new CharacterSaveData();
				c.StoreData(playerData.stats[i], playerData.inventory[i], playerData.skills[i]);
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
		Debug.Log("Successfully saved the save data!\n" + _savePath);
	}

	/// <summary>
	/// Does a pre load which is used to show which save files are used to
	/// give the player a sense of which save file to load.
	/// </summary>
	public void PreLoad() {
		string path = _savePath;
		if (!File.Exists(_savePath)){
			path = _backupSavePath;
			Debug.LogWarning("No save file found: " + path);
			EmptySave();
		}
		else {
			XmlSerializer serializer = new XmlSerializer(typeof(SavePackage));
			FileStream file = File.Open(path,FileMode.Open);
			saveFileData = serializer.Deserialize(file) as SavePackage;
			file.Close();
		}

		if (saveFileData == null) {
			Debug.LogError("Could not open the file: " + path);
			return;
		}

		// Options
		musicVolume.value = saveFileData.musicVolume;
		sfxVolume.value = saveFileData.sfxVolume;
		gameSpeed.value = saveFileData.gameSpeed;
		useAnimations.value = saveFileData.useAnimations;
		trueHit.value = saveFileData.trueHit;
		autoEnd.value = saveFileData.autoEnd;

		//Save files
		for (int i = 0; i < saveFileData.saveFiles.Length; i++) {
			if (saveFileData.saveFiles[i] == null)
				continue;
			simpleChapterIndex[i].value = saveFileData.saveFiles[i].chapterIndex;
			simplePlayTimes[i].value = saveFileData.saveFiles[i].playTime;
		}
		
		Debug.Log("Successfully pre-loaded the save data!");

		if (SceneManager.GetActiveScene().name == "BattleScene" || SceneManager.GetActiveScene().name == "BaseScene") {
			Load();
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
		
		// Set basic data
		currentChapterIndex.value = simpleChapterIndex[saveIndex.value].value;
		currentPlayTime.value = simplePlayTimes[saveIndex.value].value;

		// Read data in save file
		SaveData loadedData = saveFileData.saveFiles[saveIndex.value];
		currentMoney.value = loadedData.money;
		currentScrap.value = loadedData.scrap;
		currentBexp.value = loadedData.bexp;
		
		playerData.ResetData();
		for (int i = 0; i < loadedData.characters.Count; i++) {
			CharData cStats = (CharData)characterLibrary.GetEntry(loadedData.characters[i].id);
			CharClass cClass = (CharClass)classLibrary.GetEntry(loadedData.characters[i].classID);
			playerData.stats.Add(new StatsContainer(loadedData.characters[i], cStats, cClass));
			playerData.inventory.Add(new InventoryContainer(itemLibrary, loadedData.characters[i]));
			playerData.skills.Add(new SkillsContainer(skillLibrary, loadedData.characters[i]));
		}
		Debug.Log("Successfully loaded " + loadedData.characters.Count + " characters");
		for (int i = 0; i < loadedData.items.Count; i++) {
			ItemEntry item = (ItemEntry)itemLibrary.GetEntry(loadedData.items[i].id);
			playerData.items.Add(new InventoryItem { item = item, charges = loadedData.items[i].charges });
		}
		for (int i = 0; i < loadedData.upgrade.Count; i++) {
			UpgradeEntry upgrade = (UpgradeEntry)upgradeLibrary.GetEntry(loadedData.upgrade[i].id);
			playerData.upgrader.upgrades.Add(new UpgradeItem { upgrade = upgrade, researched = loadedData.upgrade[i].researched });
		}
		for (int i = 0; i < loadedData.missions.Count; i++) {
			MapEntry map = (MapEntry)missionLibrary.GetEntry(loadedData.missions[i].id);
			playerData.missions.Add(new MissionContainer { map = map, cleared = loadedData.missions[i].cleared });
		}
		playerData.upgrader.CalculateResearch();
		Debug.Log("Successfully loaded the save data!");
		loadFinishedEvent.Invoke();
	}
}

[System.Serializable]
public class SavePackage {
	public int musicVolume;
	public int sfxVolume;
	public int gameSpeed;
	public bool useAnimations;
	public bool trueHit;
	public bool autoEnd;
	public SaveData[] saveFiles = new SaveData[SaveController.SAVE_FILES_COUNT];
}

[System.Serializable]
public class SaveData {
	public int chapterIndex;
	public string levelName;
	public int playTime;
	public int money;
	public int scrap;
	public int bexp;
	public List<CharacterSaveData> characters = new List<CharacterSaveData>();
	public List<ItemSaveData> items = new List<ItemSaveData>();
	public List<UpgradeSaveData> upgrade = new List<UpgradeSaveData>();
	public List<MissionSaveData> missions = new List<MissionSaveData>();
}
