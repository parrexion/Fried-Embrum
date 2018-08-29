using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
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
	
	public MapInfoVariable currentMap;
	public SaveListVariable availableUnits;
	public IntVariable saveIndex;

	[Header("Simple Data")]
	public IntVariable[] chapterIndex;
	public StringVariable[] levelNames;
	public IntVariable[] playTimes;

	[Header("Libraries")]
	public ItemLibrary itemLibrary;
	public CharacterLibrary characterLibrary;
	public ClassLibrary classLibrary;
	
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
		chapterIndex[saveIndex.value].value++;
		levelNames[saveIndex.value].value = (currentMap.value.nextLevel == null) ? "Game Cleared" : currentMap.value.name;
		playTimes[saveIndex.value].value += Random.Range(360,1240);

		// Setup save data
		SaveData data = new SaveData();
		data.chapterIndex = chapterIndex[saveIndex.value].value;
		data.levelName = levelNames[saveIndex.value].value;
		data.playTime = playTimes[saveIndex.value].value;
		for (int i = 0; i < availableUnits.stats.Length; i++) {
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

		XmlSerializer serializer = new XmlSerializer(typeof(SavePackage));
		FileStream file = File.Open(path,FileMode.Open);
		saveFileData = serializer.Deserialize(file) as SavePackage;
		file.Close();

		if (saveFileData == null) {
			Debug.LogError("Could not open the file: " + path);
			return;
		}

		for (int i = 0; i < saveFileData.saveFiles.Length; i++) {
			if (saveFileData.saveFiles[i] == null)
				continue;
			chapterIndex[i].value = saveFileData.saveFiles[i].chapterIndex;
			levelNames[i].value = saveFileData.saveFiles[i].levelName;
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
		
		// Read data in save file
		SaveData loadedData = saveFileData.saveFiles[saveIndex.value];
		Debug.Log("Characters:  " + loadedData.characters.Count);
		availableUnits.stats = new StatsContainer[loadedData.characters.Count];
		availableUnits.inventory = new InventoryContainer[loadedData.characters.Count];
		availableUnits.skills = new SkillsContainer[loadedData.characters.Count];
		for (int i = 0; i < loadedData.characters.Count; i++) {
			CharData cStats = characterLibrary.GetEntry(loadedData.characters[i].id);
			CharClass cClass = classLibrary.GetEntry(loadedData.characters[i].classID);
			availableUnits.stats[i] = new StatsContainer(itemLibrary, loadedData.characters[i], cStats, cClass);
			availableUnits.inventory[i] = new InventoryContainer(itemLibrary, loadedData.characters[i]);
			availableUnits.skills[i] = new SkillsContainer(itemLibrary, loadedData.characters[i]);
			Debug.Log("Done loading " + cStats.charName);
		}
		Debug.Log("Successfully loaded the save data!");
	}
}

[System.Serializable]
public class SavePackage {
	public SaveData[] saveFiles = new SaveData[SaveController.SAVE_FILES_COUNT];
}

[System.Serializable]
public class SaveData {
	public int chapterIndex;
	public string levelName;
	public int playTime;
	public List<CharacterSaveData> characters = new List<CharacterSaveData>();
}
