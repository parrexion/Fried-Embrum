using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExplanationController : InputReceiverDelegate {

	public IntVariable turnCount;

    [Header("Character stats")]
    public TacticsMoveVariable selectedCharacter;
    public InventoryTuple[] inventory;
    public ScrObjEntryReference[] skills;

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
            inventory[i] = inv.GetTuple(i);
        }

        SkillsContainer skill = selectedCharacter.value.skills;
        for (int i = 0; i < SkillsContainer.SKILL_SIZE; i++) {
            skills[i].value = skill.skills[i];
        }

		baseStats.UpdateSelection(page == StatsPage.BASIC);
		statsStats.UpdateSelection(page == StatsPage.STATS);
		inventoryStats.UpdateSelection(page == StatsPage.INVENTORY);
    }

    public override void OnDownArrow() {
        if (page == StatsPage.BASIC) {
            baseStats.MoveDown();
			menuMoveEvent.Invoke();
        }
        else if (page == StatsPage.STATS) {
            statsStats.MoveDown();
			menuMoveEvent.Invoke();
        }
        else if (page == StatsPage.INVENTORY) {
            inventoryStats.MoveDown();
			menuMoveEvent.Invoke();
        }
    }

    public override void OnUpArrow() {
        if (page == StatsPage.BASIC) {
            baseStats.MoveUp();
			menuMoveEvent.Invoke();
        }
        else if (page == StatsPage.STATS) {
            statsStats.MoveUp();
			menuMoveEvent.Invoke();
        }
        else if (page == StatsPage.INVENTORY) {
            inventoryStats.MoveUp();
			menuMoveEvent.Invoke();
        }
    }

    public override void OnLeftArrow() {
        if (page == StatsPage.BASIC) {
            baseStats.MoveLeft();
			menuMoveEvent.Invoke();
        }
        else if (page == StatsPage.STATS) {
            statsStats.MoveLeft();
			menuMoveEvent.Invoke();
        }
        else if (page == StatsPage.INVENTORY) {
            inventoryStats.MoveLeft();
			menuMoveEvent.Invoke();
        }
    }

    public override void OnRightArrow() {
        if (page == StatsPage.BASIC) {
            baseStats.MoveRight();
			menuMoveEvent.Invoke();
        }
        else if (page == StatsPage.STATS) {
            statsStats.MoveRight();
			menuMoveEvent.Invoke();
        }
        else if (page == StatsPage.INVENTORY) {
            inventoryStats.MoveRight();
			menuMoveEvent.Invoke();
        }
    }

    public override void OnLButton() {
        OnBackButton();
		menuBackEvent.Invoke();
    }

    public override void OnBackButton() {
		baseStats.UpdateSelection(false);
		statsStats.UpdateSelection(false);
		inventoryStats.UpdateSelection(false);
		if (turnCount.value == 0)
			MenuChangeDelay(MenuMode.FORMATION);
		else
			MenuChangeDelay(MenuMode.MAP);
		menuBackEvent.Invoke();
    }

    public override void OnYButton() {
		if (currentMenuMode.value == (int)MenuMode.INV || changing)
			return;

		changing = true;
        StartCoroutine(ChangeStatsScreen());
		menuAcceptEvent.Invoke();
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
	
	
    public override void OnOkButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
}
