using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryMenuController : InputReceiverDelegate {

	public TacticsMoveVariable selectedCharacter;
	public ActionModeVariable currentMode;

	[Header("Extra sounds")]
	public AudioQueueVariable sfxQueue;
	public SfxEntry healItemSfx;
	public SfxEntry boostItemSfx;
	
	[Header("Inventory Menu")]
	public GameObject inventoryMenu;
	public IntVariable inventoryIndex;
	public IntVariable itemMenuPosition;

	public MyButtonList inventoryButtons;
	private bool selectMode;

	public UnityEvent inventoryChangedEvent;
	public UnityEvent playSfxEvent;
	
	
	private void Start () {
		inventoryMenu.SetActive(false);
		inventoryButtons.ResetButtons();
	}

    public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.INV);
		inventoryMenu.SetActive(active);
		if (active) {
			inventoryIndex.value = 0;
			inventoryChangedEvent.Invoke();
		}
    }

	private void ButtonSetup() {
		inventoryButtons.ResetButtons();
		InventoryTuple tuple = selectedCharacter.value.inventory.GetTuple(inventoryIndex.value);
		int skill = selectedCharacter.value.stats.GetWpnSkill(tuple.item);
		if (tuple.item?.itemCategory == ItemCategory.WEAPON && tuple.item.CanUse(skill)) {
			inventoryButtons.AddButton("EQUIP", 0);
		}
		else if (tuple.item?.itemCategory == ItemCategory.CONSUME) {
			inventoryButtons.AddButton("USE", 1);
		}
		inventoryButtons.AddButton("DROP", 2);
		
		itemMenuPosition.value = inventoryButtons.GetPosition();
	}

    public override void OnUpArrow() {
		if (!selectMode) {
			inventoryIndex.value = OPMath.FullLoop(0, InventoryContainer.INVENTORY_SIZE, inventoryIndex.value -1);
			inventoryChangedEvent.Invoke();
		}
		else {
			itemMenuPosition.value = inventoryButtons.Move(-1);
			menuMoveEvent.Invoke();
		}
    }

    public override void OnDownArrow() {
		if (!selectMode) {
			inventoryIndex.value = OPMath.FullLoop(0, InventoryContainer.INVENTORY_SIZE, inventoryIndex.value +1);
			inventoryChangedEvent.Invoke();
		}
		else {
			itemMenuPosition.value = inventoryButtons.Move(1);
			menuMoveEvent.Invoke();
		}
    }

    public override void OnOkButton() {
		if (!selectMode) {
			InventoryTuple tuple = selectedCharacter.value.inventory.GetTuple(inventoryIndex.value);
			if (tuple.item != null) {
				selectMode = true;
				ButtonSetup();
				menuAcceptEvent.Invoke();
			}
		}
		else {
			switch (inventoryButtons.GetValue())
			{
				case 0:
					EquipItem();
					break;
				case 1:
					UseItem();
					break;
				case 2:
					DropItem();
					break;
			}
			menuAcceptEvent.Invoke();
		}
    }

    public override void OnBackButton() {
		if (!selectMode) {
			menuBackEvent.Invoke();
			InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
			inventoryIndex.value = -1;
		}
		else {
			selectMode = false;
			inventoryButtons.ResetButtons();
		}
    }

	/// <summary>
	/// Equips the selected item and moves the cursor to the new position.
	/// </summary>
	private void EquipItem() {
		selectMode = false;
		inventoryButtons.ResetButtons();
		selectedCharacter.value.inventory.EquipItem(inventoryIndex.value);
		inventoryIndex.value = 0;
		inventoryChangedEvent.Invoke();
	}

	/// <summary>
	/// Uses the selected item.
	/// </summary>
	private void UseItem() {
		selectMode = false;
		inventoryButtons.ResetButtons();
		InventoryTuple tup = selectedCharacter.value.inventory.GetTuple(inventoryIndex.value);
		SfxEntry sfx = (tup.item.itemType == ItemType.CHEAL) ? healItemSfx : boostItemSfx;
		sfxQueue.Enqueue(sfx);
		playSfxEvent.Invoke();

		selectedCharacter.value.inventory.UseItem(inventoryIndex.value, selectedCharacter.value);
		inventoryIndex.value = -1;
		currentMode.value = ActionMode.NONE;
		InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
		selectedCharacter.value.End();
		inventoryChangedEvent.Invoke();
	}

	/// <summary>
	/// Drops the selected item.
	/// </summary>
	private void DropItem() {
		selectMode = false;
		inventoryButtons.ResetButtons();
		selectedCharacter.value.inventory.DropItem(inventoryIndex.value, selectedCharacter.value.stats);
		inventoryChangedEvent.Invoke();
	}


    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }
    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
    public override void OnStartButton() { }
}
