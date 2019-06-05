using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SupportContainer {
	
	public int roomNo;
	public List<SupportValue> supportValues = new List<SupportValue>();


	public SupportContainer(CharacterSaveData saveData) {
		if (saveData == null) {
			roomNo = -1;
			supportValues = new List<SupportValue>();
			return;
		}

		roomNo = saveData.roomNo;

		supportValues = new List<SupportValue>();
		for (int i = 0; i < saveData.supports.Count; i++) {
			supportValues.Add(saveData.supports[i]);
			//Debug.Log("Added support value " + supportValues[i].uuid + " = " + supportValues[i].value);
		}
	}
	
	public SupportValue GetSupportValue(CharData other) {
		for (int i = 0; i < supportValues.Count; i++) {
			if (supportValues[i].uuid == other.uuid)
				return supportValues[i];
		}
		return new SupportValue(){ uuid = other.uuid };
	}
}
