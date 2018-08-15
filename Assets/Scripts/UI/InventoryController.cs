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
	public UnityEvent hideTooltipEvent;
	public UnityEvent inventoryChangedEvent;

	private StatsContainer _stats;
	private InventoryContainer _inventory;
	private bool _tooltipActive;
	
	
	private void Start () {
		tooltipObject.SetActive(false);
		inventoryMenuObject.SetActive(false);
	}

	public void SelectItem() {
		inventoryMenuObject.SetActive(false);
		_stats = selectedCharacter.value.stats;
		_inventory = selectedCharacter.value.inventory;
		TooltipType position = (TooltipType) selectedItem.value;
		Item item = null;
		int skill = 0;
		switch (position) {
			case TooltipType.NONE:
				break;
			case TooltipType.NAME:
				break;
			case TooltipType.CLASS:
				break;
			case TooltipType.INV1:
				item = _inventory.GetItem(0).item;
				skill = _stats.GetWpnSkill((WeaponItem)item);
				tooltipMessage.value = (item != null) ? _inventory.inventory[0].item.description : "";
				inventoryMenuObject.SetActive(item != null);
				if (item != null) {
					equipButton.interactable = (((WeaponItem)(item)).itemCategory == ItemCategory.WEAPON && ((WeaponItem)(item)).CanUse(skill));
					useButton.interactable = (((WeaponItem)(item)).itemCategory == ItemCategory.CONSUME);
					dropButton.interactable = true;
				}
				break;
			case TooltipType.INV2:
				item = _inventory.GetItem(1).item;
				skill = _stats.GetWpnSkill((WeaponItem)item);
				tooltipMessage.value = (item != null) ? _inventory.inventory[1].item.description : "";
				inventoryMenuObject.SetActive(item != null);
				if (item != null) {
					equipButton.interactable = (((WeaponItem)(item)).itemCategory == ItemCategory.WEAPON && ((WeaponItem)(item)).CanUse(skill));
					useButton.interactable = (((WeaponItem)(item)).itemCategory == ItemCategory.CONSUME);
					dropButton.interactable = true;
				}
				break;
			case TooltipType.INV3:
				item = _inventory.GetItem(2).item;
				skill = _stats.GetWpnSkill((WeaponItem)item);
				tooltipMessage.value = (item != null) ? _inventory.inventory[2].item.description : "";
				inventoryMenuObject.SetActive(item != null);
				if (item != null) {
					equipButton.interactable = (((WeaponItem)(item)).itemCategory == ItemCategory.WEAPON && ((WeaponItem)(item)).CanUse(skill));
					useButton.interactable = (((WeaponItem)(item)).itemCategory == ItemCategory.CONSUME);
					dropButton.interactable = true;
				}
				break;
			case TooltipType.INV4:
				item = _inventory.GetItem(3).item;
				skill = _stats.GetWpnSkill((WeaponItem)item);
				tooltipMessage.value = (item != null) ? _inventory.inventory[3].item.description : "";
				inventoryMenuObject.SetActive(item != null);
				if (item != null) {
					equipButton.interactable = (((WeaponItem)(item)).itemCategory == ItemCategory.WEAPON && ((WeaponItem)(item)).CanUse(skill));
					useButton.interactable = (((WeaponItem)(item)).itemCategory == ItemCategory.CONSUME);
					dropButton.interactable = true;
				}
				break;
			case TooltipType.INV5:
				item = _inventory.GetItem(4).item;
				skill = _stats.GetWpnSkill((WeaponItem)item);
				tooltipMessage.value = (item != null) ? _inventory.inventory[4].item.description : "";
				inventoryMenuObject.SetActive(item != null);
				if (item != null) {
					equipButton.interactable = (((WeaponItem)(item)).itemCategory == ItemCategory.WEAPON && ((WeaponItem)(item)).CanUse(skill));
					useButton.interactable = (((WeaponItem)(item)).itemCategory == ItemCategory.CONSUME);
					dropButton.interactable = true;
				}
				break;
			case TooltipType.SKL1:
				tooltipMessage.value = (_stats.skills[0]) ? _stats.skills[0].description : "";
				break;
			case TooltipType.SKL2:
				tooltipMessage.value = (_stats.skills[1]) ? _stats.skills[1].description : "";
				break;
			case TooltipType.SKL3:
				tooltipMessage.value = (_stats.skills[2]) ? _stats.skills[2].description : "";
				break;
			case TooltipType.SKL4:
				tooltipMessage.value = (_stats.skills[3]) ? _stats.skills[3].description : "";
				break;
			case TooltipType.SKL5:
				tooltipMessage.value = (_stats.skills[4]) ? _stats.skills[4].description : "";
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
		_inventory.EquipItem(index);
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
		_inventory.UseItem(index, selectedCharacter.value);
		inventoryChangedEvent.Invoke();
	}

	public void DropItem() {
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
		_inventory.DropItem(index);
		inventoryChangedEvent.Invoke();
		showTooltipEvent.Invoke();
	}

	private void ShowTooltip() {
		if (string.IsNullOrEmpty(tooltipMessage.value)) {
			hideTooltipEvent.Invoke();
			return;
		}
		tooltipObject.SetActive(true);
		tooltipText.text = tooltipMessage.value;
	}

	public void HideTooltip() {
		selectedItem.value = -1;
		tooltipObject.SetActive(false);
		inventoryMenuObject.SetActive(false);
	}
}
