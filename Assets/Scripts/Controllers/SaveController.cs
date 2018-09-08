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
	public SaveListVariable availableUnits;
	public IntVariable saveIndex;

	[Header("Current Data")]
	public IntVariable currentChapterIndex;
	public IntVariable currentPlayTime;

	[Header("Simple Data")]
	public IntVariable[] chapterIndex;
	public IntVariable[] playTimes;

	[Header("Libraries")]
	public ScrObjLibraryVariable itemLibrary;
	public ScrObjLibraryVariable skillLibrary;
	public ScrObjLibraryVariable characterLibrary;
	public ScrObjLibraryVariable classLibrary;

	[Header("Options")]
	public IntVariable musicVolume;
	public IntVariable sfxVolume;
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
		saveIndex.value = 0;
		PreLoad();
		// EmptySave();
	}

	public void EmptySave() {
		saveFileData = new SavePackage();

		// Setup save data
		saveFileData.musicVolume = 10;
		saveFileData.sfxVolume = 10;
		saveFileData.useAnimations = true;
		saveFileData.trueHit = true;
		saveFileData.autoEnd = true;
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
		Debug.Log("Successfully saved new save data!\n" + _savePath);
	}

	public void Save() {
		// Update data
		chapterIndex[saveIndex.value].value = currentChapterIndex.value;
		playTimes[saveIndex.value].value = currentPlayTime.value;

		// Options
		saveFileData.musicVolume = musicVolume.value;
		saveFileData.sfxVolume = sfxVolume.value;
		saveFileData.useAnimations = useAnimations.value;
		saveFileData.trueHit = trueHit.value;
		saveFileData.autoEnd = autoEnd.value;

		// Setup save data
		SaveData data = new SaveData();
		data.chapterIndex = chapterIndex[saveIndex.value].value;
		data.playTime = playTimes[saveIndex.value].value;
		for (int i = 0; i < availableUnits.stats.Count; i++) {
			if (availableUnits.stats[i].charData == null)
				continue;
			CharacterSaveData c = new CharacterSaveData();
			c.StoreData(availableUnits.stats[i], availableUnits.inventory[i], availableUnits.skills[i]);
			data.characters.Add(c);
		}
		saveFileData.saveFiles[saveIndex.value] = data;
		
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
		useAnimations.value = saveFileData.useAnimations;
		trueHit.value = saveFileData.trueHit;
		autoEnd.value = saveFileData.autoEnd;

		//Save files
		for (int i = 0; i < saveFileData.saveFiles.Length; i++) {
			if (saveFileData.saveFiles[i] == null)
				continue;
			chapterIndex[i].value = saveFileData.saveFiles[i].chapterIndex;
			playTimes[i].value = saveFileData.saveFiles[i].playTime;
		}
		
		Debug.Log("Successfully pre-loaded the save data!");

		if (SceneManager.GetActiveScene().name == "BattleScene") {
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
		currentChapterIndex.value = chapterIndex[saveIndex.value].value;
		currentPlayTime.value = playTimes[saveIndex.value].value;

		// Read data in save file
		SaveData loadedData = saveFileData.saveFiles[saveIndex.value];
		Debug.Log("Characters:  " + loadedData.characters.Count);
		availableUnits.stats = new List<StatsContainer>();
		availableUnits.inventory = new List<InventoryContainer>();
		availableUnits.skills = new List<SkillsContainer>();
		for (int i = 0; i < loadedData.characters.Count; i++) {
			CharData cStats = (CharData)characterLibrary.GetEntry(loadedData.characters[i].id);
			CharClass cClass = (CharClass)classLibrary.GetEntry(loadedData.characters[i].classID);
			availableUnits.stats.Add(new StatsContainer(loadedData.characters[i], cStats, cClass));
			availableUnits.inventory.Add(new InventoryContainer(itemLibrary, loadedData.characters[i]));
			availableUnits.skills.Add(new SkillsContainer(skillLibrary, loadedData.characters[i]));
			Debug.Log("Done loading " + cStats.entryName);
		}
		Debug.Log("Successfully loaded the save data!");
		loadFinishedEvent.Invoke();
	}
}

[System.Serializable]
public class SavePackage {
	public int musicVolume;
	public int sfxVolume;
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
	public List<CharacterSaveData> characters = new List<CharacterSaveData>();
}
