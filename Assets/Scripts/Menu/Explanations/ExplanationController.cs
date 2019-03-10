using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplanationController : InputReceiverDelegate {

	public IntVariable currentPage;
	public ExplanationGroup baseStats;
	public ExplanationGroup statsStats;
	public ExplanationGroup inventoryStats;

    [Header("Character stats")]
    public TacticsMoveVariable selectedCharacter;
    public ScrObjEntryReference[] inventory;


    public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.TOOL);
		baseStats.UpdateSelection(active && currentPage.value == (int)InventoryStatsType.BASIC);
		statsStats.UpdateSelection(active && currentPage.value == (int)InventoryStatsType.STATS);
		inventoryStats.UpdateSelection(active && currentPage.value == (int)InventoryStatsType.INVENTORY);

        if (active)
            Setup();
	}

    private void Setup() {
        InventoryContainer inv = selectedCharacter.value.inventory;
        for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
            inventory[i].value = inv.GetTuple(i).item;
        }
    }

    public override void OnDownArrow() {
        if (currentPage.value == (int)InventoryStatsType.BASIC) {
            baseStats.Move(1);
        }
        else if (currentPage.value == (int)InventoryStatsType.STATS) {
            statsStats.Move(1);
        }
        else if (currentPage.value == (int)InventoryStatsType.INVENTORY) {
            inventoryStats.Move(1);
        }
    }

    public override void OnUpArrow() {
        if (currentPage.value == (int)InventoryStatsType.BASIC) {
            baseStats.Move(-1);
        }
        else if (currentPage.value == (int)InventoryStatsType.STATS) {
            statsStats.Move(-1);
        }
        else if (currentPage.value == (int)InventoryStatsType.INVENTORY) {
            inventoryStats.Move(-1);
        }
    }

    public override void OnLButton() {
        OnBackButton();
    }

    public override void OnBackButton() {
		menuBackEvent.Invoke();
		MenuChangeDelay(MenuMode.MAP);
		baseStats.UpdateSelection(false);
		statsStats.UpdateSelection(false);
		inventoryStats.UpdateSelection(false);
    }

    public override void OnYButton() {
        StartCoroutine(WaitForPageUpdate());
    }

    private IEnumerator WaitForPageUpdate() {
        yield return null;
		baseStats.UpdateSelection(currentPage.value == (int)InventoryStatsType.BASIC);
		statsStats.UpdateSelection(currentPage.value == (int)InventoryStatsType.STATS);
		inventoryStats.UpdateSelection(currentPage.value == (int)InventoryStatsType.INVENTORY);
    }

	
    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }
    public override void OnOkButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
}
