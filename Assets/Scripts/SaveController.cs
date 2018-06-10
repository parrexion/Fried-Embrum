using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class SaveController : MonoBehaviour {

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

	public static int highestID;

	public IntVariable currentOrbs;
	public SaveListVariable equippedUnits;
	public SaveListVariable availableUnits;
	public SaveListVariable enemyUnits;

	public CharacterStats[] charList;
	
	private string _savePath = "";


	private void Initialize() {
		_savePath = Application.persistentDataPath+"/saveData.xml";
		Load();
//		Save();
	}

	public void Save() {
//		UpdateData();
		SaveData data = new SaveData {currentObrs = currentOrbs.value};
		data.StoreData(equippedUnits.values, availableUnits.values);
		
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
		XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
		using (XmlWriter xmlWriter = XmlWriter.Create(_savePath, xmlWriterSettings)) {
			serializer.Serialize(xmlWriter, data);
		}
		Debug.Log("Successfully saved the save data!");
	}

	public void Load() {
		//Load save data
		if (File.Exists(_savePath)){
			XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
			FileStream file = File.Open(_savePath,FileMode.Open);
			SaveData loadedData = serializer.Deserialize(file) as SaveData;
			file.Close();

			if (loadedData == null) {
				Debug.LogWarning("Could not open the file: " + _savePath);
				Save();
			}
			else {
				currentOrbs.value = loadedData.currentObrs;
				availableUnits.values = loadedData.ReadData(charList);
				Debug.Log("Successfully loaded the save data!");
			}
		}
		else {
			Debug.LogWarning("Could not open the file: " + _savePath);
			Save();
		}
	}
}

[System.Serializable]
public class SaveData {

	public int currentObrs;
	public List<CharacterSave> characters = new List<CharacterSave>();
	
	public void StoreData(StatsContainer[] data, StatsContainer[] data2) {
		characters.Clear();
		for (int i = 0; i < data.Length; i++) {
			if (data[i].id == -1)
				continue;
			CharacterSave c = new CharacterSave();
			c.LoadData(data[i]);
			characters.Add(c);
		}
		for (int i = 0; i < data2.Length; i++) {
			if (data2[i].id == -1)
				continue;
			CharacterSave c = new CharacterSave();
			c.LoadData(data2[i]);
			characters.Add(c);
		}

		while (characters.Count < ConstValues.AVAILABLE_SIZE) {
			characters.Add(new CharacterSave());
		}
	}

	public StatsContainer[] ReadData(CharacterStats[] charList) {
		StatsContainer[] stats = new StatsContainer[characters.Count];
		for (int i = 0; i < characters.Count; i++) {
			CharacterStats cStats = (characters[i].statsID != -1) ? charList[characters[i].statsID] : null;
			stats[i] = new StatsContainer(characters[i], cStats);
			SaveController.highestID = Mathf.Max(SaveController.highestID, stats[i].statsID);
		}

		return stats;
	}
}
