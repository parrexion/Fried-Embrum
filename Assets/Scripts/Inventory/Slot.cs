using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour {

	public int slotID;
	public Image icon;
	public StatsContainer item;


	public void AddItem(StatsContainer charItem) {
		item = charItem;

		icon.sprite = item.portrait;
	}

	public void ClearSlot() {
		item = null;
		icon.sprite = null;
		icon.enabled = false;
	}
}
