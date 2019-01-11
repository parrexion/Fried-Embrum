﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreaparationController : MonoBehaviour {
    
    public TacticsMoveVariable selectCharacter;
    // public CharacterStatsVariable clickCharacter;
    public MapTileVariable targetTile;
    public ActionModeVariable currentMode;
    public FactionVariable currentTurn;
    
    [Header("Data")]
    public SaveListVariable equippedUnits;
    public SaveListVariable availableUnits;
    public CharacterSaveData noPlayer;
    public CharacterSaveData[] charactersSave;
    
    [Header("Other")]
    public Button startButton;
    
    
    private void Awake() {
        selectCharacter.value = null;
        // clickCharacter.stats = null;
        // clickCharacter.inventory = null;
        targetTile.value = null;
        currentMode.value = ActionMode.NONE;
        currentTurn.value = Faction.NONE;

        SetupCharacter();
        CheckButton();
    }


    private void SetupCharacter() {
//        for (int i = 0; i < playerData.values.Length; i++) {
//            playerData.values[i] = new StatsContainer(null, null);
//        }
    }

    public void CheckButton() {
        //TODO
//        if (startButton == null)
//            return;
        
//        bool available = false;
//        for (int i = 0; i < equippedUnits.values.Length; i++) {
//            if (equippedUnits.values[i].id != -1) {
//                available = true;
//                break;
//            }
//        }

//        startButton.interactable = available;
    }
}
