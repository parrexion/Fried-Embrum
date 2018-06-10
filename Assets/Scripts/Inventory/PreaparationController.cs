using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreaparationController : MonoBehaviour {
    
    public TacticsMoveVariable selectCharacter;
    public CharacterStatsVariable clickCharacter;
    public MapTileVariable targetTile;
    public ActionModeVariable currentMode;
    public FactionVariable currentTurn;
    
    [Header("Data")]
    public SaveListVariable equippedUnits;
    public SaveListVariable availableUnits;
    public CharacterSave noPlayer;
    public CharacterSave[] characters;
    
    [Header("Other")]
    public Button startButton;
    
    
    private void Awake() {
        selectCharacter.value = null;
        clickCharacter.value = null;
        targetTile.value = null;
        currentMode.value = ActionMode.NONE;
        currentTurn.value = Faction.NONE;

        SetupCharacter();
        CheckButton();
    }


    private void SetupCharacter() {
        for (int i = 0; i < equippedUnits.values.Length; i++) {
            equippedUnits.values[i] = new StatsContainer(null,null);
        }
//        for (int i = 0; i < availableUnits.values.Length; i++) {
//            availableUnits.values[i] = new StatsContainer((i < characters.Length) ? characters[i] : noPlayer);
//        }
    }

    public void CheckButton() {
        if (startButton == null)
            return;
        
        bool available = false;
        for (int i = 0; i < equippedUnits.values.Length; i++) {
            if (equippedUnits.values[i].id != -1) {
                available = true;
                break;
            }
        }

        startButton.interactable = available;
    }
}
