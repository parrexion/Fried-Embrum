using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplanationController : InputReceiver {

	public IntVariable currentPage;
	public ExplanationGroup baseStats;
	public ExplanationGroup statsStats;
	public ExplanationGroup inventoryStats;

    [Header("Character stats")]
    public TacticsMoveVariable selectedCharacter;
    public ScrObjEntryReference[] inventory;


    public override void OnMenuModeChanged() {
		MenuMode mode = (MenuMode)currentMenuMode.value;
		active = (mode == MenuMode.TOOL);
		baseStats.UpdateSelection(active && currentPage.value == (int)InventoryStatsType.BASIC);
		statsStats.UpdateSelection(active && currentPage.value == (int)InventoryStatsType.STATS);
		inventoryStats.UpdateSelection(active && currentPage.value == (int)InventoryStatsType.INVENTORY);

        if (active)
            Setup();
	}

    private void Setup() {
        InventoryContainer inv = selectedCharacter.value.inventory;
        for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
            inventory[i].value = inv.GetItem(i).item;
        }
    }

    public override void OnDownArrow() {
        if (!active)
            return;

        if (currentPage.value == (int)InventoryStatsType.BASIC) {
            baseStats.MoveDown();
        }
        else if (currentPage.value == (int)InventoryStatsType.STATS) {
            statsStats.MoveDown();
        }
        else if (currentPage.value == (int)InventoryStatsType.INVENTORY) {
            inventoryStats.MoveDown();
        }
    }

    public override void OnUpArrow() {
        if (!active)
            return;

        if (currentPage.value == (int)InventoryStatsType.BASIC) {
            baseStats.MoveUp();
        }
        else if (currentPage.value == (int)InventoryStatsType.STATS) {
            statsStats.MoveUp();
        }
        else if (currentPage.value == (int)InventoryStatsType.INVENTORY) {
            inventoryStats.MoveUp();
        }
    }

    public override void OnLeftArrow() { }

    public override void OnRightArrow() { }

    public override void OnLButton() {
        OnBackButton();
    }

    public override void OnBackButton() {
        if (!active)
            return;
        
        currentMenuMode.value = (int)MenuMode.MAP;
		menuBackEvent.Invoke();
		StartCoroutine(MenuChangeDelay());
		baseStats.UpdateSelection(false);
		statsStats.UpdateSelection(false);
		inventoryStats.UpdateSelection(false);
    }

    public override void OnYButton() {
        if (!active)
            return;
        StartCoroutine(WaitForPageUpdate());
    }

    private IEnumerator WaitForPageUpdate() {
        yield return null;
		baseStats.UpdateSelection(active && currentPage.value == (int)InventoryStatsType.BASIC);
		statsStats.UpdateSelection(active && currentPage.value == (int)InventoryStatsType.STATS);
		inventoryStats.UpdateSelection(active && currentPage.value == (int)InventoryStatsType.INVENTORY);
    }


    public override void OnOkButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
}
