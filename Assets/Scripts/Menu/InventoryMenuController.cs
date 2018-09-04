using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryMenuController : InputReceiver {

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

	[Header("Tooltip")]
	public GameObject tooltipObject;
	public Text tooltipText;
	public StringVariable tooltipMessage;

	public UnityEvent inventoryChangedEvent;
	public UnityEvent playSfxEvent;
	
	
	private void Start () {
		tooltipObject.SetActive(false);
		inventoryMenu.SetActive(false);
		inventoryButtons = inventoryMenu.GetComponentsInChildren<Image>(true);
	}

    public override void OnMenuModeChanged() {
		MenuMode mode = (MenuMode)currentMenuMode.value;
		active = (mode == MenuMode.INV);
		inventoryMenu.SetActive(active);
		ButtonSetup();
    }

	private void ButtonSetup() {
		if (!active)
			return;
			
		InventoryTuple tuple = selectedCharacter.value.inventory.GetItem(inventoryIndex.value);
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
		if (!active)
			return;

		do {
			inventoryMenuPosition.value--;
			if (inventoryMenuPosition.value < 0)
				inventoryMenuPosition.value = inventoryButtons.Length-1;
		} while (!inventoryButtons[inventoryMenuPosition.value].gameObject.activeSelf);

		menuMoveEvent.Invoke();
		ButtonHighlighting();
    }

    public override void OnDownArrow() {
		if (!active)
			return;

		do {
			inventoryMenuPosition.value++;
			if (inventoryMenuPosition.value >= inventoryButtons.Length)
				inventoryMenuPosition.value = 0;
		} while (!inventoryButtons[inventoryMenuPosition.value].gameObject.activeSelf);
			
		menuMoveEvent.Invoke();
		ButtonHighlighting();
    }

    public override void OnOkButton() {
		if (!active)
			return;

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
		if (!active)
			return;

        currentMenuMode.value = (int)MenuMode.STATS;
		menuBackEvent.Invoke();
		StartCoroutine(MenuChangeDelay());
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
		currentMenuMode.value = (int)MenuMode.STATS;
		StartCoroutine(MenuChangeDelay());
	}

	/// <summary>
	/// Uses the selected item.
	/// </summary>
	public void UseItem() {
		InventoryTuple tup = selectedCharacter.value.inventory.GetItem(inventoryIndex.value);
		SfxEntry sfx = (tup.item.itemType == ItemType.CHEAL) ? healItemSfx : boostItemSfx;
		sfxQueue.Enqueue(sfx);
		playSfxEvent.Invoke();

		selectedCharacter.value.inventory.UseItem(inventoryIndex.value, selectedCharacter.value);
		inventoryIndex.value = -1;
		ButtonHighlighting();
		currentMenuMode.value = (int)MenuMode.MAP;
		currentMode.value = ActionMode.NONE;
		inventoryChangedEvent.Invoke();
		selectedCharacter.value.End();
		StartCoroutine(MenuChangeDelay());
	}

	/// <summary>
	/// Drops the selected item.
	/// </summary>
	public void DropItem() {
		selectedCharacter.value.inventory.DropItem(inventoryIndex.value);
		inventoryChangedEvent.Invoke();
		currentMenuMode.value = (int)MenuMode.STATS;
		StartCoroutine(MenuChangeDelay());
	}


    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }
    public override void OnSp1Button() { }
    public override void OnSp2Button() { }
    public override void OnStartButton() { }
}

	// private void ShowTooltip() {
	// 	if (string.IsNullOrEmpty(tooltipMessage.value)) {
	// 		hideTooltipEvent.Invoke();
	// 		return;
	// 	}
	// 	tooltipObject.SetActive(true);
	// 	tooltipText.text = tooltipMessage.value;
	// }

	// public void HideTooltip() {
	// 	selectedItem.value = -1;
	// 	tooltipObject.SetActive(false);
	// 	inventoryMenu.SetActive(false);
	// }