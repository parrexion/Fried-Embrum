using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcastInputController : InputReceiver {

	public ForecastUI forecast;

	public TacticsMoveVariable selectedCharacter;
	public MapTileVariable defendCharacter;
	public IntVariable battleWeaponIndex;

	private List<InventoryTuple> attackerWeapons;
	private int listIndex;


	private void Start() {
		OnMenuModeChanged();
	}

	public override void OnMenuModeChanged() {
		active = (currentMenuMode.value == (int)MenuMode.ATTACK || currentMenuMode.value == (int)MenuMode.HEAL);
		forecast.UpdateUI(active);

		if (!active)
			return;

		listIndex = 0;
		if (currentMenuMode.value == (int)MenuMode.ATTACK)
			attackerWeapons = selectedCharacter.value.inventory.GetAllUsableItemTuple(ItemCategory.WEAPON, selectedCharacter.value.stats);
		else
			attackerWeapons = selectedCharacter.value.inventory.GetAllUsableItemTuple(ItemCategory.STAFF, selectedCharacter.value.stats);
		battleWeaponIndex.value = attackerWeapons[listIndex].index;
	}

	public override void OnLeftArrow() {
		if (!active)
			return;

		ChangeWeapon(-1);
	}

	public override void OnRightArrow() {
		if (!active)
			return;

		ChangeWeapon(1);
	}

	public override void OnOkButton() {
		if (!active)
			return;

		if (currentMenuMode.value == (int)MenuMode.ATTACK) {
			selectedCharacter.value.Attack(defendCharacter.value);
			menuAcceptEvent.Invoke();
		}
		else if (currentMenuMode.value == (int)MenuMode.HEAL) {
			selectedCharacter.value.Heal(defendCharacter.value);
			menuAcceptEvent.Invoke();
		}
	}

	public override void OnBackButton() {
		if (!active)
			return;

		battleWeaponIndex.value = attackerWeapons[0].index;
		currentMenuMode.value = (int)MenuMode.MAP;
		menuBackEvent.Invoke();
		StartCoroutine(MenuChangeDelay());
	}

	/// <summary>
	/// Changes to the next available weapon without equipping it for the forecast.
	/// </summary>
	/// <param name="diff"></param>
	public void ChangeWeapon(int diff) {
		int startIndex = listIndex;
		listIndex += diff;
		listIndex = (listIndex >= 0) ? listIndex % attackerWeapons.Count : listIndex + attackerWeapons.Count;
		battleWeaponIndex.value = attackerWeapons[listIndex].index;
		if (startIndex != listIndex)
			menuMoveEvent.Invoke();
		forecast.UpdateUI(active);
	}


	public override void OnUpArrow() {}
	public override void OnDownArrow() {}
	public override void OnSp1Button() {}
	public override void OnSp2Button() {}
	public override void OnStartButton() {}

}
