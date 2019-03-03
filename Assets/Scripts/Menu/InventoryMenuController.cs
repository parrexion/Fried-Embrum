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
	public IntVariable inventoryMenuPosition;
	private Image[] inventoryButtons = new Image[0];

	public UnityEvent inventoryChangedEvent;
	public UnityEvent playSfxEvent;
	
	
	private void Start () {
		inventoryMenu.SetActive(false);
		inventoryButtons = inventoryMenu.GetComponentsInChildren<Image>(true);
	}

    public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.INV);
		inventoryMenu.SetActive(active);
		if (active)
			ButtonSetup();
    }

	private void ButtonSetup() {
		InventoryTuple tuple = selectedCharacter.value.inventory.GetTuple(inventoryIndex.value);
		int skill = selectedCharacter.value.stats.GetWpnSkill(tuple.item);
		inventoryButtons[0].gameObject.SetActive(tuple.item.itemCategory == ItemCategory.WEAPON && tuple.item.CanUse(skill));
		inventoryButtons[1].gameObject.SetActive(tuple.item.itemCategory == ItemCategory.CONSUME);
		
		if (inventoryMenuPosition.value == -1)
			OnDownArrow();
		ButtonHighlighting();
	}

	/// <summary>
	/// Colors the selected button to show the current selection.
	/// </summary>
	private void ButtonHighlighting() {
		for (int i = 0; i < inventoryButtons.Length; i++) {
			inventoryButtons[i].color = (inventoryMenuPosition.value == i) ? Color.cyan : Color.white;
		}
	}

    public override void OnUpArrow() {
		do {
			inventoryMenuPosition.value--;
			if (inventoryMenuPosition.value < 0)
				inventoryMenuPosition.value = inventoryButtons.Length-1;
		} while (!inventoryButtons[inventoryMenuPosition.value].gameObject.activeSelf);

		menuMoveEvent.Invoke();
		ButtonHighlighting();
    }

    public override void OnDownArrow() {
		do {
			inventoryMenuPosition.value++;
			if (inventoryMenuPosition.value >= inventoryButtons.Length)
				inventoryMenuPosition.value = 0;
		} while (!inventoryButtons[inventoryMenuPosition.value].gameObject.activeSelf);
			
		menuMoveEvent.Invoke();
		ButtonHighlighting();
    }

    public override void OnOkButton() {
		switch (inventoryMenuPosition.value)
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

    public override void OnBackButton() {
		menuBackEvent.Invoke();
		InputDelegateController.instance.TriggerMenuChange(MenuMode.STATS);
		inventoryIndex.value = -1;
		ButtonHighlighting();
    }

	/// <summary>
	/// Equips the selected item and moves the cursor to the new position.
	/// </summary>
	public void EquipItem() {
		selectedCharacter.value.inventory.EquipItem(inventoryIndex.value);
		inventoryIndex.value = 0;
		inventoryChangedEvent.Invoke();
		InputDelegateController.instance.TriggerMenuChange(MenuMode.STATS);
	}

	/// <summary>
	/// Uses the selected item.
	/// </summary>
	public void UseItem() {
		InventoryTuple tup = selectedCharacter.value.inventory.GetTuple(inventoryIndex.value);
		SfxEntry sfx = (tup.item.itemType == ItemType.CHEAL) ? healItemSfx : boostItemSfx;
		sfxQueue.Enqueue(sfx);
		playSfxEvent.Invoke();

		selectedCharacter.value.inventory.UseItem(inventoryIndex.value, selectedCharacter.value);
		inventoryIndex.value = -1;
		ButtonHighlighting();
		currentMode.value = ActionMode.NONE;
		inventoryChangedEvent.Invoke();
		selectedCharacter.value.End();
		InputDelegateController.instance.TriggerMenuChange(MenuMode.MAP);
	}

	/// <summary>
	/// Drops the selected item.
	/// </summary>
	public void DropItem() {
		selectedCharacter.value.inventory.DropItem(inventoryIndex.value, selectedCharacter.value.stats);
		inventoryChangedEvent.Invoke();
		InputDelegateController.instance.TriggerMenuChange(MenuMode.STATS);
	}


    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }
    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
    public override void OnStartButton() { }
}
