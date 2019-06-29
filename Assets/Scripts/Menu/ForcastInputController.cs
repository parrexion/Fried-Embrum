using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ForcastInputController : InputReceiverDelegate {

	public ForecastUI forecast;

	public TacticsMoveVariable selectedCharacter;
	public MapTileVariable defendTile;
	public IntVariable battleWeaponIndex;
	public ActionModeVariable currentAction;

	public UnityEvent characterChangedEvent;

	private List<InventoryTuple> attackerWeapons;
	private int listIndex;


	private void Start() {
		OnMenuModeChanged();
	}

	public override void OnMenuModeChanged() {
		bool active = UpdateState(MenuMode.WEAPON);
		forecast.UpdateUI(active || currentMenuMode.value == (int)MenuMode.BATTLE);

		if (!active)
			return;

		if (currentAction.value == ActionMode.ATTACK)
			attackerWeapons = selectedCharacter.value.inventory.GetAllUsableItemTuple(ItemCategory.WEAPON);
		else
			attackerWeapons = selectedCharacter.value.inventory.GetAllUsableItemTuple(ItemCategory.SUPPORT);
		listIndex = -1;
		ChangeWeapon(1);
		characterChangedEvent.Invoke();
	}

	public override void OnLeftArrow() {
		ChangeWeapon(-1);
	}

	public override void OnRightArrow() {
		ChangeWeapon(1);
	}

	public override void OnOkButton() {
		if (currentAction.value == ActionMode.ATTACK) {
			selectedCharacter.value.Attack(defendTile.value);
			MenuChangeDelay(MenuMode.BATTLE);
			menuAcceptEvent.Invoke();
		}
		else if (currentAction.value == ActionMode.HEAL) {
			selectedCharacter.value.Heal(defendTile.value);
			MenuChangeDelay(MenuMode.BATTLE);
			menuAcceptEvent.Invoke();
		}
	}

	public override void OnBackButton() {
		battleWeaponIndex.value = attackerWeapons[0].index;
		MenuChangeDelay(MenuMode.MAP);
		menuBackEvent.Invoke();
	}

	/// <summary>
	/// Changes to the next available weapon without equipping it for the forecast.
	/// </summary>
	/// <param name="diff"></param>
	public void ChangeWeapon(int diff) {
		bool inRange = false;
		int startIndex = listIndex;
		do {
			listIndex = OPMath.FullLoop(0, attackerWeapons.Count, listIndex + diff);
			inRange = attackerWeapons[listIndex].item.InRange(BattleMap.DistanceTo(selectedCharacter.value, defendTile.value));
			battleWeaponIndex.value = attackerWeapons[listIndex].index;
		} while (!inRange && startIndex != listIndex);

		if (startIndex != listIndex)
			menuMoveEvent.Invoke();
		forecast.UpdateUI(true);
	}


	public override void OnUpArrow() {}
	public override void OnDownArrow() {}
    public override void OnLButton() {}
    public override void OnRButton() {}
	public override void OnXButton() {}
	public override void OnYButton() {}
	public override void OnStartButton() {}

}
