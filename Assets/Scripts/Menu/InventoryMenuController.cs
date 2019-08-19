using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryMenuController : InputReceiverDelegate {

	public TacticsMoveVariable selectedCharacter;
	public ActionModeVariable currentMode;
	
	[Header("Inventory Menu")]
	public GameObject inventoryMenu;
	public IntVariable inventoryIndex;
	public IntVariable itemMenuPosition;

	public MyPrompt prompt;
	private bool popupMode;
	public MyButtonList inventoryButtons;
	private bool selectMode;

	[Header("Extra sounds")]
	public AudioQueueVariable sfxQueue;
	public SfxEntry healItemSfx;
	public SfxEntry boostItemSfx;

	[Header("Events")]
	public UnityEvent inventoryChangedEvent;
	public UnityEvent playSfxEvent;
	
	
	private void Start () {
		inventoryMenu.SetActive(false);
		inventoryButtons.ResetButtons();
		inventoryIndex.value = -1;
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
		WeaponRank skill = selectedCharacter.value.inventory.GetWpnSkill(tuple);
		if (tuple.itemCategory == ItemCategory.WEAPON && tuple.CanUse(skill)) {
			inventoryButtons.AddButton("EQUIP", 0);
		}
		else if (tuple.itemCategory == ItemCategory.CONSUME) {
			inventoryButtons.AddButton("USE", 1);
		}
		inventoryButtons.AddButton("DROP", 2);
		
		itemMenuPosition.value = inventoryButtons.GetPosition();
	}

    public override void OnUpArrow() {
		if (!selectMode) {
			inventoryIndex.value = OPMath.FullLoop(0, InventoryContainer.INVENTORY_SIZE, inventoryIndex.value -1);
			inventoryChangedEvent.Invoke();
			menuMoveEvent.Invoke();
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
			menuMoveEvent.Invoke();
		}
		else {
			itemMenuPosition.value = inventoryButtons.Move(1);
			menuMoveEvent.Invoke();
		}
    }

    public override void OnOkButton() {
		if (popupMode) {
			popupMode = false;
			MyPrompt.Result res = prompt.Click(true);
			if (res == MyPrompt.Result.OK1) {
				DropItem();
				menuAcceptEvent.Invoke();
			}
			else {
				menuBackEvent.Invoke();
			}
		}
		else if (!selectMode) {
			InventoryTuple tuple = selectedCharacter.value.inventory.GetTuple(inventoryIndex.value);
			if (!string.IsNullOrEmpty(tuple.uuid)) {
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
					InventoryTuple tup = selectedCharacter.value.inventory.GetTuple(inventoryIndex.value);
					prompt.ShowYesNoPopup("Drop " + tup.entryName + "?", false);
					popupMode = true;
					break;
			}
			menuAcceptEvent.Invoke();
		}
    }

    public override void OnBackButton() {
		if (popupMode) {
			popupMode = false;
			MyPrompt.Result res = prompt.Click(false);
		}
		else if (!selectMode) {
			InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
			inventoryIndex.value = -1;
		}
		else {
			selectMode = false;
			inventoryButtons.ResetButtons();
		}
		menuBackEvent.Invoke();
    }

	public override void OnLeftArrow() {
		if(popupMode)
			prompt.Move(-1);
	}

	public override void OnRightArrow() {
		if(popupMode)
			prompt.Move(1);
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
		SfxEntry sfx = (tup.weaponType == WeaponType.C_HEAL) ? healItemSfx : boostItemSfx;
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
		selectedCharacter.value.inventory.DropItem(inventoryIndex.value);
		inventoryChangedEvent.Invoke();
	}


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
    public override void OnStartButton() { }
}
