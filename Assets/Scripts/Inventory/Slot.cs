using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour {

	public int slotID;
	public Image icon;
	public StatsContainer stats;
	public InventoryContainer inventory;


	public void AddItem(StatsContainer charStats, InventoryContainer charInv) {
		stats = charStats;
		inventory = charInv;

		icon.sprite = stats.charData.portrait;
	}

	public void ClearSlot() {
		stats = null;
		inventory = null;
		icon.sprite = null;
		icon.enabled = false;
	}
}
