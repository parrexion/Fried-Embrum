using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplanationInventoryObject : ExplanationObject {

	public TacticsMoveVariable selectedCharacter;
	public int index;


	public override string GetTooltip() {
		if (selectedCharacter.value != null) {
			InventoryTuple tuple = selectedCharacter.value.inventory.GetItem(index);
			return (!string.IsNullOrEmpty(tuple.uuid)) ? tuple.Description() : "-EMPTY-";
		}

		return base.GetTooltip();
	}

}
