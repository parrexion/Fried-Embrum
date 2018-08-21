using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LibraryEntries/Battle")]
public class BattleEntry : ScrObjLibraryEntry {

	public enum RemoveSide {NONE,LEFT,RIGHT};


	// General battle things
	public bool escapeButtonEnabled = true;
	
	// Tutorial stuff
	public bool isTutorial = false;
	public Sprite backgroundHintLeft = null;
	public Sprite backgroundHintRight = null;
	public RemoveSide removeSide = RemoveSide.NONE;
	public bool playerInvincible = false;
	public bool useSlowTime = true;

	// Background
	public Sprite backgroundLeft = null;
	public Sprite backgroundRight = null;

	// Enemies 
	public bool randomizeEnemies = false;
	public int numberOfEnemies = 1;
	public List<EnemyEntry> enemyTypes = new List<EnemyEntry>();

	// Player stuff
	public bool useSpecificModule = false;

	// After match values
	public bool changePosition = false;
	public Vector2 playerPosition = new Vector2();
	public DialogueEntry nextDialogue = null;


	/// <summary>
	/// Resets all values to start values.
	/// </summary>
	/// <param name="other"></param>
	public override void ResetValues() {
		base.ResetValues();

		// General battle things
		escapeButtonEnabled = true;

		// Tutorial stuff
		isTutorial = false;
		backgroundHintLeft = null;
		backgroundHintRight = null;
		removeSide = RemoveSide.NONE;
		playerInvincible = false;
		useSlowTime = true;

		// Background
		backgroundLeft = null;
		backgroundRight = null;

		// Enemies 
		randomizeEnemies = false;
		numberOfEnemies = 1;
		enemyTypes = new List<EnemyEntry>();

		// Player stuff
		useSpecificModule = true;

		// After match values
		changePosition = false;
		playerPosition = new Vector2();
		nextDialogue = null;
	}

	/// <summary>
	/// Copies all the values from another BattleEntry.
	/// </summary>
	/// <param name="battle"></param>
	public override void CopyValues(ScrObjLibraryEntry other) {
		base.CopyValues(other);
		BattleEntry be = (BattleEntry) other;

		// General battle things
		escapeButtonEnabled = be.escapeButtonEnabled;

		// Tutorial stuff
		isTutorial = be.isTutorial;
		backgroundHintLeft = be.backgroundHintLeft;
		backgroundHintRight = be.backgroundHintRight;
		removeSide =  be.removeSide;
		playerInvincible = be.playerInvincible;
		useSlowTime = be.useSlowTime;

		// Background
		backgroundLeft = be.backgroundLeft;
		backgroundRight = be.backgroundRight;

		// Enemies 
		randomizeEnemies = be.randomizeEnemies;
		numberOfEnemies = be.numberOfEnemies;
		enemyTypes = new List<EnemyEntry>();
		for (int i = 0; i < be.enemyTypes.Count; i++) {
			enemyTypes.Add(be.enemyTypes[i]);
		}

		
		// After match values
		changePosition = be.changePosition;
		playerPosition = be.playerPosition;
		nextDialogue = be.nextDialogue;
	}
}
