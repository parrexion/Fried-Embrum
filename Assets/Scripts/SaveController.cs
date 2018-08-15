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

	public ItemLibrary itemLibrary;
	public CharacterLibrary characterLibrary;
	public ClassLibrary classLibrary;
	
	public SaveListVariable availableUnits;
	
	private string _savePath = "";


	private void Initialize() {
		_savePath = Application.persistentDataPath+"/saveData.xml";
		Load();
//		Save();
	}

	public void Save() {
		// Setup save data
		SaveData data = new SaveData();
		for (int i = 0; i < availableUnits.stats.Length; i++) {
			if (availableUnits.stats[i].charData == null)
				continue;
			CharacterSaveData c = new CharacterSaveData();
			c.StoreData(availableUnits.stats[i], availableUnits.inventory[i]);
			data.characters.Add(c);
		}
		
		//Write to file
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() { Indent = true };
		XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
		using (XmlWriter xmlWriter = XmlWriter.Create(_savePath, xmlWriterSettings)) {
			serializer.Serialize(xmlWriter, data);
		}
		Debug.Log("Successfully saved the save data!");
	}

	public void Load() {
		//Read save data file
		if (File.Exists(_savePath)){
			XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
			FileStream file = File.Open(_savePath,FileMode.Open);
			SaveData loadedData = serializer.Deserialize(file) as SaveData;
			file.Close();

			if (loadedData == null) {
				Debug.LogWarning("Could not open the file: " + _savePath);
				Save();
				return;
			}
			
			// Read data in save file
			Debug.Log("Characters:  " + loadedData.characters.Count);
			availableUnits.stats = new StatsContainer[loadedData.characters.Count];
			availableUnits.inventory = new InventoryContainer[loadedData.characters.Count];
			for (int i = 0; i < loadedData.characters.Count; i++) {
				CharacterStats cStats = characterLibrary.GetEntry(loadedData.characters[i].id);
				CharClass cClass = classLibrary.GetEntry(loadedData.characters[i].classID);
				availableUnits.stats[i] = new StatsContainer(itemLibrary, loadedData.characters[i], cStats, cClass);
				availableUnits.inventory[i] = new InventoryContainer(itemLibrary, loadedData.characters[i]);
			}
			Debug.Log("Successfully loaded the save data!");
		}
		else {
			Debug.LogWarning("Could not open the file: " + _savePath);
			Save();
		}
	}
}

[System.Serializable]
public class SaveData {

	public List<CharacterSaveData> characters = new List<CharacterSaveData>();

}
