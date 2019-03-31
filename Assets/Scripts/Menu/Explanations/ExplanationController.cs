using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExplanationController : InputReceiverDelegate {

    [Header("Character stats")]
    public TacticsMoveVariable selectedCharacter;
    public ScrObjEntryReference[] inventory;

	[Header("Pages")]
	public IntVariable currentPage;
	public ExplanationGroup baseStats;
	public ExplanationGroup statsStats;
	public ExplanationGroup inventoryStats;

	[Header("Tooltip")]
	public GameObject tooltipObject;
	public UnityEvent changeStatsPageEvent;

	private StatsPage page;
	private bool changing;


	private void Start() {
		tooltipObject.SetActive(false);
		baseStats.UpdateSelection(false);
		statsStats.UpdateSelection(false);
		inventoryStats.UpdateSelection(false);
	}

	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.TOOLTIP);
		page = (currentMenuMode.value == (int)MenuMode.INV) ? StatsPage.INVENTORY : (StatsPage)currentPage.value;

		tooltipObject.SetActive(active);
        if (active)
            Setup();
	}

	/// <summary>
	/// Updates the inventory and the explanation groups.
	/// </summary>
    private void Setup() {
        InventoryContainer inv = selectedCharacter.value.inventory;
        for (int i = 0; i < InventoryContainer.INVENTORY_SIZE; i++) {
            inventory[i].value = inv.GetTuple(i).item;
        }

		baseStats.UpdateSelection(page == StatsPage.BASIC);
		statsStats.UpdateSelection(page == StatsPage.STATS);
		inventoryStats.UpdateSelection(page == StatsPage.INVENTORY);
    }

    public override void OnDownArrow() {
        if (page == StatsPage.BASIC) {
            baseStats.Move(1);
        }
        else if (page == StatsPage.STATS) {
            statsStats.Move(1);
        }
        else if (page == StatsPage.INVENTORY) {
            inventoryStats.Move(1);
        }
    }

    public override void OnUpArrow() {
        if (page == StatsPage.BASIC) {
            baseStats.Move(-1);
        }
        else if (page == StatsPage.STATS) {
            statsStats.Move(-1);
        }
        else if (page == StatsPage.INVENTORY) {
            inventoryStats.Move(-1);
        }
    }

    public override void OnLButton() {
        OnBackButton();
    }

    public override void OnBackButton() {
		menuBackEvent.Invoke();
		baseStats.UpdateSelection(false);
		statsStats.UpdateSelection(false);
		inventoryStats.UpdateSelection(false);
		MenuChangeDelay(MenuMode.MAP);
    }

    public override void OnYButton() {
		if (currentMenuMode.value == (int)MenuMode.INV || changing)
			return;

		changing = true;
        StartCoroutine(ChangeStatsScreen());
    }

    /// <summary>
	/// Changes the stats screen to the next one if possible.
	/// </summary>
	/// <param name="dir"></param>
	private IEnumerator ChangeStatsScreen() {
		changeStatsPageEvent.Invoke();
		yield return null;
		page = (StatsPage)currentPage.value;
		Setup();
		changing = false;
	}
	
	
    public override void OnLeftArrow() { }
    public override void OnRightArrow() { }
    public override void OnOkButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
}
