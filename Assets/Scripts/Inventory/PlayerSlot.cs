using UnityEngine;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour {

	public int slotID;
//	public Image icon;
	public TacticsMove tactics;


	public void AddItem(TacticsMove charItem) {
		tactics = charItem;
//		icon.sprite = tactics.stats.portrait;
	}

	public void ClearSlot() {
		tactics = null;
//		icon.sprite = null;
//		icon.enabled = false;
	}
}
