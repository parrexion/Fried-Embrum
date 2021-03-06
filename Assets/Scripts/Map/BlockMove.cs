﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMove : TacticsMove {

	/// <summary>
	/// Additional setup for player characters.
	/// </summary>
	protected override void ExtraSetup() { }

	/// <summary>
	/// Removes the character from the NPC list.
	/// </summary>
	public override void RemoveFromList() {	}

	/// <summary>
	/// Additional functions which run when the player ends their turn.
	/// </summary>
	public override void EndMovement() { }
	
	protected override IEnumerator OnDeath() {
		GetComponent<SpriteRenderer>().color = new Color(0.4f,0.4f,0.4f);
		currentTile.currentCharacter = null;
		yield return new WaitForSeconds(0.4f);
		sfxQueue.Enqueue(deathSfx);
		playSfxEvent.Invoke();
		yield return new WaitForSeconds(1f);
		currentTile.SetTerrain(currentTile.alternativeTerrain);
	}

	public override void ReceiveBuff(Boost boost, bool isBuff, bool useAnim) {
		//No buffs for blocks.
	}
}
