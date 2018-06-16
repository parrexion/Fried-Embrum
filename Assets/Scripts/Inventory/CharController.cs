using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController : MonoBehaviour {
    
    public TacticsMoveVariable selectCharacter;
    public CharacterStatsVariable clickCharacter;
    public MapTileVariable targetTile;
    public ActionModeVariable currentMode;
    public FactionVariable currentTurn;
    
    [Header("Data")]
    public SaveListVariable equippedUnits;
    public SaveListVariable enemyUnits;
    public CharacterSaveData noPlayer;
    
    
    private void Awake() {
        selectCharacter.value = null;
        clickCharacter.value = null;
        targetTile.value = null;
        currentMode.value = ActionMode.NONE;
        currentTurn.value = Faction.NONE;

        SetupCharacter();
    }


    private void SetupCharacter() {
        
    }
}
