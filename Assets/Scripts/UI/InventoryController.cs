using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {

	public TacticsMoveVariable selectedCharacter;
	public IntVariable selectedItem;
	
	[Header("Inventory Menu")]
	public GameObject inventoryMenuObject;
	public Button equipButton;
	public Button useButton;
	public Button dropButton;

	[Header("Tooltip")]
	public GameObject tooltipObject;
	public Text tooltipText;
	public StringVariable tooltipMessage;

	public UnityEvent showTooltipEvent;
	public UnityEvent inventoryChangedEvent;

	private StatsContainer _stats;
	private bool _tooltipActive;
	
	
	private void Start () {
		tooltipObject.SetActive(false);
		inventoryMenuObject.SetActive(false);
	}

	public void SelectItem() {
		inventoryMenuObject.SetActive(false);
		_stats = selectedCharacter.value.stats;
		TooltipType position = (TooltipType) selectedItem.value;
		WeaponItem item;
		switch (position) {
			case TooltipType.NONE:
				break;
			case TooltipType.NAME:
				break;
			case TooltipType.CLASS:
				break;
			case TooltipType.INV1:
				item = _stats.GetItem(0);
				tooltipMessage.value = (item != null) ? _stats.inventory[0].item.description : "";
				inventoryMenuObject.SetActive(item != null);
				if (item != null) {
					equipButton.interactable = (item.itemCategory == ItemCategory.WEAPON);
					useButton.interactable = (item.itemCategory == ItemCategory.CONSUME);
					dropButton.interactable = true;
				}
				break;
			case TooltipType.INV2:
				item = _stats.GetItem(1);
				tooltipMessage.value = (item != null) ? _stats.inventory[1].item.description : "";
				inventoryMenuObject.SetActive(item != null);
				if (item != null) {
					equipButton.interactable = (item.itemCategory == ItemCategory.WEAPON);
					useButton.interactable = (item.itemCategory == ItemCategory.CONSUME);
					dropButton.interactable = true;
				}
				break;
			case TooltipType.INV3:
				item = _stats.GetItem(2);
				tooltipMessage.value = (item != null) ? _stats.inventory[2].item.description : "";
				inventoryMenuObject.SetActive(item != null);
				if (item != null) {
					equipButton.interactable = (item.itemCategory == ItemCategory.WEAPON);
					useButton.interactable = (item.itemCategory == ItemCategory.CONSUME);
					dropButton.interactable = true;
				}
				break;
			case TooltipType.INV4:
				item = _stats.GetItem(3);
				tooltipMessage.value = (item != null) ? _stats.inventory[3].item.description : "";
				inventoryMenuObject.SetActive(item != null);
				if (item != null) {
					equipButton.interactable = (item.itemCategory == ItemCategory.WEAPON);
					useButton.interactable = (item.itemCategory == ItemCategory.CONSUME);
					dropButton.interactable = true;
				}
				break;
			case TooltipType.INV5:
				item = _stats.GetItem(4);
				tooltipMessage.value = (item != null) ? _stats.inventory[4].item.description : "";
				inventoryMenuObject.SetActive(item != null);
				if (item != null) {
					equipButton.interactable = (item.itemCategory == ItemCategory.WEAPON);
					useButton.interactable = (item.itemCategory == ItemCategory.CONSUME);
					dropButton.interactable = true;
				}
				break;
			case TooltipType.SKL1:
				break;
			case TooltipType.SKL2:
				break;
			case TooltipType.SKL3:
				break;
			case TooltipType.SKL4:
				break;
			case TooltipType.SKL5:
				break;
			default:
				Debug.LogError("WTF!?");
				break;
		}
		
		ShowTooltip();
	}

	public void EquipItem() {
		TooltipType position = (TooltipType) selectedItem.value;
		int index = 0;
		switch (position) {
			case TooltipType.INV1:
				index = 0;
				break;
			case TooltipType.INV2:
				index = 1;
				break;
			case TooltipType.INV3:
				index = 2;
				break;
			case TooltipType.INV4:
				index = 3;
				break;
			case TooltipType.INV5:
				index = 4;
				break;
		}
		_stats.EquipItem(index);
		selectedItem.value = (int) TooltipType.INV1;
		showTooltipEvent.Invoke();
		inventoryChangedEvent.Invoke();
	}

	public void UseItem() {
		TooltipType position = (TooltipType) selectedItem.value;
		int index = 0;
		switch (position) {
			case TooltipType.INV1:
				index = 0;
				break;
			case TooltipType.INV2:
				index = 1;
				break;
			case TooltipType.INV3:
				index = 2;
				break;
			case TooltipType.INV4:
				index = 3;
				break;
			case TooltipType.INV5:
				index = 4;
				break;
		}
		_stats.UseItem(index, selectedCharacter.value);
		inventoryChangedEvent.Invoke();
	}

	private void ShowTooltip() {
		if (string.IsNullOrEmpty(tooltipMessage.value)) {
			tooltipObject.SetActive(false);
			return;
		}
		tooltipObject.SetActive(true);
		tooltipText.text = tooltipMessage.value;
	}

	public void HideTooltip() {
		tooltipObject.SetActive(false);
		inventoryMenuObject.SetActive(false);
	}
}
