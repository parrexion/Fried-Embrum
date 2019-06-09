using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplanationSkillObject : ExplanationObject {

	public override string GetTooltip() {
		if (scrObject != null) {
			return (scrObject.value != null) ? ((CharacterSkill)scrObject.value).description : "-EMPTY-";
		}

		return fallbackString;
	}

}
